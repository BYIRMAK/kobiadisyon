using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using KobiPOS.Views.Dialogs;

namespace KobiPOS.ViewModels
{
    public class ReservationViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly User _currentUser;
        private DateTime _filterDate = DateTime.Today;
        
        public ObservableCollection<Reservation> Reservations { get; set; } = new();
        
        public DateTime FilterDate
        {
            get => _filterDate;
            set
            {
                if (SetProperty(ref _filterDate, value))
                {
                    LoadReservations();
                }
            }
        }
        
        public ICommand AddReservationCommand { get; }
        public ICommand EditReservationCommand { get; }
        public ICommand CancelReservationCommand { get; }
        public ICommand RefreshCommand { get; }
        
        public ReservationViewModel(User currentUser)
        {
            _databaseService = DatabaseService.Instance;
            _currentUser = currentUser;
            
            AddReservationCommand = new RelayCommand(_ => AddReservation());
            EditReservationCommand = new RelayCommand(param => EditReservation(param as Reservation));
            CancelReservationCommand = new RelayCommand(param => CancelReservation(param as Reservation));
            RefreshCommand = new RelayCommand(_ => LoadReservations());
            
            LoadReservations();
        }
        
        private void LoadReservations()
        {
            // Get reservations for the selected date only (using same date for start and end to filter single day)
            var selectedDate = FilterDate.Date;
            var reservations = _databaseService.GetReservations(selectedDate, selectedDate);
            Reservations.Clear();
            foreach (var res in reservations)
            {
                Reservations.Add(res);
            }
        }
        
        private void AddReservation()
        {
            var dialog = new AddReservationDialog(_currentUser);
            var result = dialog.ShowDialog();
            
            if (result == true && dialog.Success)
            {
                LoadReservations();
            }
        }
        
        private void EditReservation(Reservation? reservation)
        {
            if (reservation == null) return;
            
            var dialog = new EditReservationDialog(reservation);
            var result = dialog.ShowDialog();
            
            if (result == true && dialog.Success)
            {
                LoadReservations();
            }
        }
        
        private void CancelReservation(Reservation? reservation)
        {
            if (reservation == null) return;
            
            var result = MessageBox.Show(
                $"{reservation.CustomerName} adlı rezervasyonu iptal etmek istediğinize emin misiniz?",
                "Rezervasyon İptali",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _databaseService.CancelReservation(reservation.ID);
                LoadReservations();
                MessageBox.Show("Rezervasyon iptal edildi.", "Başarılı", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
