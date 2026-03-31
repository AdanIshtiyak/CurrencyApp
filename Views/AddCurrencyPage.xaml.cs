using CurrencyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CurrencyApp.Views
{
  public partial class AddCurrencyPage : Page
  {
    public AddCurrencyPage()
    {
      InitializeComponent();
      DataContext = App.ServiceProvider.GetRequiredService<AddCurrencyViewModel>();
    }
  }
}