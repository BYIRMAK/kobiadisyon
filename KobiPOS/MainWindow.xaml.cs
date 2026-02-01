using System.Windows;
using KobiPOS.Models;
using KobiPOS.ViewModels;
using KobiPOS.Views;

namespace KobiPOS;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(User currentUser)
    {
        InitializeComponent();
        _viewModel = new MainViewModel(currentUser);
        _viewModel.LogoutRequested += OnLogoutRequested;
        DataContext = _viewModel;

        TitleTextBlock.Text = _viewModel.Title;
        UserInfoTextBlock.Text = $"Kullanıcı: {currentUser.FullName} ({currentUser.Role})";

        // Show initial view
        ShowTables();
    }

    private void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        ShowHome();
    }

    private void TablesButton_Click(object sender, RoutedEventArgs e)
    {
        ShowTables();
    }

    private void ProductsButton_Click(object sender, RoutedEventArgs e)
    {
        ShowProducts();
    }

    private void ReportsButton_Click(object sender, RoutedEventArgs e)
    {
        ShowReports();
    }

    private void LicenseButton_Click(object sender, RoutedEventArgs e)
    {
        ShowLicense();
    }

    private void SupportButton_Click(object sender, RoutedEventArgs e)
    {
        ShowSupport();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Çıkış yapmak istediğinizden emin misiniz?",
            "Çıkış",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            OnLogoutRequested(this, EventArgs.Empty);
        }
    }

    private void ShowHome()
    {
        var homeView = new HomeView();
        ContentArea.Content = homeView;
    }

    private void ShowTables()
    {
        var tablesView = new TablesView();
        tablesView.DataContext = new TablesViewModel(_viewModel.CurrentUser);
        ContentArea.Content = tablesView;
    }

    private void ShowProducts()
    {
        var productsView = new ProductView();
        productsView.DataContext = new ProductViewModel(_viewModel.CurrentUser);
        ContentArea.Content = productsView;
    }

    private void ShowReports()
    {
        var reportsView = new ReportView();
        reportsView.DataContext = new ReportViewModel();
        ContentArea.Content = reportsView;
    }

    private void ShowLicense()
    {
        var licenseView = new LicenseView();
        licenseView.DataContext = new LicenseViewModel();
        ContentArea.Content = licenseView;
    }

    private void ShowSupport()
    {
        var supportView = new SupportView();
        supportView.DataContext = new SupportViewModel();
        ContentArea.Content = supportView;
    }

    private void OnLogoutRequested(object? sender, EventArgs e)
    {
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        Close();
    }
}