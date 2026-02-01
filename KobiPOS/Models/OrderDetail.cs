namespace KobiPOS.Models
{
    public class OrderDetail
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string Notes { get; set; } = string.Empty;
        
        // Yeni alanlar: Zaman takibi için
        public DateTime AddedTime { get; set; } = DateTime.Now;
        public string AddedBy { get; set; } = string.Empty;
        
        // UI için computed properties
        public string FormattedTime => AddedTime.ToString("HH:mm");
        public string DetailedTime => AddedTime.ToString("HH:mm:ss");
        public string TimeAndUser => $"{DetailedTime} - {AddedBy}";
    }
}
