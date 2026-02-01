using KobiPOS.Helpers;
using KobiPOS.Models;
using KobiPOS.Services;
using System.Windows;
using System.Windows.Input;

namespace KobiPOS.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly DatabaseService _db;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ExitCommand { get; }

        public event EventHandler<User>? LoginSuccessful;

        public LoginViewModel()
        {
            _db = DatabaseService.Instance;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
        }

        private bool CanExecuteLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object? parameter)
        {
            ErrorMessage = string.Empty;

            var user = _db.ValidateUser(Username, Password);
            if (user != null)
            {
                LoginSuccessful?.Invoke(this, user);
            }
            else
            {
                ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
                Password = string.Empty;
            }
        }
    }
}
