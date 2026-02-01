using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private User _currentUser;
        private object? _currentView;
        private string _title = "KobiPOS - Cafe & Restoran Yönetim Sistemi";
        private readonly LicenseService _licenseService;

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ICommand ShowTablesCommand { get; }
        public ICommand ShowProductsCommand { get; }
        public ICommand ShowReportsCommand { get; }
        public ICommand ShowLicenseCommand { get; }
        public ICommand ShowSupportCommand { get; }
        public ICommand LogoutCommand { get; }

        public event EventHandler? LogoutRequested;

        public MainViewModel(User user)
        {
            _currentUser = user;
            _licenseService = new LicenseService();

            ShowTablesCommand = new RelayCommand(_ => ShowTables());
            ShowProductsCommand = new RelayCommand(_ => ShowProducts());
            ShowReportsCommand = new RelayCommand(_ => ShowReports());
            ShowLicenseCommand = new RelayCommand(_ => ShowLicense());
            ShowSupportCommand = new RelayCommand(_ => ShowSupport());
            LogoutCommand = new RelayCommand(_ => Logout());

            // Check license status
            CheckLicenseStatus();

            // Show tables view by default
            ShowTables();
        }

        private void CheckLicenseStatus()
        {
            var status = _licenseService.GetLicenseStatus();
            if (!status.IsLicensed && status.DaysRemaining > 0)
            {
                Title = $"KobiPOS - Deneme Sürümü ({status.DaysRemaining} gün kaldı)";
            }
            else if (status.IsReadOnly)
            {
                Title = "KobiPOS - Lisans Gerekli (Sadece Okuma Modu)";
            }
        }

        private void ShowTables()
        {
            CurrentView = new TablesViewModel(CurrentUser);
        }

        private void ShowProducts()
        {
            CurrentView = new ProductViewModel(CurrentUser);
        }

        private void ShowReports()
        {
            CurrentView = new ReportViewModel();
        }

        private void ShowLicense()
        {
            CurrentView = new LicenseViewModel();
        }

        private void ShowSupport()
        {
            CurrentView = new SupportViewModel();
        }

        private void Logout()
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
