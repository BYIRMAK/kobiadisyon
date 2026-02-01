using System.Windows;
using System.IO;
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

    private void ProductManagementButton_Click(object sender, RoutedEventArgs e)
    {
        ShowProductManagement();
    }

    private void TableManagementButton_Click(object sender, RoutedEventArgs e)
    {
        ShowTableManagement();
    }

    private void ZoneManagementButton_Click(object sender, RoutedEventArgs e)
    {
        ShowZoneManagement();
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
        var tablesViewModel = new TablesViewModel(_viewModel.CurrentUser);
        tablesViewModel.TableSelected += OnTableSelected;
        tablesView.DataContext = tablesViewModel;
        ContentArea.Content = tablesView;
    }

    private void OnTableSelected(object? sender, Table table)
    {
        var orderView = new OrderView();
        var orderViewModel = new OrderViewModel(_viewModel.CurrentUser, table);
        orderViewModel.BackRequested += (s, e) => ShowTables();
        orderViewModel.OrderSaved += (s, e) => ShowTables();
        orderViewModel.CheckoutRequested += (s, e) => ShowCheckout(orderViewModel, table);
        orderView.DataContext = orderViewModel;
        ContentArea.Content = orderView;
    }

    private void ShowCheckout(OrderViewModel orderViewModel, Table table)
    {
        if (orderViewModel.CurrentOrder == null)
        {
            MessageBox.Show("Sipariş bulunamadı.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var checkoutView = new CheckoutView();
        var checkoutViewModel = new CheckoutViewModel(
            _viewModel.CurrentUser, 
            table, 
            orderViewModel.CurrentOrder, 
            orderViewModel.CurrentOrderItems);
        
        checkoutViewModel.BackRequested += (s, e) => OnTableSelected(null, table);
        checkoutViewModel.PaymentCompleted += (s, e) => ShowTables();
        checkoutViewModel.ReceiptRequested += OnReceiptRequested;
        
        checkoutView.DataContext = checkoutViewModel;
        ContentArea.Content = checkoutView;
    }

    private void OnReceiptRequested(object? sender, string receipt)
    {
        try
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Receipts");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filename = Path.Combine(directory, $"Adisyon_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(filename, receipt);

            MessageBox.Show(
                $"Adisyon kaydedildi:\n{filename}", 
                "Başarılı", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);

            // Open the file with the system's default application for .txt files
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = filename,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Adisyon kaydedilirken hata oluştu: {ex.Message}", 
                "Hata", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }
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

    private void ShowProductManagement()
    {
        var productManagementView = new ProductManagementView();
        productManagementView.DataContext = new ProductManagementViewModel();
        ContentArea.Content = productManagementView;
    }

    private void ShowTableManagement()
    {
        var tableManagementView = new TableManagementView();
        tableManagementView.DataContext = new TableManagementViewModel();
        ContentArea.Content = tableManagementView;
    }

    private void ShowZoneManagement()
    {
        var zoneManagementView = new ZoneManagementView();
        zoneManagementView.DataContext = new ZoneManagementViewModel();
        ContentArea.Content = zoneManagementView;
    }

    private void OnLogoutRequested(object? sender, EventArgs e)
    {
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        Close();
    }
}