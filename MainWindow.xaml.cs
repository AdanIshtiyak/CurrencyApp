using CurrencyApp.ViewModels;
using CurrencyApp.Views;
using System.Windows;

namespace CurrencyApp
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      // Open Currencies page by default on startup
      MainFrame.Navigate(new CurrencyListPage());
    }

    private void BtnCurrencies_Click(object sender, RoutedEventArgs e)
      => MainFrame.Navigate(new CurrencyListPage());

    private void BtnAddCurrency_Click(object sender, RoutedEventArgs e)
      => MainFrame.Navigate(new AddCurrencyPage());

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
      => MainFrame.Navigate(new SettingsPage());
  }
}