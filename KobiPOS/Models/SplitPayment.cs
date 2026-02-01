namespace KobiPOS.Models
{
    public class SplitPayment
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int PersonNumber { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
