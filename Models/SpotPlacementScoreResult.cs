namespace SpotPlacementScoreWebsite.Models
{
    public class SpotPlacementScoreResult
    {
        public string Region { get; set; } = string.Empty;
        public string VmSku { get; set; } = string.Empty;
        public string AvailabilityZone { get; set; } = string.Empty;
        public string Score { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
