namespace KobiPOS.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
