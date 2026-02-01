using System.Windows.Controls;
using System.Windows.Input;

namespace KobiPOS.Views
{
    /// <summary>
    /// Interaction logic for ProductManagementView.xaml
    /// </summary>
    public partial class ProductManagementView : UserControl
    {
        public ProductManagementView()
        {
            InitializeComponent();
        }

        private void DialogBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Close dialog only if clicking on the background, not the dialog content
            if (e.Source == sender)
            {
                var viewModel = DataContext as ViewModels.ProductManagementViewModel;
                viewModel?.CancelEditCommand.Execute(null);
            }
        }
    }
}
