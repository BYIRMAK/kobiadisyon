using System;

namespace KobiPOS.Models
{
    public class Reservation
    {
        // Primary Properties
        public int ID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public int GuestCount { get; set; }
        public DateTime ReservationDate { get; set; } = DateTime.Today;
        public TimeSpan ReservationTime { get; set; } = new TimeSpan(19, 0, 0); // Default: 19:00
        public int TableID { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending;
        public string? Notes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation Properties (JOIN'den gelecek)
        public string TableName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        
        // Computed Properties
        public DateTime ReservationDateTime => ReservationDate.Date + ReservationTime;
        
        public string FormattedDate => ReservationDate.ToString("dd.MM.yyyy");
        
        public string FormattedTime => ReservationTime.ToString(@"HH\:mm");
        
        public string FormattedDateTime => $"{FormattedDate} {FormattedTime}";
        
        // Durum metinleri (Türkçe)
        public string StatusText => Status switch
        {
            ReservationStatus.Pending => "Bekliyor",
            ReservationStatus.Confirmed => "Onaylandı",
            ReservationStatus.Completed => "Tamamlandı",
            ReservationStatus.Cancelled => "İptal Edildi",
            ReservationStatus.NoShow => "Gelmedi",
            _ => Status
        };
        
        // Durum ikonları
        public string StatusIcon => Status switch
        {
            ReservationStatus.Pending => "⏳",
            ReservationStatus.Confirmed => "✅",
            ReservationStatus.Completed => "✔️",
            ReservationStatus.Cancelled => "❌",
            ReservationStatus.NoShow => "🚫",
            _ => "📅"
        };
        
        // Durum renkleri
        public string StatusColor => Status switch
        {
            ReservationStatus.Pending => "#FF9800",      // Turuncu
            ReservationStatus.Confirmed => "#4CAF50",    // Yeşil
            ReservationStatus.Completed => "#9E9E9E",    // Gri
            ReservationStatus.Cancelled => "#F44336",    // Kırmızı
            ReservationStatus.NoShow => "#795548",       // Kahverengi
            _ => "#2196F3"               // Mavi
        };
        
        // Rezervasyon ne kadar yakın?
        public TimeSpan TimeUntilReservation => ReservationDateTime - DateTime.Now;
        
        // Rezervasyon yakında mı? (60 dakika içinde)
        public bool IsUpcoming => TimeUntilReservation.TotalMinutes > 0 
                                && TimeUntilReservation.TotalMinutes <= 60;
        
        // Rezervasyon çok yakın mı? (15 dakika içinde) - Uyarı için
        public bool IsSoon => TimeUntilReservation.TotalMinutes > 0 
                            && TimeUntilReservation.TotalMinutes <= 15;
        
        // Rezervasyon geçti mi?
        public bool IsPast => TimeUntilReservation.TotalMinutes < 0;
        
        // Rezervasyon aktif mi? (Pending veya Confirmed)
        public bool IsActive => Status == ReservationStatus.Pending || Status == ReservationStatus.Confirmed;
        
        // Görünüm için özet bilgi
        public string DisplayInfo => $"{CustomerName} - {GuestCount} kişi - {FormattedTime}";
        
        // Uyarı mesajı için
        public string AlertMessage
        {
            get
            {
                var minutes = (int)TimeUntilReservation.TotalMinutes;
                if (minutes <= 0)
                    return "Rezervasyon zamanı geldi!";
                else if (minutes == 1)
                    return "1 dakika sonra rezervasyon!";
                else if (minutes <= 15)
                    return $"{minutes} dakika sonra rezervasyon!";
                else
                    return $"{minutes} dakika sonra rezervasyon";
            }
        }
    }
}
