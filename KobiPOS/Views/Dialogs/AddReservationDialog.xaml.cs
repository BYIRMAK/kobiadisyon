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
                return;
            }
            
            if (ReservationDate < DateTime.Today)
            {
                MessageBox.Show("Geçmiş tarih için rezervasyon oluşturamazsınız!", "Uyarı", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                // SAAT PARSE (GÜVENLİ)
                string timeString = "19:00:00"; // Default
                if (ReservationTimeComboBox.SelectedItem is ComboBoxItem selectedTimeItem)
                {
                    timeString = selectedTimeItem.Tag?.ToString() ?? "19:00:00";
                }
                
                // KİŞİ SAYISI PARSE (GÜVENLİ)
                int guestCount = 4; // Default
                if (GuestCountComboBox.SelectedItem is ComboBoxItem selectedGuestItem)
                {
                    var contentStr = selectedGuestItem.Content?.ToString();
                    if (!string.IsNullOrWhiteSpace(contentStr) && int.TryParse(contentStr, out int parsedCount))
                    {
                        guestCount = parsedCount;
                    }
                }
                
                var reservation = new Reservation
                {
                    CustomerName = CustomerName.Trim(),
                    CustomerPhone = CustomerPhone.Trim(),
                    CustomerEmail = string.IsNullOrWhiteSpace(CustomerEmail) ? null : CustomerEmail.Trim(),
                    GuestCount = guestCount,
                    ReservationDate = ReservationDate,
                    ReservationTime = TimeSpan.Parse(timeString),
                    TableID = SelectedTable.ID,
                    Status = "Confirmed",
                    Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                    CreatedBy = _currentUser.ID
                };
                
                _databaseService.AddReservation(reservation);
                
                Success = true;
                MessageBox.Show("Rezervasyon başarıyla oluşturuldu!", "Başarılı", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rezervasyon oluşturulurken hata:\n\n{ex.Message}\n\nDetay: {ex.StackTrace}", "Hata", 
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
