using KobiPOS.Helpers;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        private string _reportData = "Raporlar hazırlanıyor...";

        public string ReportData
        {
            get => _reportData;
            set => SetProperty(ref _reportData, value);
        }

        public ICommand GenerateReportCommand { get; }

        public ReportViewModel()
        {
            GenerateReportCommand = new RelayCommand(_ => GenerateReport());
            GenerateReport();
        }

        private void GenerateReport()
        {
            ReportData = "=== GÜNLÜK RAPOR ===\n\n";
            ReportData += $"Tarih: {DateTime.Now:dd.MM.yyyy}\n";
            ReportData += "Toplam Satış: 0.00 ₺\n";
            ReportData += "Toplam Sipariş: 0\n";
            ReportData += "\nDetaylı raporlar geliştirme aşamasında...";
        }
    }
}
