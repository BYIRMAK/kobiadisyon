namespace KobiPOS.Models
{
    public class Table
    {
        public int ID { get; set; }
        public int TableNumber { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Status { get; set; } = "Boş"; // Boş, Dolu, Rezerve
        public int Capacity { get; set; }
        public int? ZoneID { get; set; }
        public bool IsActive { get; set; } = true;
        
        // YENİ: Rezervasyon bilgisi
        public Reservation? CurrentReservation { get; set; }
        
        // Masa rengi (güncellenmiş - Rezerve durumu eklendi)
        public string StatusColor => Status switch
        {
            "Boş" => "#4CAF50",        // Yeşil
            "Dolu" => "#F44336",       // Kırmızı
            "Rezerve" => "#FFC107",    // Sarı (AMBER)
            _ => "#9E9E9E"             // Gri
        };
        
        // Masa ikonu (güncellenmiş)
        public string StatusIcon => Status switch
        {
            "Boş" => "✓",
            "Dolu" => "●",
            "Rezerve" => "📅",
            _ => "?"
        };
        
        // Masa kartında gösterilecek bilgi (güncellenmiş)
        public string DisplayInfo
        {
            get
            {
                if (Status == "Rezerve" && CurrentReservation != null)
                {
                    // Rezerve masalar için özel görünüm
                    return $"REZERVE\n{CurrentReservation.FormattedTime}\n{CurrentReservation.CustomerName}";
                }
                else if (Status == "Dolu")
                {
                    return "Dolu";
                }
                else
                {
                    return "Boş";
                }
            }
        }
        
        // Kapasite bilgisi
        public string CapacityText => $"Kap: {Capacity} kişi";
        
        // Rezerve mi?
        public bool IsReserved => Status == "Rezerve";
        
        // Boş mu?
        public bool IsAvailable => Status == "Boş";
        
        // Dolu mu?
        public bool IsOccupied => Status == "Dolu";
    }
}
