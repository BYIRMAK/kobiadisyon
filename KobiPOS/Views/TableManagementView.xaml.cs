using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace KobiPOS.Views
{
    public partial class TableManagementView : UserControl
    {
        public TableManagementView()
        {
            InitializeComponent();
        }

        private void DialogBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == sender)
            {
                var viewModel = DataContext as ViewModels.TableManagementViewModel;
                viewModel?.CancelEditCommand.Execute(null);
            }
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$");
        }
    }
}
