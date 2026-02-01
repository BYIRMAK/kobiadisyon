using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        private readonly DatabaseService _db;
        private readonly User _currentUser;
        private readonly Table _table;
        private Order? _currentOrder;
        
        private ObservableCollection<Category> _categories;
        private Category? _selectedCategory;
        private ObservableCollection<Product> _products;
        private ObservableCollection<OrderItem> _orderItems;
        private decimal _subTotal;
        private decimal _taxAmount;
        private decimal _totalAmount;
        private const decimal TAX_RATE = 0.20m; // 20% KDV

        public Table Table
        {
            get => _table;
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    LoadProductsByCategory();
                }
            }
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ObservableCollection<OrderItem> OrderItems
        {
            get => _orderItems;
            set => SetProperty(ref _orderItems, value);
        }

        public decimal SubTotal
        {
            get => _subTotal;
            set => SetProperty(ref _subTotal, value);
        }

        public decimal TaxAmount
        {
            get => _taxAmount;
            set => SetProperty(ref _taxAmount, value);
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }

        public ICommand AddProductCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SaveOrderCommand { get; }
        public ICommand CloseCheckCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SelectCategoryCommand { get; }

        public event EventHandler? OrderSaved;
        public event EventHandler? CheckoutRequested;
        public event EventHandler? BackRequested;

        public Order? CurrentOrder => _currentOrder;
        public List<OrderItem> CurrentOrderItems => OrderItems.ToList();

        public OrderViewModel(User currentUser, Table table)
        {
            _currentUser = currentUser;
            _table = table;
            _db = DatabaseService.Instance;
            
            _categories = new ObservableCollection<Category>();
            _products = new ObservableCollection<Product>();
            _orderItems = new ObservableCollection<OrderItem>();

            AddProductCommand = new RelayCommand(ExecuteAddProduct);
            IncreaseQuantityCommand = new RelayCommand(ExecuteIncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(ExecuteDecreaseQuantity);
            RemoveItemCommand = new RelayCommand(ExecuteRemoveItem);
            SaveOrderCommand = new RelayCommand(_ => ExecuteSaveOrder());
            CloseCheckCommand = new RelayCommand(_ => ExecuteCloseCheck(), _ => OrderItems.Count > 0);
            BackCommand = new RelayCommand(_ => BackRequested?.Invoke(this, EventArgs.Empty));
            SelectCategoryCommand = new RelayCommand(param => SelectedCategory = param as Category);

            LoadCategories();
            LoadExistingOrder();
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

        private void LoadProductsByCategory()
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

        private void LoadExistingOrder()
        {
            _currentOrder = _db.GetPendingOrderByTable(_table.ID);
            if (_currentOrder != null)
            {
                var details = _db.GetOrderDetails(_currentOrder.ID);
                foreach (var detail in details)
                {
                    var item = new OrderItem
                    {
                        ProductID = detail.ProductID,
                        ProductName = detail.ProductName,
                        UnitPrice = detail.UnitPrice,
                        Quantity = detail.Quantity,
                        Notes = detail.Notes
                    };
                    item.PropertyChanged += OrderItem_PropertyChanged;
                    OrderItems.Add(item);
                }
                CalculateTotals();
            }
        }

        private void ExecuteAddProduct(object? parameter)
        {
            if (parameter is Product product)
            {
                var existingItem = OrderItems.FirstOrDefault(x => x.ProductID == product.ID);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    var newItem = new OrderItem
                    {
                        ProductID = product.ID,
                        ProductName = product.ProductName,
                        UnitPrice = product.Price,
                        Quantity = 1
                    };
                    newItem.PropertyChanged += OrderItem_PropertyChanged;
                    OrderItems.Add(newItem);
                }
                CalculateTotals();
            }
        }

        private void ExecuteIncreaseQuantity(object? parameter)
        {
            if (parameter is OrderItem item)
            {
                item.Quantity++;
            }
        }

        private void ExecuteDecreaseQuantity(object? parameter)
        {
            if (parameter is OrderItem item && item.Quantity > 1)
            {
                item.Quantity--;
            }
        }

        private void ExecuteRemoveItem(object? parameter)
        {
            if (parameter is OrderItem item)
            {
                item.PropertyChanged -= OrderItem_PropertyChanged;
                OrderItems.Remove(item);
                CalculateTotals();
            }
        }

        private void OrderItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderItem.Quantity) || e.PropertyName == nameof(OrderItem.LineTotal))
            {
                CalculateTotals();
            }
        }

        private void CalculateTotals()
        {
            SubTotal = OrderItems.Sum(x => x.LineTotal);
            TaxAmount = SubTotal * TAX_RATE;
            TotalAmount = SubTotal + TaxAmount;
        }

        private void ExecuteSaveOrder()
        {
            if (OrderItems.Count == 0)
            {
                MessageBox.Show("Sipariş boş olamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_currentOrder == null)
                {
                    // Create new order
                    _currentOrder = new Order
                    {
                        TableID = _table.ID,
                        OrderDate = DateTime.Now,
                        UserID = _currentUser.ID,
                        SubTotal = SubTotal,
                        TaxAmount = TaxAmount,
                        TotalAmount = TotalAmount,
                        Status = "Bekliyor"
                    };
                    _currentOrder.ID = _db.CreateOrder(_currentOrder);
                }
                else
                {
                    // Update existing order
                    _currentOrder.SubTotal = SubTotal;
                    _currentOrder.TaxAmount = TaxAmount;
                    _currentOrder.TotalAmount = TotalAmount;
                    _db.UpdateOrder(_currentOrder);
                    _db.DeleteOrderDetails(_currentOrder.ID);
                }

                // Add order details
                foreach (var item in OrderItems)
                {
                    var detail = new OrderDetail
                    {
                        OrderID = _currentOrder.ID,
                        ProductID = item.ProductID,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.LineTotal,
                        Notes = item.Notes
                    };
                    _db.AddOrderDetail(detail);
                }

                // Update table status
                _db.UpdateTableStatus(_table.ID, "Dolu");

                MessageBox.Show("Sipariş kaydedildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                OrderSaved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sipariş kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCloseCheck()
        {
            if (OrderItems.Count == 0)
            {
                MessageBox.Show("Sipariş boş olamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // First save the order
            ExecuteSaveOrder();
            
            // Then navigate to checkout
            CheckoutRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
