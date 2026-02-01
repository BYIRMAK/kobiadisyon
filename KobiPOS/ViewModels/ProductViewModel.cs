using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly DatabaseService _db;
        private readonly User _currentUser;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Product> _products;
        private Category? _selectedCategory;
        private Product? _selectedProduct;

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    LoadProducts();
                }
            }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public ICommand RefreshCommand { get; }

        public ProductViewModel(User currentUser)
        {
            _currentUser = currentUser;
            _db = DatabaseService.Instance;
            _categories = new ObservableCollection<Category>();
            _products = new ObservableCollection<Product>();

            RefreshCommand = new RelayCommand(_ => LoadCategories());

            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = _db.GetAllCategories();
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            if (Categories.Count > 0)
            {
                SelectedCategory = Categories[0];
            }
        }

        private void LoadProducts()
        {
            Products.Clear();

            if (SelectedCategory != null)
            {
                var products = _db.GetProductsByCategory(SelectedCategory.ID);
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
        }
    }
}
