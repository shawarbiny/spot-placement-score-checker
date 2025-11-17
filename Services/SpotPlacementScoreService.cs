using SpotPlacementScoreWebsite.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SpotPlacementScoreWebsite.Services
{
    /// <summary>
    /// Service to check Azure Spot VM Placement Scores
    /// Uses Azure Compute REST API to determine spot VM availability
    /// </summary>
    public class SpotPlacementScoreService : ISpotPlacementScoreService
    {
        private readonly ILogger<SpotPlacementScoreService> _logger;
        private readonly HttpClient _httpClient;

        public SpotPlacementScoreService(ILogger<SpotPlacementScoreService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public SpotPlacementScoreService(ILogger<SpotPlacementScoreService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Check spot placement scores for multiple SKU and region combinations
        /// Makes a single API call with all SKUs and all regions
        /// </summary>
        public async Task<(List<SpotPlacementScoreResult> Results, string RequestJson, string ResponseJson)> CheckSpotPlacementScoreAsync(
            string subscriptionId,
            List<string> vmSkus,
            List<string> regions,
            bool useAvailabilityZones,
            int desiredCount,
            string accessToken)
        {
            var results = new List<SpotPlacementScoreResult>();
            string requestJson = string.Empty;
            string responseJson = string.Empty;

            try
            {
                // Use the first region as the location for the API call (it accepts all regions in the body)
                var primaryRegion = regions.FirstOrDefault() ?? "eastus";
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Compute/locations/{primaryRegion}/placementScores/spot/generate?api-version=2025-06-05";

                var requestBody = new
                {
                    desiredLocations = regions.ToArray(),
                    desiredSizes = vmSkus.Select(sku => new { sku }).ToArray(),
                    desiredCount = desiredCount,
                    availabilityZones = useAvailabilityZones
                };

                // Serialize request for logging
                requestJson = JsonSerializer.Serialize(new 
                { 
                    url = url,
                    method = "POST",
                    body = requestBody 
                }, new JsonSerializerOptions { WriteIndented = true });

                _logger.LogInformation("Making single API call for {SkuCount} SKUs across {RegionCount} regions", vmSkus.Count, regions.Count);

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    // Format response for display
                    responseJson = JsonSerializer.Serialize(
                        JsonSerializer.Deserialize<JsonElement>(responseContent), 
                        new JsonSerializerOptions { WriteIndented = true });
                    
                    var json = JsonDocument.Parse(responseContent);

                    // Parse the response for spot placement scores
                    if (json.RootElement.TryGetProperty("placementScores", out var scores))
                    {
                        foreach (var scoreElement in scores.EnumerateArray())
                        {
                            // Response format: { "region": "eastus", "sku": "Standard_D2_v3", "availabilityZone": "1", "score": "High", "isQuotaAvailable": true }
                            var scoreRegion = scoreElement.TryGetProperty("region", out var regionValue) 
                                ? regionValue.GetString() ?? "Unknown" 
                                : "Unknown";
                            
                            var vmSize = scoreElement.TryGetProperty("sku", out var size) 
                                ? size.GetString() ?? "Unknown" 
                                : "Unknown";

                            var availabilityZone = scoreElement.TryGetProperty("availabilityZone", out var azValue) 
                                ? azValue.GetString() ?? "N/A" 
                                : "N/A";

                            var score = scoreElement.TryGetProperty("score", out var scoreValue) 
                                ? scoreValue.GetString() ?? "Unknown" 
                                : "Unknown";
                            
                            var isAvailable = scoreElement.TryGetProperty("isQuotaAvailable", out var quotaAvail) 
                                ? quotaAvail.GetBoolean() 
                                : false;

                            // Consider High and Medium scores as available
                            var available = isAvailable && 
                                           (score.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                                            score.Equals("Medium", StringComparison.OrdinalIgnoreCase));

                            results.Add(new SpotPlacementScoreResult
                            {
                                Region = scoreRegion,
                                VmSku = vmSize,
                                AvailabilityZone = availabilityZone,
                                Score = score,
                                IsAvailable = available,
                                Message = GetScoreMessage(score, available)
                            });
                        }
                    }
                    
                    // If no scores returned, create error entries for each combination
                    if (results.Count == 0)
                    {
                        foreach (var region in regions)
                        {
                            foreach (var sku in vmSkus)
                            {
                                results.Add(new SpotPlacementScoreResult
                                {
                                    Region = region,
                                    VmSku = sku,
                                    AvailabilityZone = "N/A",
                                    Score = "No Score",
                                    IsAvailable = false,
                                    Message = "No placement score available"
                                });
                            }
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    responseJson = JsonSerializer.Serialize(new 
                    { 
                        statusCode = (int)response.StatusCode,
                        error = errorContent 
                    }, new JsonSerializerOptions { WriteIndented = true });
                    
                    _logger.LogWarning("Failed to get spot placement scores: {Status} - {Error}", 
                        response.StatusCode, errorContent);

                    // Create error entries for each combination
                    foreach (var region in regions)
                    {
                        foreach (var sku in vmSkus)
                        {
                            results.Add(new SpotPlacementScoreResult
                            {
                                Region = region,
                                VmSku = sku,
                                AvailabilityZone = "N/A",
                                Score = "Error",
                                IsAvailable = false,
                                Message = $"API Error: {response.StatusCode}"
                            });
                        }
                    }
                }

                _logger.LogInformation("Completed spot placement score check for {Count} combinations", results.Count);
                return (results, requestJson, responseJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking spot placement scores");
                
                responseJson = JsonSerializer.Serialize(new 
                { 
                    error = ex.Message,
                    stackTrace = ex.StackTrace 
                }, new JsonSerializerOptions { WriteIndented = true });
                
                // Create error entries for each combination
                var errorResults = new List<SpotPlacementScoreResult>();
                foreach (var region in regions)
                {
                    foreach (var sku in vmSkus)
                    {
                        errorResults.Add(new SpotPlacementScoreResult
                        {
                            Region = region,
                            VmSku = sku,
                            AvailabilityZone = "N/A",
                            Score = "Error",
                            IsAvailable = false,
                            Message = ex.Message
                        });
                    }
                }

                return (errorResults, requestJson, responseJson);
            }
        }

        /// <summary>
        /// Generate user-friendly message based on score
        /// </summary>
        private string GetScoreMessage(string score, bool isAvailable)
        {
            if (!isAvailable)
                return "Quota not available";

            return score?.ToLower() switch
            {
                "0" or "verylow" => "Very Low - High eviction risk",
                "1" or "low" => "Low - Moderate eviction risk",
                "2" or "medium" => "Medium - Some eviction risk",
                "3" or "high" => "High - Low eviction risk",
                "4" or "veryhigh" => "Very High - Minimal eviction risk",
                _ => "Score not available"
            };
        }
    }
}
