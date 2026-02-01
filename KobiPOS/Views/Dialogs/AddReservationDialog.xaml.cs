using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KobiPOS.Models;
using KobiPOS.Services;

namespace KobiPOS.Views.Dialogs
{
    public partial class AddReservationDialog : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly User _currentUser;
        
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public int GuestCount { get; set; } = 4;
        public DateTime ReservationDate { get; set; } = DateTime.Today;
        public string ReservationTimeString { get; set; } = "19:00:00";
        public string? Notes { get; set; }
        public List<Table> AvailableTables { get; set; } = new();
        public Table? SelectedTable { get; set; }
        
        public bool Success { get; private set; } = false;
        
        public AddReservationDialog(User currentUser)
        {
            InitializeComponent();
            _databaseService = DatabaseService.Instance;
            _currentUser = currentUser;
            
            LoadAvailableTables();
            DataContext = this;
        }
        
        private void LoadAvailableTables()
        {
            var tables = _databaseService.GetAllTables();
            var zones = _databaseService.GetAllZones();
            
            // Bölge bilgisini ekleyerek DisplayName oluştur
            foreach (var table in tables)
            {
                var zone = zones.FirstOrDefault(z => z.ID == table.ZoneID);
                var zoneName = zone?.ZoneName ?? "Tümü";
                table.DisplayName = $"{zoneName} - {table.TableName} (Kap: {table.Capacity})";
                AvailableTables.Add(table);
            }
            
            if (AvailableTables.Any())
                SelectedTable = AvailableTables.First();
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                MessageBox.Show("Lütfen müşteri adını girin!", "Uyarı", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CustomerNameTextBox.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(CustomerPhone))
            {
                MessageBox.Show("Lütfen telefon numarasını girin!", "Uyarı", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CustomerPhoneTextBox.Focus();
                return;
            }
            
            if (SelectedTable == null)
            {
                MessageBox.Show("Lütfen masa seçin!", "Uyarı", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TableComboBox.Focus();
                return;
            }
            
            if (ReservationDate < DateTime.Today)
            {
                MessageBox.Show("Geçmiş tarih için rezervasyon oluşturamazsınız!", "Uyarı", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ReservationDatePicker.Focus();
                return;
            }
            
            try
            {
                // SAAT PARSE - GÜVENLİ YOL
                string timeString = "19:00:00"; // Default saat
                
                var selectedTimeItem = ReservationTimeComboBox.SelectedItem as ComboBoxItem;
                if (selectedTimeItem != null && selectedTimeItem.Tag != null)
                {
                    timeString = selectedTimeItem.Tag.ToString();
                }
                
                // KİŞİ SAYISI PARSE - GÜVENLİ YOL
                int guestCount = 4; // Default kişi sayısı
                
                var selectedGuestItem = GuestCountComboBox.SelectedItem as ComboBoxItem;
                if (selectedGuestItem != null && selectedGuestItem.Content != null)
                {
                    string contentValue = selectedGuestItem.Content.ToString();
                    if (int.TryParse(contentValue, out int parsed))
                    {
                        guestCount = parsed;
                    }
                }
                
                // TIMESPAN PARSE - GÜVENLİ YOL
                TimeSpan reservationTime = TimeSpan.FromHours(19); // Default 19:00
                if (!TimeSpan.TryParse(timeString, out reservationTime))
                {
                    // Eğer parse başarısız olursa default 19:00 kullan
                    reservationTime = TimeSpan.FromHours(19);
                }
                
                // REZERVASYON NESNESİ OLUŞTUR
                var reservation = new Reservation
                {
                    CustomerName = CustomerName.Trim(),
                    CustomerPhone = CustomerPhone.Trim(),
                    CustomerEmail = string.IsNullOrWhiteSpace(CustomerEmail) ? null : CustomerEmail.Trim(),
                    GuestCount = guestCount,
                    ReservationDate = ReservationDate,
                    ReservationTime = reservationTime,
                    TableID = SelectedTable.ID,
                    Status = "Confirmed",
                    Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                    CreatedBy = _currentUser.ID
                };
                
                // VERİTABANINA KAYDET
                _databaseService.AddReservation(reservation);
                
                Success = true;
                
                MessageBox.Show(
                    $"Rezervasyon başarıyla oluşturuldu!\n\nMüşteri: {reservation.CustomerName}\nTarih: {reservation.FormattedDate}\nSaat: {reservation.FormattedTime}\nMasa: {SelectedTable.TableName}", 
                    "Başarılı", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                
                DialogResult = true;
                Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(
                    $"Veri formatı hatası!\n\nLütfen tüm alanları doğru doldurduğunuzdan emin olun.\n\nDetay: {ex.Message}", 
                    "Format Hatası", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Rezervasyon oluşturulurken beklenmeyen bir hata oluştu!\n\nHata: {ex.Message}\n\nİç Hata: {ex.InnerException?.Message}", 
                    "Hata", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}