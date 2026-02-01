using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;

namespace KobiPOS.ViewModels
{
    public class ProductManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Product> _filteredProducts;
        private Product? _selectedProduct;
        private Category? _selectedCategory;
        private bool _isEditMode;
        private Product _editingProduct;

        public ProductManagementViewModel()
        {
            _databaseService = DatabaseService.Instance;
            _products = new ObservableCollection<Product>();
            _categories = new ObservableCollection<Category>();
            _filteredProducts = new ObservableCollection<Product>();
            _editingProduct = new Product();

            AddProductCommand = new RelayCommand(ExecuteAddProduct);
            EditProductCommand = new RelayCommand(ExecuteEditProduct, CanEditOrDelete);
            DeleteProductCommand = new RelayCommand(ExecuteDeleteProduct, CanEditOrDelete);
            SaveProductCommand = new RelayCommand(ExecuteSaveProduct, CanSaveProduct);
            CancelEditCommand = new RelayCommand(ExecuteCancelEdit);
            SelectImageCommand = new RelayCommand(ExecuteSelectImage);
            RemoveImageCommand = new RelayCommand(ExecuteRemoveImage);
            FilterCategoryCommand = new RelayCommand(ExecuteFilterCategory);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            LoadCategories();
            LoadProducts();
            
            // Set first category as selected by default, or show all if no categories
            if (Categories.Count > 0)
            {
                SelectedCategory = Categories[0];
            }
            else
            {
                // No categories available, show all products
                FilterProductsByCategory();
            }
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set => SetProperty(ref _filteredProducts, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    // Update IsSelected property for all categories
                    foreach (var category in Categories)
                    {
                        category.IsSelected = (category == value);
                    }
                    FilterProductsByCategory();
                }
            }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public Product EditingProduct
        {
            get => _editingProduct;
            set => SetProperty(ref _editingProduct, value);
        }

        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand SaveProductCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand FilterCategoryCommand { get; }
        public ICommand RefreshCommand { get; }

        private void LoadCategories()
        {
            try
            {
                var categories = _databaseService.GetAllCategories();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategoriler yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _databaseService.GetAllProducts();
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
                
                // Refresh filtered products after loading
                FilterProductsByCategory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ürünler yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddProduct(object? parameter)
        {
            EditingProduct = new Product
            {
                IsActive = true,
                Unit = "Adet",
                StockTracking = false,
                StockQuantity = 0,
                CurrentStock = 0
            };
            IsEditMode = true;
        }

        private void ExecuteEditProduct(object? parameter)
        {
            // Parameter olarak gelen Product nesnesini kullan
            var product = parameter as Product;
            if (product == null) return;

            SelectedProduct = product; // Seçili ürünü ayarla

            EditingProduct = new Product
            {
                ID = product.ID,
                ProductName = product.ProductName,
                CategoryID = product.CategoryID,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Barcode = product.Barcode,
                ImagePath = product.ImagePath,
                Description = product.Description,
                Unit = product.Unit,
                StockTracking = product.StockTracking,
                CurrentStock = product.CurrentStock,
                IsActive = product.IsActive
            };
            IsEditMode = true;
        }

        private void ExecuteDeleteProduct(object? parameter)
        {
            // Parameter olarak gelen Product nesnesini kullan
            var product = parameter as Product;
            if (product == null) return;

            var result = MessageBox.Show(
                $"'{product.ProductName}' ürününü silmek istediğinizden emin misiniz?",
                "Ürün Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _databaseService.DeleteProduct(product.ID);
                LoadProducts();
                FilterProductsByCategory(); // Filtreyi yenile
                MessageBox.Show("Ürün başarıyla silindi.", "Başarılı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ürün silinirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSaveProduct(object? parameter)
        {
            if (!ValidateProduct()) return;

            try
            {
                if (EditingProduct.ID == 0)
                {
                    _databaseService.AddProduct(EditingProduct);
                    
                    // If there's a temp image, rename it to use the actual product ID
                    if (!string.IsNullOrEmpty(EditingProduct.ImagePath) && EditingProduct.ImagePath.Contains("temp_"))
                    {
                        UpdateTempImagePath(EditingProduct);
                    }
                    
                    MessageBox.Show("Ürün başarıyla eklendi.", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _databaseService.UpdateProduct(EditingProduct);
                    MessageBox.Show("Ürün başarıyla güncellendi.", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                IsEditMode = false;
                LoadProducts();
                FilterProductsByCategory(); // Filtreyi güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ürün kaydedilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void UpdateTempImagePath(Product product)
        {
            try
            {
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Products");
                string oldFileName = Path.GetFileName(product.ImagePath);
                string oldPath = Path.Combine(imagesFolder, oldFileName);
                
                if (File.Exists(oldPath))
                {
                    string newFileName = $"{product.ID}.jpg";
                    string newPath = Path.Combine(imagesFolder, newFileName);
                    
                    // Delete old file if it exists
                    if (File.Exists(newPath))
                    {
                        File.Delete(newPath);
                    }
                    
                    File.Move(oldPath, newPath);
                    product.ImagePath = $"/Images/Products/{newFileName}";
                    _databaseService.UpdateProduct(product);
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail - the temp image will remain
                System.Diagnostics.Debug.WriteLine($"Failed to update temp image path: {ex.Message}");
            }
        }

        private void ExecuteCancelEdit(object? parameter)
        {
            IsEditMode = false;
            EditingProduct = new Product();
        }

        private void ExecuteSelectImage(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Ürün Görseli Seç",
                Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp|Tüm Dosyalar|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;

                // File size check (2 MB = 2,097,152 bytes)
                var fileInfo = new FileInfo(selectedFile);
                if (fileInfo.Length > 2097152)
                {
                    MessageBox.Show("Resim boyutu 2 MB'dan büyük olamaz!", "Uyarı", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Save image and get path
                string imagePath = SaveProductImage(selectedFile, EditingProduct.ID);

                if (!string.IsNullOrEmpty(imagePath))
                {
                    EditingProduct.ImagePath = imagePath;
                    OnPropertyChanged(nameof(EditingProduct));
                    MessageBox.Show("Resim başarıyla yüklendi!", "Başarılı", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ExecuteRemoveImage(object? parameter)
        {
            if (MessageBox.Show("Ürün görselini kaldırmak istediğinize emin misiniz?",
                "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Delete physical file if it exists
                    // Only delete files for saved products (ID > 0), not temporary files used during product creation
                    if (EditingProduct.ID > 0 && !string.IsNullOrEmpty(EditingProduct.ImagePath))
                    {
                        string fullPath = Helpers.PathToImageSourceConverter.ConvertToAbsolutePath(EditingProduct.ImagePath);
                        
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                    }
                    
                    EditingProduct.ImagePath = string.Empty;
                    OnPropertyChanged(nameof(EditingProduct));
                    MessageBox.Show("Resim kaldırıldı!", "Başarılı", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Resim kaldırılırken hata oluştu: {ex.Message}", "Hata",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string SaveProductImage(string sourceFilePath, int productId)
        {
            try
            {
                // Create Images/Products folder
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Products");
                Directory.CreateDirectory(imagesFolder);

                // Always use .jpg extension since we're saving as JPEG
                string fileName = productId > 0 
                    ? $"{productId}.jpg" 
                    : $"temp_{DateTime.Now.Ticks}.jpg";
                string destinationPath = Path.Combine(imagesFolder, fileName);

                // Resize and save image (300x300)
                ResizeAndSaveImage(sourceFilePath, destinationPath, 300, 300);

                // Return relative path
                return $"/Images/Products/{fileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Resim kaydedilirken hata: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        private void ResizeAndSaveImage(string sourcePath, string destinationPath, int width, int height)
        {
            using (var image = System.Drawing.Image.FromFile(sourcePath))
            using (var resized = new System.Drawing.Bitmap(width, height))
            {
                using (var graphics = System.Drawing.Graphics.FromImage(resized))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image, 0, 0, width, height);
                }
                resized.Save(destinationPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private bool CanEditOrDelete(object? parameter)
        {
            return parameter is Product;
        }

        private bool CanSaveProduct(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(EditingProduct?.ProductName) &&
                   EditingProduct?.CategoryID > 0 &&
                   EditingProduct?.Price >= 0;
        }

        private bool ValidateProduct()
        {
            if (string.IsNullOrWhiteSpace(EditingProduct.ProductName))
            {
                MessageBox.Show("Ürün adı boş olamaz.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditingProduct.CategoryID <= 0)
            {
                MessageBox.Show("Lütfen bir kategori seçin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditingProduct.Price < 0)
            {
                MessageBox.Show("Fiyat negatif olamaz.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ExecuteFilterCategory(object? parameter)
        {
            if (parameter is Category category)
            {
                SelectedCategory = category;
            }
        }

        private void ExecuteRefresh(object? parameter)
        {
            LoadProducts();
            FilterProductsByCategory();
        }

        private void FilterProductsByCategory()
        {
            FilteredProducts.Clear();

            if (SelectedCategory == null)
            {
                // Show all products
                foreach (var product in Products)
                {
                    FilteredProducts.Add(product);
                }
            }
            else
            {
                // Filter by selected category
                var filtered = Products.Where(p => p.CategoryID == SelectedCategory.ID);
                foreach (var product in filtered)
                {
                    FilteredProducts.Add(product);
                }
            }
        }
    }
}
