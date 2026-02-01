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
                    string fullPath = ConvertToAbsolutePath(imagePath);
                    
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
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is NotSupportedException)
                {
                    // If there's an error loading the image (file locked, permission denied, or invalid format), return null
                    // This will cause the image control to not display anything
                    System.Diagnostics.Debug.WriteLine($"Failed to load image from {imagePath}: {ex.Message}");
                    return null;
                }
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Converts a relative image path (e.g., /Images/Products/1.jpg) to an absolute file system path
        /// </summary>
        public static string ConvertToAbsolutePath(string relativePath)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
