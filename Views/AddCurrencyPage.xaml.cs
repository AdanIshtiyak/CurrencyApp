using CurrencyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace CurrencyApp.Views
{
  public partial class AddCurrencyPage : Page
  {
    public AddCurrencyPage()
    {
      InitializeComponent();

      DataContext = App.ServiceProvider.GetRequiredService<AddCurrencyViewModel>();

      Loaded += AddCurrencyPage_Loaded;
    }

    private async void AddCurrencyPage_Loaded(object sender, RoutedEventArgs e)
    {
      if (DataContext is AddCurrencyViewModel vm)
      {
        await vm.LoadAllExistingCurrencyIds();
      }
    }
  }
}