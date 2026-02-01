using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KobiPOS.Views
{
    public partial class ZoneManagementView : UserControl
    {
        public ZoneManagementView()
        {
            InitializeComponent();
        }

        private void DialogBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Close dialog only if clicking on the background, not the dialog content
            if (e.Source == sender)
            {
                var viewModel = DataContext as ViewModels.ZoneManagementViewModel;
                viewModel?.CancelEditCommand.Execute(null);
            }
        }
    }
}
