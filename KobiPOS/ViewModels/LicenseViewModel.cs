using KobiPOS.Helpers;
using KobiPOS.Services;
using System.Windows;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class LicenseViewModel : ViewModelBase
    {
        private readonly LicenseService _licenseService;
        private string _hardwareId = string.Empty;
        private string _licenseKey = string.Empty;
        private string _customerName = string.Empty;
        private string _licenseStatus = string.Empty;
        private string _statusMessage = string.Empty;

        public string HardwareId
        {
            get => _hardwareId;
            set => SetProperty(ref _hardwareId, value);
        }

        public string LicenseKey
        {
            get => _licenseKey;
            set => SetProperty(ref _licenseKey, value);
        }

        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }

        public string LicenseStatus
        {
            get => _licenseStatus;
            set => SetProperty(ref _licenseStatus, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand ActivateCommand { get; }
        public ICommand CopyHardwareIdCommand { get; }

        public LicenseViewModel()
        {
            _licenseService = new LicenseService();
            ActivateCommand = new RelayCommand(_ => ActivateLicense(), _ => CanActivate());
            CopyHardwareIdCommand = new RelayCommand(_ => CopyHardwareId());

            LoadLicenseInfo();
        }

        private void LoadLicenseInfo()
        {
            HardwareId = HardwareService.GetHardwareID();
            var status = _licenseService.GetLicenseStatus();

            if (status.IsLicensed)
            {
                LicenseStatus = $"✓ Lisanslı - {status.LicenseType}\n";
                LicenseStatus += $"Müşteri: {status.CustomerName}\n";
                LicenseStatus += $"Geçerlilik: {status.ExpiryDate:dd.MM.yyyy}\n";
                LicenseStatus += $"Kalan Gün: {status.DaysRemaining}";
            }
            else
            {
                LicenseStatus = $"⚠ {status.LicenseType}\n";
                if (status.DaysRemaining > 0)
                {
                    LicenseStatus += $"Kalan Gün: {status.DaysRemaining}";
                }
                else
                {
                    LicenseStatus += "Lisans aktivasyonu gerekiyor!";
                }
            }
        }

        private bool CanActivate()
        {
            return !string.IsNullOrWhiteSpace(LicenseKey) && !string.IsNullOrWhiteSpace(CustomerName);
        }

        private void ActivateLicense()
        {
            var success = _licenseService.ActivateLicense(LicenseKey, CustomerName);

            if (success)
            {
                StatusMessage = "✓ Lisans başarıyla aktive edildi!";
                LoadLicenseInfo();
                LicenseKey = string.Empty;
                CustomerName = string.Empty;

                MessageBox.Show(
                    "Lisans başarıyla aktive edildi!\nUygulamayı yeniden başlatın.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                StatusMessage = "✗ Lisans aktivasyonu başarısız! Lütfen lisans anahtarını kontrol edin.";
                MessageBox.Show(
                    "Lisans aktivasyonu başarısız!\nLisans anahtarını kontrol edin veya Kobi Bilişim ile iletişime geçin.",
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CopyHardwareId()
        {
            Clipboard.SetText(HardwareId);
            StatusMessage = "Hardware ID panoya kopyalandı!";
        }
    }
}
