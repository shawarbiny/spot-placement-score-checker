using SpotPlacementScoreWebsite.Models;

namespace SpotPlacementScoreWebsite.Services
{
    public interface IAzureResourceService
    {
        Task<List<SubscriptionInfo>> GetSubscriptionsAsync(string accessToken);
        Task<List<string>> GetInstanceSeriesAsync(string subscriptionId, string accessToken);
        Task<List<VmSkuInfo>> GetVmSkusAsync(string subscriptionId, string? series, string accessToken);
        Task<List<RegionInfo>> GetRegionsAsync(string subscriptionId, string accessToken);
    }
}
