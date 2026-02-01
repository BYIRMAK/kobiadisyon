using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KobiPOS.Models
{
    public class OrderItem : INotifyPropertyChanged
    {
        private int _quantity;
        private string _notes = string.Empty;
        private decimal _lineTotal;

        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    UpdateLineTotal();
                }
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal LineTotal
        {
            get => _lineTotal;
            private set
            {
                if (_lineTotal != value)
                {
                    _lineTotal = value;
                    OnPropertyChanged();
                }
            }
        }

        // Yeni alanlar: Zaman takibi için
        public DateTime AddedTime { get; set; } = DateTime.Now;
        public string AddedBy { get; set; } = string.Empty;

        // UI için computed properties
        public string FormattedTime => AddedTime.ToString("HH:mm");
        public string DetailedTime => AddedTime.ToString("HH:mm:ss");
        public string TimeAndUser => $"{DetailedTime} - {AddedBy}";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateLineTotal()
        {
            LineTotal = Quantity * UnitPrice;
        }

        public OrderItem()
        {
            UpdateLineTotal();
        }
    }
}
