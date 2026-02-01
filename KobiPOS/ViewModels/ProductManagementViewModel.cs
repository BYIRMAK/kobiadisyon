using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class ProductManagementViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Category> _categories;
        private Product? _selectedProduct;
        private bool _isEditMode;
        private Product _editingProduct;

        public ProductManagementViewModel()
        {
            _databaseService = DatabaseService.Instance;
            _products = new ObservableCollection<Product>();
            _categories = new ObservableCollection<Category>();
            _editingProduct = new Product();

            AddProductCommand = new RelayCommand(ExecuteAddProduct);
            EditProductCommand = new RelayCommand(ExecuteEditProduct, CanEditOrDelete);
            DeleteProductCommand = new RelayCommand(ExecuteDeleteProduct, CanEditOrDelete);
            SaveProductCommand = new RelayCommand(ExecuteSaveProduct, CanSaveProduct);
            CancelEditCommand = new RelayCommand(ExecuteCancelEdit);

            LoadCategories();
            LoadProducts();
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
            if (SelectedProduct == null) return;

            EditingProduct = new Product
            {
                ID = SelectedProduct.ID,
                ProductName = SelectedProduct.ProductName,
                CategoryID = SelectedProduct.CategoryID,
                Price = SelectedProduct.Price,
                StockQuantity = SelectedProduct.StockQuantity,
                Barcode = SelectedProduct.Barcode,
                ImagePath = SelectedProduct.ImagePath,
                Description = SelectedProduct.Description,
                Unit = SelectedProduct.Unit,
                StockTracking = SelectedProduct.StockTracking,
                CurrentStock = SelectedProduct.CurrentStock,
                IsActive = SelectedProduct.IsActive
            };
            IsEditMode = true;
        }

        private void ExecuteDeleteProduct(object? parameter)
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show(
                $"'{SelectedProduct.ProductName}' ürününü silmek istediğinizden emin misiniz?",
                "Ürün Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _databaseService.DeleteProduct(SelectedProduct.ID);
                LoadProducts();
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ürün kaydedilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancelEdit(object? parameter)
        {
            IsEditMode = false;
            EditingProduct = new Product();
        }

        private bool CanEditOrDelete(object? parameter)
        {
            return SelectedProduct != null;
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
    }
}
