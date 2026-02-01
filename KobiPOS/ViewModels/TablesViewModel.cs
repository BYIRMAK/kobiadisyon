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
        private ObservableCollection<Table> _tables;
        private Table? _selectedTable;

        public ObservableCollection<Table> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public Table? SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public ICommand OpenTableCommand { get; }
        public ICommand CloseTableCommand { get; }
        public ICommand RefreshCommand { get; }

        public TablesViewModel(User currentUser)
        {
            _currentUser = currentUser;
            _db = DatabaseService.Instance;
            _tables = new ObservableCollection<Table>();

            OpenTableCommand = new RelayCommand(ExecuteOpenTable, CanExecuteTableCommand);
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
                Tables.Add(table);
            }
        }

        private bool CanExecuteTableCommand(object? parameter)
        {
            return parameter is Table table && table.Status == "Boş";
        }

        private bool CanExecuteCloseTable(object? parameter)
        {
            return parameter is Table table && table.Status == "Dolu";
        }

        private void ExecuteOpenTable(object? parameter)
        {
            if (parameter is Table table)
            {
                _db.UpdateTableStatus(table.ID, "Dolu");
                LoadTables();
                MessageBox.Show($"{table.TableName} açıldı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExecuteCloseTable(object? parameter)
        {
            if (parameter is Table table)
            {
                var result = MessageBox.Show(
                    $"{table.TableName} kapatılsın mı?",
                    "Onay",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _db.UpdateTableStatus(table.ID, "Boş");
                    LoadTables();
                    MessageBox.Show($"{table.TableName} kapatıldı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
