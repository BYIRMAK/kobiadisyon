namespace KobiPOS.Models
{
    public class Table
    {
        public int ID { get; set; }
        public int TableNumber { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Status { get; set; } = "Boş"; // Boş, Dolu, Rezerve
        public int Capacity { get; set; }
    }
}
