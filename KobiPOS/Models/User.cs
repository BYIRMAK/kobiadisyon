namespace KobiPOS.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // SHA256 hash
        public string Role { get; set; } = string.Empty; // Admin, Kasiyer, Garson
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
