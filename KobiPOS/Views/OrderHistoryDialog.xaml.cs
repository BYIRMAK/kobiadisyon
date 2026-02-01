using System.Windows;
using KobiPOS.Models;
using KobiPOS.ViewModels;

namespace KobiPOS.Views
{
    public partial class OrderHistoryDialog : Window
    {
        public OrderHistoryDialog(Order order, User currentUser)
        {
            InitializeComponent();
            DataContext = new OrderHistoryViewModel(order, currentUser);
            
            // Close command handler
            var viewModel = DataContext as OrderHistoryViewModel;
            if (viewModel != null)
            {
                viewModel.CloseRequested += (s, e) => this.Close();
            }
        }
    }
}
