using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace KobiPOS.Helpers
{
    public class PathToImageSourceConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath && !string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    // Convert relative path to absolute path
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string fullPath = Path.Combine(baseDirectory, imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    
                    if (File.Exists(fullPath))
                    {
                        // Create BitmapImage from file path
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                        bitmap.EndInit();
                        bitmap.Freeze(); // Important for performance
                        return bitmap;
                    }
                }
                catch
                {
                    // If there's an error loading the image, return null
                    return null;
                }
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
