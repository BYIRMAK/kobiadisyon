using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class ZoneManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Zone> _zones;
        private Zone? _selectedZone;
        private bool _isEditMode;
        private Zone _editingZone;

        public ZoneManagementViewModel()
        {
            _databaseService = DatabaseService.Instance;
            _zones = new ObservableCollection<Zone>();
            _editingZone = new Zone();

            AddZoneCommand = new RelayCommand(ExecuteAddZone);
            EditZoneCommand = new RelayCommand(ExecuteEditZone, CanEditOrDelete);
            DeleteZoneCommand = new RelayCommand(ExecuteDeleteZone, CanEditOrDelete);
            SaveZoneCommand = new RelayCommand(ExecuteSaveZone, CanSaveZone);
            CancelEditCommand = new RelayCommand(ExecuteCancelEdit);

            LoadZones();
        }

        public ObservableCollection<Zone> Zones
        {
            get => _zones;
            set => SetProperty(ref _zones, value);
        }

        public Zone? SelectedZone
        {
            get => _selectedZone;
            set
            {
                if (SetProperty(ref _selectedZone, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public Zone EditingZone
        {
            get => _editingZone;
            set => SetProperty(ref _editingZone, value);
        }

        public ICommand AddZoneCommand { get; }
        public ICommand EditZoneCommand { get; }
        public ICommand DeleteZoneCommand { get; }
        public ICommand SaveZoneCommand { get; }
        public ICommand CancelEditCommand { get; }

        private void LoadZones()
        {
            try
            {
                var zones = _databaseService.GetAllZones();
                Zones.Clear();
                foreach (var zone in zones)
                {
                    Zones.Add(zone);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bölgeler yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddZone(object? parameter)
        {
            EditingZone = new Zone
            {
                ColorCode = "#2196F3",
                IsActive = true
            };
            IsEditMode = true;
        }

        private void ExecuteEditZone(object? parameter)
        {
            if (SelectedZone == null) return;

            EditingZone = new Zone
            {
                ID = SelectedZone.ID,
                ZoneName = SelectedZone.ZoneName,
                ColorCode = SelectedZone.ColorCode,
                Description = SelectedZone.Description,
                IsActive = SelectedZone.IsActive
            };
            IsEditMode = true;
        }

        private void ExecuteDeleteZone(object? parameter)
        {
            if (SelectedZone == null) return;

            var result = MessageBox.Show(
                $"'{SelectedZone.ZoneName}' bölgesini silmek istediğinizden emin misiniz?",
                "Bölge Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _databaseService.DeleteZone(SelectedZone.ID);
                LoadZones();
                MessageBox.Show("Bölge başarıyla silindi.", "Başarılı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bölge silinirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSaveZone(object? parameter)
        {
            if (!ValidateZone()) return;

            try
            {
                if (EditingZone.ID == 0)
                {
                    _databaseService.AddZone(EditingZone);
                    MessageBox.Show("Bölge başarıyla eklendi.", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _databaseService.UpdateZone(EditingZone);
                    MessageBox.Show("Bölge başarıyla güncellendi.", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                IsEditMode = false;
                LoadZones();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bölge kaydedilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancelEdit(object? parameter)
        {
            IsEditMode = false;
            EditingZone = new Zone();
        }

        private bool CanEditOrDelete(object? parameter)
        {
            return SelectedZone != null;
        }

        private bool CanSaveZone(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(EditingZone?.ZoneName);
        }

        private bool ValidateZone()
        {
            if (string.IsNullOrWhiteSpace(EditingZone.ZoneName))
            {
                MessageBox.Show("Bölge adı boş olamaz.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EditingZone.ColorCode))
            {
                EditingZone.ColorCode = "#2196F3";
            }

            return true;
        }
    }
}
