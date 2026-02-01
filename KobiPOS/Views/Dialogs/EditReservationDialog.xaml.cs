using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KobiPOS.Models;
using KobiPOS.Services;

namespace KobiPOS.Views.Dialogs
{
    public partial class EditReservationDialog : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly Reservation _reservation;
        
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public int GuestCount { get; set; }
        public DateTime ReservationDate { get; set; }
        public int TableID { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<Table> AvailableTables { get; set; } = new();
        
        public bool Success { get; private set; } = false;
        
        public EditReservationDialog(Reservation reservation)
        {
            InitializeComponent();
            _databaseService = DatabaseService.Instance;
            _reservation = reservation;
            
            LoadAvailableTables();
            LoadReservationData();
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
        }
        
        private void LoadReservationData()
        {
            CustomerName = _reservation.CustomerName;
            CustomerPhone = _reservation.CustomerPhone;
            CustomerEmail = _reservation.CustomerEmail;
            GuestCount = _reservation.GuestCount;
            ReservationDate = _reservation.ReservationDate;
            TableID = _reservation.TableID;
            Status = _reservation.Status;
            Notes = _reservation.Notes;
            
            // Kişi sayısı ComboBox seçimi
            foreach (ComboBoxItem item in GuestCountComboBox.Items)
            {
                if (item.Content.ToString() == GuestCount.ToString())
                {
                    GuestCountComboBox.SelectedItem = item;
                    break;
                }
            }
            
            // Saat ComboBox seçimi
            var timeString = _reservation.ReservationTime.ToString(@"HH\:mm");
            foreach (ComboBoxItem item in ReservationTimeComboBox.Items)
            {
                if (item.Content.ToString() == timeString)
                {
                    ReservationTimeComboBox.SelectedItem = item;
                    break;
                }
            }
            
            // Durum ComboBox seçimi
            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if (item.Tag.ToString() == Status)
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }
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
            
            if (TableID == 0)
            {
                MessageBox.Show("Lütfen masa seçin!", "Uyarı", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TableComboBox.Focus();
                return;
            }
            
            try
            {
                // SAAT PARSE - GÜVENLİ YOL
                string timeString = "19:00:00"; // Default
                
                var selectedTimeItem = ReservationTimeComboBox.SelectedItem as ComboBoxItem;
                if (selectedTimeItem != null && selectedTimeItem.Tag != null)
                {
                    timeString = selectedTimeItem.Tag.ToString();
                }
                
                // KİŞİ SAYISI PARSE - GÜVENLİ YOL
                int guestCount = 4; // Default
                
                var selectedGuestItem = GuestCountComboBox.SelectedItem as ComboBoxItem;
                if (selectedGuestItem != null && selectedGuestItem.Content != null)
                {
                    string contentValue = selectedGuestItem.Content.ToString();
                    if (int.TryParse(contentValue, out int parsed))
                    {
                        guestCount = parsed;
                    }
                }
                
                // DURUM PARSE - GÜVENLİ YOL
                string status = "Confirmed"; // Default
                
                var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;
                if (selectedStatusItem != null && selectedStatusItem.Tag != null)
                {
                    status = selectedStatusItem.Tag.ToString();
                }
                
                // REZERVASYON GÜNCELLE
                _reservation.CustomerName = CustomerName.Trim();
                _reservation.CustomerPhone = CustomerPhone.Trim();
                _reservation.CustomerEmail = string.IsNullOrWhiteSpace(CustomerEmail) ? null : CustomerEmail.Trim();
                _reservation.GuestCount = guestCount;
                _reservation.ReservationDate = ReservationDate;
                _reservation.ReservationTime = TimeSpan.Parse(timeString);
                _reservation.TableID = TableID;
                _reservation.Status = status;
                _reservation.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
                
                // VERİTABANINA KAYDET
                _databaseService.UpdateReservation(_reservation);
                
                Success = true;
                
                MessageBox.Show(
                    $"Rezervasyon başarıyla güncellendi!\n\nMüşteri: {_reservation.CustomerName}\nDurum: {_reservation.StatusText}", 
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
                    $"Rezervasyon güncellenirken beklenmeyen bir hata oluştu!\n\nHata: {ex.Message}\n\nİç Hata: {ex.InnerException?.Message}", 
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
