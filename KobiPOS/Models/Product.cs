using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace KobiPOS.Models
{
    public class Product : INotifyPropertyChanged
    {
        private string _imagePath = string.Empty;

        public int ID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Barcode { get; set; } = string.Empty;
        
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasImage));
                    OnPropertyChanged(nameof(ImageSource));
                }
            }
        }
        
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = "Adet";
        public bool StockTracking { get; set; }
        public int CurrentStock { get; set; }
        public bool IsActive { get; set; } = true;
        
        public bool HasImage => !string.IsNullOrEmpty(ImagePath);
        
        /// <summary>
        /// Returns the absolute path for the image to be used in WPF bindings
        /// </summary>
        public string? ImageSource
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                {
                    return null;
                }
                
                try
                {
                    // Use the existing converter's logic to get absolute path
                    string fullPath = Helpers.PathToImageSourceConverter.ConvertToAbsolutePath(ImagePath);
                    
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
                catch
                {
                    // If there's any error, return null
                }
                
                return null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
