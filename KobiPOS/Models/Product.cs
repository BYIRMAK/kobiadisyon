namespace KobiPOS.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = "Adet";
        public bool StockTracking { get; set; }
        public int CurrentStock { get; set; }
        public bool IsActive { get; set; } = true;
        
        public bool HasImage => !string.IsNullOrEmpty(ImagePath);
    }
}
