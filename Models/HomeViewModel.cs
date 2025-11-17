namespace SpotPlacementScoreWebsite.Models
{
    public class HomeViewModel
    {
        public List<SubscriptionInfo> Subscriptions { get; set; } = new();
        public List<string> InstanceSeries { get; set; } = new();
        public List<VmSkuInfo> VmSkus { get; set; } = new();
        public List<RegionInfo> Regions { get; set; } = new();
        public SpotPlacementRequest? Request { get; set; }
        public List<SpotPlacementScoreResult>? Results { get; set; }
        public string? RawApiRequest { get; set; }
        public string? RawApiResponse { get; set; }
    }
}
