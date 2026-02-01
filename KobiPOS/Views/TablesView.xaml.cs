using System.Windows.Controls;
using System.Windows.Input;
using KobiPOS.ViewModels;

namespace KobiPOS.Views
{
    public partial class TablesView : UserControl
    {
        public TablesView()
        {
            InitializeComponent();
        }

        private void TableCard_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var tableDisplayModel = border?.DataContext as TableDisplayModel;
            if (tableDisplayModel == null) return;

            // Direkt OrderView'ı aç (ViewModel üzerinden)
            var viewModel = DataContext as TablesViewModel;
            viewModel?.OpenTableCommand.Execute(tableDisplayModel);
        }
    }
}
