using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KobiPOS.Models
{
    public class Category : INotifyPropertyChanged
    {
        private bool _isSelected;

        public int ID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
