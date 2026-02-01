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
        private TableDisplayModel? _selectedTable;

        public ObservableCollection<TableDisplayModel> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public TableDisplayModel? SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
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

            OpenTableCommand = new RelayCommand(ExecuteOpenTable);
            CloseTableCommand = new RelayCommand(ExecuteCloseTable, CanExecuteCloseTable);
            RefreshCommand = new RelayCommand(_ => LoadTables());

            LoadTables();
        }

        private void LoadTables()
        {
            var tables = _db.GetAllTables();
            Tables.Clear();
            foreach (var table in tables)
            {
                var orderTotal = _db.GetTableOrderTotal(table.ID);
                Tables.Add(new TableDisplayModel
                {
                    Table = table,
                    OrderTotal = orderTotal
                });
            }
        }

        private bool CanExecuteCloseTable(object? parameter)
        {
            return parameter is TableDisplayModel model && model.Table.Status == "Dolu";
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
                    _db.UpdateTableStatus(model.Table.ID, "Boş");
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
