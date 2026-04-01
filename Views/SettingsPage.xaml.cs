using CurrencyApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CurrencyApp.Views
{
  public partial class SettingsPage : Page
  {
    public SettingsPage()
    {
      InitializeComponent();
      DataContext = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
    }
  }
}