using KobiPOS.Services;

namespace KobiPOS.Models
{
    public class Zone
    {
        public int ID { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string ColorCode { get; set; } = "#2196F3";
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        public int TableCount
        {
            get
            {
                var db = DatabaseService.Instance;
                return db.GetTablesByZone(this.ID).Count;
            }
        }
    }
}
