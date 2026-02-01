using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class TablesViewModel : ViewModelBase
    {
        private readonly DatabaseService _db;
        private readonly User _currentUser;
        private ObservableCollection<TableDisplayModel> _tables;
        private ObservableCollection<TableDisplayModel> _filteredTables;
        private ObservableCollection<Zone> _zones;
        private TableDisplayModel? _selectedTable;
        private Zone? _selectedZone;
        private List<TableDisplayModel> _allTables;

        public ObservableCollection<TableDisplayModel> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public ObservableCollection<TableDisplayModel> FilteredTables
        {
            get => _filteredTables;
            set => SetProperty(ref _filteredTables, value);
        }

        public ObservableCollection<Zone> Zones
        {
            get => _zones;
            set => SetProperty(ref _zones, value);
        }

        public TableDisplayModel? SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public Zone? SelectedZone
        {
            get => _selectedZone;
            set
            {
                if (SetProperty(ref _selectedZone, value))
                {
                    FilterTablesByZone();
                }
            }
        }

        public ICommand OpenTableCommand { get; }
        public ICommand CloseTableCommand { get; }
        public ICommand RefreshCommand { get; }

        public event EventHandler<Table>? TableSelected;

        public TablesViewModel(User currentUser)
        {
            _currentUser = currentUser;
            _db = DatabaseService.Instance;
            _tables = new ObservableCollection<TableDisplayModel>();
            _filteredTables = new ObservableCollection<TableDisplayModel>();
            _zones = new ObservableCollection<Zone>();
            _allTables = new List<TableDisplayModel>();

            OpenTableCommand = new RelayCommand(ExecuteOpenTable);
            CloseTableCommand = new RelayCommand(ExecuteCloseTable, CanExecuteCloseTable);
            RefreshCommand = new RelayCommand(_ => LoadTables());

            LoadZones();
            LoadTables();
        }

        private void LoadZones()
        {
            var zones = _db.GetAllZones();
            Zones.Clear();
            
            // Add "Tümü" (All) tab
            Zones.Add(new Zone
            {
                ID = 0,
                ZoneName = "Tümü",
                ColorCode = "#808080"
            });
            
            foreach (var zone in zones)
            {
                Zones.Add(zone);
            }
            
            SelectedZone = Zones.FirstOrDefault();
        }

        private void LoadTables()
        {
            var tables = _db.GetAllTables();
            var today = DateTime.Today;
            var now = DateTime.Now;
            
            Tables.Clear();
            _allTables.Clear();
            
            foreach (var table in tables)
            {
                // Masanın bugünkü rezervasyonlarını kontrol et
                var reservations = _db.GetTableReservations(table.ID, today);
                
                // Aktif rezervasyon var mı? (30 dakika öncesi - 30 dakika sonrası)
                var activeReservation = reservations.FirstOrDefault(r => 
                    r.ReservationDateTime >= now.AddMinutes(-30) && 
                    r.ReservationDateTime <= now.AddMinutes(30) &&
                    r.IsActive);
                
                // Eğer masa dolu değilse ve aktif rezervasyon varsa
                if (activeReservation != null && table.Status != TableStatus.Occupied)
                {
                    table.Status = TableStatus.Reserved;
                    table.CurrentReservation = activeReservation;
                }
                
                var orderTotal = _db.GetTableOrderTotal(table.ID);
                var displayModel = new TableDisplayModel
                {
                    Table = table,
                    OrderTotal = orderTotal
                };
                Tables.Add(displayModel);
                _allTables.Add(displayModel);
            }
            
            FilterTablesByZone();
        }

        private void FilterTablesByZone()
        {
            FilteredTables.Clear();
            
            if (SelectedZone == null || SelectedZone.ID == 0)
            {
                // Show all tables
                foreach (var table in _allTables)
                {
                    FilteredTables.Add(table);
                }
            }
            else
            {
                // Show only tables in selected zone
                foreach (var table in _allTables.Where(t => t.Table.ZoneID == SelectedZone.ID))
                {
                    FilteredTables.Add(table);
                }
            }
        }

        private bool CanExecuteCloseTable(object? parameter)
        {
            return parameter is TableDisplayModel model && model.Table.Status == TableStatus.Occupied;
        }

        private void ExecuteOpenTable(object? parameter)
        {
            if (parameter is TableDisplayModel model)
            {
                TableSelected?.Invoke(this, model.Table);
            }
        }

        private void ExecuteCloseTable(object? parameter)
        {
            if (parameter is TableDisplayModel model)
            {
                var result = MessageBox.Show(
                    $"{model.Table.TableName} kapatılsın mı?",
                    "Onay",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _db.UpdateTableStatus(model.Table.ID, TableStatus.Empty);
                    LoadTables();
                    MessageBox.Show($"{model.Table.TableName} kapatıldı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public void Refresh()
        {
            LoadTables();
        }
    }

    public class TableDisplayModel
    {
        public Table Table { get; set; } = new Table();
        public decimal OrderTotal { get; set; }
        public string DisplayTotal => OrderTotal > 0 ? $"{OrderTotal:N2} ₺" : "";
    }
}
