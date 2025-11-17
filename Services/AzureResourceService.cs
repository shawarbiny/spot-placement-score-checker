using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using SpotPlacementScoreWebsite.Models;
using System.Text.Json;

namespace SpotPlacementScoreWebsite.Services
{
    /// <summary>
    /// Service to interact with Azure Resource Manager APIs
    /// Uses delegated token from authenticated user for secure access
    /// </summary>
    public class AzureResourceService : IAzureResourceService
    {
        private readonly ILogger<AzureResourceService> _logger;
        private readonly HttpClient _httpClient;

        public AzureResourceService(ILogger<AzureResourceService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public AzureResourceService(ILogger<AzureResourceService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Get all subscriptions accessible to the authenticated user
        /// </summary>
        public async Task<List<SubscriptionInfo>> GetSubscriptionsAsync(string accessToken)
        {
            try
            {
                var subscriptions = new List<SubscriptionInfo>();
                var url = "https://management.azure.com/subscriptions?api-version=2022-12-01";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var items = json.RootElement.GetProperty("value");

                foreach (var item in items.EnumerateArray())
                {
                    subscriptions.Add(new SubscriptionInfo
                    {
                        SubscriptionId = item.GetProperty("subscriptionId").GetString() ?? "",
                        DisplayName = item.GetProperty("displayName").GetString() ?? ""
                    });
                }

                _logger.LogInformation("Retrieved {Count} subscriptions", subscriptions.Count);
                return subscriptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subscriptions");
                throw;
            }
        }

        /// <summary>
        /// Get available VM instance series
        /// </summary>
        public async Task<List<string>> GetInstanceSeriesAsync(string subscriptionId, string accessToken)
        {
            try
            {
                var seriesSet = new HashSet<string>();
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Compute/skus?api-version=2021-07-01&$filter=location eq 'eastus'";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var items = json.RootElement.GetProperty("value");

                foreach (var item in items.EnumerateArray())
                {
                    var resourceType = item.GetProperty("resourceType").GetString();
                    if (resourceType != "virtualMachines") continue;

                    var name = item.GetProperty("name").GetString() ?? "";
                    
                    // Extract series from SKU name (e.g., "Standard_D4s_v3" -> "Dsv3-series")
                    var series = ExtractSeriesFromSku(name);
                    if (!string.IsNullOrEmpty(series))
                    {
                        seriesSet.Add(series);
                    }
                }

                var seriesList = seriesSet.OrderBy(s => s).ToList();
                _logger.LogInformation("Retrieved {Count} instance series", seriesList.Count);
                return seriesList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching instance series");
                throw;
            }
        }

        /// <summary>
        /// Get available VM SKUs for a subscription, optionally filtered by series
        /// </summary>
        public async Task<List<VmSkuInfo>> GetVmSkusAsync(string subscriptionId, string? series, string accessToken)
        {
            try
            {
                var skus = new List<VmSkuInfo>();
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Compute/skus?api-version=2021-07-01&$filter=location eq 'eastus'";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var items = json.RootElement.GetProperty("value");

                foreach (var item in items.EnumerateArray())
                {
                    var resourceType = item.GetProperty("resourceType").GetString();
                    if (resourceType != "virtualMachines") continue;

                    var name = item.GetProperty("name").GetString() ?? "";
                    
                    // Filter by series if provided
                    if (!string.IsNullOrEmpty(series))
                    {
                        var skuSeries = ExtractSeriesFromSku(name);
                        if (skuSeries != series) continue;
                    }
                    else
                    {
                        // If no series specified, filter to common SKUs only
                        if (!name.StartsWith("Standard_D") && !name.StartsWith("Standard_E") && 
                            !name.StartsWith("Standard_F") && !name.StartsWith("Standard_B"))
                            continue;
                    }

                    var capabilities = item.GetProperty("capabilities");
                    int vcpus = 0;
                    int memoryGB = 0;

                    foreach (var cap in capabilities.EnumerateArray())
                    {
                        var capName = cap.GetProperty("name").GetString();
                        var value = cap.GetProperty("value").GetString();

                        if (capName == "vCPUs") int.TryParse(value, out vcpus);
                        if (capName == "MemoryGB") int.TryParse(value, out memoryGB);
                    }

                    skus.Add(new VmSkuInfo
                    {
                        Name = name,
                        Size = name,
                        VCpus = vcpus,
                        MemoryGB = memoryGB
                    });
                }

                _logger.LogInformation("Retrieved {Count} VM SKUs for series {Series}", skus.Count, series ?? "all");
                return skus.OrderBy(s => s.Name).Take(50).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching VM SKUs");
                throw;
            }
        }

        /// <summary>
        /// Extract series name from SKU name
        /// Examples: Standard_D4s_v3 -> Dsv3-series, Standard_E8_v5 -> Ev5-series
        /// </summary>
        private string ExtractSeriesFromSku(string skuName)
        {
            if (!skuName.StartsWith("Standard_")) return string.Empty;

            var parts = skuName.Substring(9); // Remove "Standard_"
            
            // Extract letter and version (e.g., "D4s_v3" -> "Dsv3")
            var match = System.Text.RegularExpressions.Regex.Match(parts, @"^([A-Z]+)\d+([a-z]*)(_v\d+)?");
            if (match.Success)
            {
                var letter = match.Groups[1].Value;
                var modifier = match.Groups[2].Value;
                var version = match.Groups[3].Value.Replace("_v", "v");
                
                return $"{letter}{modifier}{version}-series";
            }

            return string.Empty;
        }

        /// <summary>
        /// Get all Azure regions
        /// </summary>
        public async Task<List<RegionInfo>> GetRegionsAsync(string subscriptionId, string accessToken)
        {
            try
            {
                var regions = new List<RegionInfo>();
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/locations?api-version=2022-12-01";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var items = json.RootElement.GetProperty("value");

                foreach (var item in items.EnumerateArray())
                {
                    regions.Add(new RegionInfo
                    {
                        Name = item.GetProperty("name").GetString() ?? "",
                        DisplayName = item.GetProperty("displayName").GetString() ?? ""
                    });
                }

                _logger.LogInformation("Retrieved {Count} regions", regions.Count);
                return regions.OrderBy(r => r.DisplayName).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching regions");
                throw;
            }
        }
    }
}
