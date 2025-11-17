namespace SpotPlacementScoreWebsite.Models
{
    public class VmSkuInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int VCpus { get; set; }
        public int MemoryGB { get; set; }
    }
}
