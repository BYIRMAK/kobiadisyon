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
        
        // Non-cached property for table count to ensure fresh data
        public int TableCount
        {
            get
            {
                var db = DatabaseService.Instance;
                // Special handling for "All" zone (ID = 0)
                if (this.ID == 0)
                {
                    return db.GetAllTables().Count;
                }
                return db.GetTablesByZone(this.ID).Count;
            }
        }
    }
}
