using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class OrderHistoryViewModel : ViewModelBase
    {
        private readonly Order _order;
        private ObservableCollection<OrderDetail> _orderHistory;
        
        public event EventHandler? CloseRequested;
        
        public OrderHistoryViewModel(Order order, User currentUser)
        {
            _order = order;
            _orderHistory = new ObservableCollection<OrderDetail>();
            
            CloseCommand = new RelayCommand(_ => OnCloseRequested());
            
            LoadOrderHistory();
        }
        
        public ObservableCollection<OrderDetail> OrderHistory
        {
            get => _orderHistory;
            set => SetProperty(ref _orderHistory, value);
        }
        
        public string Title
        {
            get
            {
                var db = DatabaseService.Instance;
                var table = db.GetTableById(_order.TableID);
                return $"📜 Sipariş Geçmişi - {table?.TableName ?? "Masa"}";
            }
        }
        
        public string TotalItemsText => $"TOPLAM: {OrderHistory.Count} sipariş";
        
        public string TotalAmountText => $"{OrderHistory.Sum(x => x.LineTotal):N2} ₺";
        
        public ICommand CloseCommand { get; }
        
        private void LoadOrderHistory()
        {
            var db = DatabaseService.Instance;
            var items = db.GetOrderDetails(_order.ID);
            
            OrderHistory.Clear();
            foreach (var item in items)
            {
                OrderHistory.Add(item);
            }
            
            // Notify properties that depend on OrderHistory
            OnPropertyChanged(nameof(TotalItemsText));
            OnPropertyChanged(nameof(TotalAmountText));
        }
        
        private void OnCloseRequested()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
