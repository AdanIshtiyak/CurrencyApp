using CurrencyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CurrencyApp.Views
{
  public partial class CurrencyListPage : Page
  {
    public CurrencyListPage()
    {
      InitializeComponent();
      DataContext = App.ServiceProvider.GetRequiredService<CurrencyListViewModel>();
    }
  }
}