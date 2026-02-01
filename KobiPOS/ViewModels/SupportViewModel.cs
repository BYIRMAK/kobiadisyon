using KobiPOS.Helpers;
using System.Diagnostics;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class SupportViewModel : ViewModelBase
    {
        private string _companyInfo = string.Empty;

        public string CompanyInfo
        {
            get => _companyInfo;
            set => SetProperty(ref _companyInfo, value);
        }

        public ICommand OpenWebsiteCommand { get; }
        public ICommand OpenWhatsAppCommand { get; }

        public SupportViewModel()
        {
            OpenWebsiteCommand = new RelayCommand(_ => OpenWebsite());
            OpenWhatsAppCommand = new RelayCommand(_ => OpenWhatsApp());

            LoadCompanyInfo();
        }

        private void LoadCompanyInfo()
        {
            CompanyInfo = "═══════════════════════════════════════\n";
            CompanyInfo += "         KOBİ BİLİŞİM\n";
            CompanyInfo += "   CAFE & RESTORAN SİSTEMİ\n";
            CompanyInfo += "═══════════════════════════════════════\n\n";
            CompanyInfo += "📞 Telefon: 0552 165 04 35\n";
            CompanyInfo += "🌐 Web: www.kobibilisim.com\n";
            CompanyInfo += "💬 WhatsApp: 0552 165 04 35\n\n";
            CompanyInfo += "═══════════════════════════════════════\n";
            CompanyInfo += "Versiyon: 1.0.0\n";
            CompanyInfo += $"© {DateTime.Now.Year} Kobi Bilişim\n";
            CompanyInfo += "═══════════════════════════════════════";
        }

        private void OpenWebsite()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "http://www.kobibilisim.com",
                    UseShellExecute = true
                });
            }
            catch
            {
                // Handle error silently
            }
        }

        private void OpenWhatsApp()
        {
            try
            {
                var phoneNumber = "905521650435"; // Remove spaces and add country code
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://wa.me/{phoneNumber}",
                    UseShellExecute = true
                });
            }
            catch
            {
                // Handle error silently
            }
        }
    }
}
