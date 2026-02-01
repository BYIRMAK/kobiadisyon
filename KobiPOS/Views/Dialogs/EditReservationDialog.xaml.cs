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
            
            try
            {
                // Saat
                var selectedTimeItem = ReservationTimeComboBox.SelectedItem as ComboBoxItem;
                var timeString = selectedTimeItem?.Tag?.ToString() ?? "19:00:00";
                
                // Kişi sayısı
                var selectedGuestItem = GuestCountComboBox.SelectedItem as ComboBoxItem;
                var guestCount = int.Parse(selectedGuestItem?.Content?.ToString() ?? "4");
                
                // Durum
                var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;
                var status = selectedStatusItem?.Tag?.ToString() ?? "Confirmed";
                
                _reservation.CustomerName = CustomerName.Trim();
                _reservation.CustomerPhone = CustomerPhone.Trim();
                _reservation.CustomerEmail = string.IsNullOrWhiteSpace(CustomerEmail) ? null : CustomerEmail.Trim();
                _reservation.GuestCount = guestCount;
                _reservation.ReservationDate = ReservationDate;
                _reservation.ReservationTime = TimeSpan.Parse(timeString);
                _reservation.TableID = TableID;
                _reservation.Status = status;
                _reservation.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
                
                _databaseService.UpdateReservation(_reservation);
                
                Success = true;
                MessageBox.Show("Rezervasyon başarıyla güncellendi!", "Başarılı", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rezervasyon güncellenirken hata: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
