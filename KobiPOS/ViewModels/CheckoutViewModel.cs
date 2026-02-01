using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class CheckoutViewModel : ViewModelBase
    {
        private readonly DatabaseService _db;
        private readonly User _currentUser;
        private readonly Table _table;
        private readonly Order _order;
        private readonly List<OrderItem> _orderItems;
        
        private ObservableCollection<OrderItem> _items;
        private decimal _subTotal;
        private decimal _taxAmount;
        private decimal _discountPercent;
        private decimal _discountAmount;
        private decimal _totalAmount;
        private string _selectedPaymentType;
        private decimal _cashReceived;
        private decimal _changeAmount;

        private decimal GrossTotal => SubTotal + TaxAmount;

        public Table Table => _table;
        public User CurrentUser => _currentUser;

        public ObservableCollection<OrderItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
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

        public decimal DiscountPercent
        {
            get => _discountPercent;
            set
            {
                if (SetProperty(ref _discountPercent, value))
                {
                    CalculateDiscount();
                }
            }
        }

        public decimal DiscountAmount
        {
            get => _discountAmount;
            set
            {
                if (SetProperty(ref _discountAmount, value))
                {
                    CalculateTotal();
                }
            }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }

        public string SelectedPaymentType
        {
            get => _selectedPaymentType;
            set
            {
                if (SetProperty(ref _selectedPaymentType, value))
                {
                    OnPropertyChanged(nameof(IsCashPayment));
                    
                    // Auto-fill cash received with total amount when cash is selected
                    if (value == PaymentType.Cash)
                    {
                        CashReceived = TotalAmount;
                        
                        // Nakit seçildiğinde direkt ödemeyi tamamla
                        CompletePayment();
                        return;
                    }
                    
                    // Re-evaluate the CompletePaymentCommand's CanExecute
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public decimal CashReceived
        {
            get => _cashReceived;
            set
            {
                if (SetProperty(ref _cashReceived, value))
                {
                    CalculateChange();
                    
                    // Re-evaluate the CompletePaymentCommand's CanExecute
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public decimal ChangeAmount
        {
            get => _changeAmount;
            set
            {
                if (SetProperty(ref _changeAmount, value))
                {
                    OnPropertyChanged(nameof(HasChange));
                }
            }
        }

        public bool IsCashPayment => SelectedPaymentType == PaymentType.Cash;
        
        public bool HasChange => ChangeAmount > 0;

        public ICommand ApplyPercentDiscountCommand { get; }
        public ICommand ApplyAmountDiscountCommand { get; }
        public ICommand SelectPaymentTypeCommand { get; }
        public ICommand CompletePaymentCommand { get; }
        public ICommand PrintReceiptCommand { get; }
        public ICommand BackCommand { get; }

        public event EventHandler? PaymentCompleted;
        public event EventHandler? BackRequested;
        public event EventHandler<string>? ReceiptRequested;

        public CheckoutViewModel(User currentUser, Table table, Order order, List<OrderItem> orderItems)
        {
            _currentUser = currentUser;
            _table = table;
            _order = order;
            _orderItems = orderItems;
            _db = DatabaseService.Instance;

            _items = new ObservableCollection<OrderItem>(orderItems);
            _selectedPaymentType = PaymentType.Cash;

            SubTotal = order.SubTotal;
            TaxAmount = order.TaxAmount;
            DiscountPercent = 0;
            DiscountAmount = 0;
            TotalAmount = order.TotalAmount;

            ApplyPercentDiscountCommand = new RelayCommand(_ => ApplyPercentDiscount());
            ApplyAmountDiscountCommand = new RelayCommand(_ => ApplyAmountDiscount());
            SelectPaymentTypeCommand = new RelayCommand(param => SelectedPaymentType = param?.ToString() ?? PaymentType.Cash);
            CompletePaymentCommand = new RelayCommand(_ => CompletePayment(), _ => CanCompletePayment());
            PrintReceiptCommand = new RelayCommand(_ => PrintReceipt());
            BackCommand = new RelayCommand(_ => BackRequested?.Invoke(this, EventArgs.Empty));
        }

        private void ApplyPercentDiscount()
        {
            if (DiscountPercent >= 0 && DiscountPercent <= 100)
            {
                DiscountAmount = GrossTotal * (DiscountPercent / 100);
            }
            else
            {
                MessageBox.Show("İndirim yüzdesi 0-100 arasında olmalıdır.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ApplyAmountDiscount()
        {
            if (DiscountAmount >= 0 && DiscountAmount <= GrossTotal)
            {
                DiscountPercent = GrossTotal > 0 ? (DiscountAmount / GrossTotal) * 100 : 0;
                // Note: CalculateTotal is called automatically when DiscountAmount setter triggers property change notification
            }
            else
            {
                MessageBox.Show($"İndirim tutarı 0-{GrossTotal:N2} ₺ arasında olmalıdır.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CalculateDiscount()
        {
            DiscountAmount = GrossTotal * (DiscountPercent / 100);
        }

        private void CalculateTotal()
        {
            TotalAmount = GrossTotal - DiscountAmount;
            CalculateChange();
        }

        private void CalculateChange()
        {
            if (IsCashPayment && CashReceived >= TotalAmount)
            {
                ChangeAmount = CashReceived - TotalAmount;
            }
            else
            {
                ChangeAmount = 0;
            }
        }

        private bool CanCompletePayment()
        {
            // Sadece ödeme türü seçilmişse yeterli
            return !string.IsNullOrEmpty(SelectedPaymentType);
        }

        private void CompletePayment()
        {
            if (!CanCompletePayment())
            {
                MessageBox.Show("Lütfen ödeme bilgilerini kontrol edin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Nakit için tutar kontrolü
            if (IsCashPayment && CashReceived < TotalAmount)
            {
                MessageBox.Show(
                    $"Alınan tutar yetersiz!\n\nToplam: {TotalAmount:N2} ₺\nAlınan: {CashReceived:N2} ₺",
                    "Uyarı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            try
            {
                // Update order with payment information
                _order.SubTotal = SubTotal;
                _order.TaxAmount = TaxAmount;
                _order.DiscountAmount = DiscountAmount;
                _order.DiscountPercent = DiscountPercent;
                _order.TotalAmount = TotalAmount;
                _order.PaymentType = SelectedPaymentType;
                _order.Status = OrderStatus.Served;

                _db.UpdateOrder(_order);

                // Update table status to empty
                _db.UpdateTableStatus(_table.ID, TableStatus.Empty);

                MessageBox.Show(
                    $"Ödeme tamamlandı!\n\nToplam: {TotalAmount:N2} ₺\nÖdeme Türü: {SelectedPaymentType}" +
                    (IsCashPayment ? $"\nAlınan: {CashReceived:N2} ₺\nPara Üstü: {ChangeAmount:N2} ₺" : ""),
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                PaymentCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ödeme tamamlanırken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintReceipt()
        {
            var receipt = GenerateReceipt();
            ReceiptRequested?.Invoke(this, receipt);
        }

        private string GenerateReceipt()
        {
            var receipt = new System.Text.StringBuilder();
            receipt.AppendLine("========================================");
            receipt.AppendLine("        KOBİ BİLİŞİM");
            receipt.AppendLine("    Cafe & Restoran Sistemi");
            receipt.AppendLine("----------------------------------------");
            receipt.AppendLine("Telefon: 0552 165 04 35");
            receipt.AppendLine("Web: www.kobibilisim.com");
            receipt.AppendLine("========================================");
            receipt.AppendLine();
            receipt.AppendLine($"Masa No: {_table.TableNumber}");
            receipt.AppendLine($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}");
            receipt.AppendLine($"Kullanıcı: {_currentUser.FullName} ({_currentUser.Role})");
            receipt.AppendLine();
            receipt.AppendLine("----------------------------------------");
            receipt.AppendLine("ÜRÜNLER");
            receipt.AppendLine("----------------------------------------");

            foreach (var item in Items)
            {
                receipt.AppendLine($"{item.Quantity}x {item.ProductName,-20} {item.LineTotal,10:N2} ₺");
                if (!string.IsNullOrWhiteSpace(item.Notes))
                {
                    receipt.AppendLine($"   Not: {item.Notes}");
                }
            }

            receipt.AppendLine("----------------------------------------");
            receipt.AppendLine($"Ara Toplam:              {SubTotal,10:N2} ₺");
            receipt.AppendLine($"KDV (%20):               {TaxAmount,10:N2} ₺");
            
            if (DiscountAmount > 0)
            {
                receipt.AppendLine($"İndirim ({DiscountPercent:N1}%):       {DiscountAmount,10:N2} ₺");
            }
            
            receipt.AppendLine("----------------------------------------");
            receipt.AppendLine($"GENEL TOPLAM:           {TotalAmount,10:N2} ₺");
            receipt.AppendLine("----------------------------------------");
            receipt.AppendLine($"Ödeme: {SelectedPaymentType}");
            
            if (IsCashPayment)
            {
                receipt.AppendLine($"Alınan:                 {CashReceived,10:N2} ₺");
                receipt.AppendLine($"Para Üstü:              {ChangeAmount,10:N2} ₺");
            }
            
            receipt.AppendLine("========================================");
            receipt.AppendLine("     Afiyet olsun!");
            receipt.AppendLine("   Tekrar bekleriz...");
            receipt.AppendLine("========================================");

            return receipt.ToString();
        }
    }
}
