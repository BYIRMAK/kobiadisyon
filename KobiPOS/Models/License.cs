namespace KobiPOS.Models
{
    public class License
    {
        public int ID { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string HardwareID { get; set; } = string.Empty;
        public DateTime ActivationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
