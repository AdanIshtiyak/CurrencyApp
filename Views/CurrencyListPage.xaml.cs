using CurrencyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace CurrencyApp.Views
{
  public partial class CurrencyListPage : Page
  {
    public CurrencyListPage()
    {
      InitializeComponent();
      DataContext = App.ServiceProvider.GetRequiredService<CurrencyListViewModel>();

      Loaded += CurrencyListPage_Loaded;
    }

    private async void CurrencyListPage_Loaded(object sender, RoutedEventArgs e)
    {
      if (DataContext is CurrencyListViewModel viewModel)
      {
        await viewModel.InitCurrencies();
      }
    }
  }
}