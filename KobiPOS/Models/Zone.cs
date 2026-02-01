using KobiPOS.Services;

namespace KobiPOS.Models
{
    public class Zone
    {
        private int? _tableCount;
        
        public int ID { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string ColorCode { get; set; } = "#2196F3";
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        public int TableCount
        {
            get
            {
                if (_tableCount == null)
                {
                    var db = DatabaseService.Instance;
                    _tableCount = db.GetTablesByZone(this.ID).Count;
                }
                return _tableCount.Value;
            }
        }
    }
}
