using SpotPlacementScoreWebsite.Models;

namespace SpotPlacementScoreWebsite.Services
{
    public interface ISpotPlacementScoreService
    {
        Task<(List<SpotPlacementScoreResult> Results, string RequestJson, string ResponseJson)> CheckSpotPlacementScoreAsync(
            string subscriptionId,
            List<string> vmSkus,
            List<string> regions,
            bool useAvailabilityZones,
            int desiredCount,
            string accessToken);
    }
}
