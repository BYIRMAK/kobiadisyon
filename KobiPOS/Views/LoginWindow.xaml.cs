using System.Windows;
using KobiPOS.ViewModels;
using KobiPOS.Models;

namespace KobiPOS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            _viewModel.LoginSuccessful += OnLoginSuccessful;
            DataContext = _viewModel;

            // Set initial focus
            Loaded += (s, e) => UsernameTextBox.Focus();

            // Handle Enter key
            UsernameTextBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                    PasswordBox.Focus();
            };

            PasswordBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                    LoginButton_Click(s, e);
            };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Username = UsernameTextBox.Text;
            _viewModel.Password = PasswordBox.Password;
            _viewModel.LoginCommand.Execute(null);

            ErrorTextBlock.Text = _viewModel.ErrorMessage;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnLoginSuccessful(object? sender, User user)
        {
            var mainWindow = new MainWindow(user);
            mainWindow.Show();
            Close();
        }
    }
}
