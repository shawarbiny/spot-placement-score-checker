namespace SpotPlacementScoreWebsite.Models
{
    public class SpotPlacementRequest
    {
        public string SubscriptionId { get; set; } = string.Empty;
        public List<string> SelectedSkus { get; set; } = new();
        public List<string> SelectedRegions { get; set; } = new();
        public bool UseAvailabilityZones { get; set; } = true;
        public int DesiredCount { get; set; } = 1;
    }
}
