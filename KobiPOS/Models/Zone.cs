namespace KobiPOS.Models
{
    public class Zone
    {
        public int ID { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string ColorCode { get; set; } = "#2196F3";
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
