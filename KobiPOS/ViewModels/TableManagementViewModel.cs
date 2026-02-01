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
    public class TableManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Table> _tables;
        private ObservableCollection<Zone> _zones;
        private Table? _selectedTable;
        private bool _isEditMode;
        private Table _editingTable;

        public TableManagementViewModel()
        {
            _databaseService = DatabaseService.Instance;
            _tables = new ObservableCollection<Table>();
            _zones = new ObservableCollection<Zone>();
            _editingTable = new Table();

            AddTableCommand = new RelayCommand(ExecuteAddTable);
            EditTableCommand = new RelayCommand(ExecuteEditTable, CanEditOrDelete);
            DeleteTableCommand = new RelayCommand(ExecuteDeleteTable, CanEditOrDelete);
            SaveTableCommand = new RelayCommand(ExecuteSaveTable, CanSaveTable);
            CancelEditCommand = new RelayCommand(ExecuteCancelEdit);

            LoadZones();
            LoadTables();
        }

        public ObservableCollection<Table> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public ObservableCollection<Zone> Zones
        {
            get => _zones;
            set => SetProperty(ref _zones, value);
        }

        public Table? SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (SetProperty(ref _selectedTable, value))
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

        public Table EditingTable
        {
            get => _editingTable;
            set => SetProperty(ref _editingTable, value);
        }

        public ICommand AddTableCommand { get; }
        public ICommand EditTableCommand { get; }
        public ICommand DeleteTableCommand { get; }
        public ICommand SaveTableCommand { get; }
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

        private void LoadTables()
        {
            try
            {
                var tables = _databaseService.GetAllTables();
                Tables.Clear();
                foreach (var table in tables)
                {
                    Tables.Add(table);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Masalar yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddTable(object? parameter)
        {
            EditingTable = new Table
            {
                IsActive = true,
                Status = "Boş",
                Capacity = 4
            };
            IsEditMode = true;
        }

        private void ExecuteEditTable(object? parameter)
        {
            if (SelectedTable == null) return;

            EditingTable = new Table
            {
                ID = SelectedTable.ID,
                TableNumber = SelectedTable.TableNumber,
                TableName = SelectedTable.TableName,
                Status = SelectedTable.Status,
                Capacity = SelectedTable.Capacity,
                ZoneID = SelectedTable.ZoneID,
                IsActive = SelectedTable.IsActive
            };
            IsEditMode = true;
        }

        private void ExecuteDeleteTable(object? parameter)
        {
            if (SelectedTable == null) return;

            var result = MessageBox.Show(
                $"'{SelectedTable.TableName}' masasını silmek istediğinizden emin misiniz?",
                "Masa Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _databaseService.DeleteTable(SelectedTable.ID);
                LoadTables();
                MessageBox.Show("Masa başarıyla silindi.", "Başarılı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Masa silinirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSaveTable(object? parameter)
        {
            if (!ValidateTable()) return;

            try
            {
                if (EditingTable.ID == 0)
                {
                    _databaseService.AddTable(EditingTable);
                    MessageBox.Show("Masa başarıyla eklendi.", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _databaseService.UpdateTable(EditingTable);
                    MessageBox.Show("Masa başarıyla güncellendi.", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                IsEditMode = false;
                LoadTables();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Masa kaydedilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancelEdit(object? parameter)
        {
            IsEditMode = false;
            EditingTable = new Table();
        }

        private bool CanEditOrDelete(object? parameter)
        {
            return SelectedTable != null;
        }

        private bool CanSaveTable(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(EditingTable?.TableName) &&
                   EditingTable?.TableNumber > 0 &&
                   EditingTable?.Capacity > 0;
        }

        private bool ValidateTable()
        {
            if (string.IsNullOrWhiteSpace(EditingTable.TableName))
            {
                MessageBox.Show("Masa adı boş olamaz.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditingTable.TableNumber <= 0)
            {
                MessageBox.Show("Masa numarası 0'dan büyük olmalıdır.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditingTable.Capacity <= 0)
            {
                MessageBox.Show("Kapasite 0'dan büyük olmalıdır.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
