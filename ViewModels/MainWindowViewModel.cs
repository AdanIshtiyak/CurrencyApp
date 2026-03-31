using CurrencyApp.Entity.Models;
using CurrencyApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CurrencyApp.ViewModels
{
  public class MainWindowViewModel
  {
    private readonly CurrencyServices _currencyServices;
    public ObservableCollection<Currency> Currencies { get; set; }
    public ICommand LoadCurrencyComand { get; }

    public MainWindowViewModel(CurrencyServices currencyServices)
    {
      _currencyServices = currencyServices;

      LoadCurrencyComand = new AsyncRelayCommand(LoadCurrencyData);
    }

    public async Task LoadCurrencyData()
    {
      var currencies = await _currencyServices.FetchCurrencyDataAsync();

      Currencies = new ObservableCollection<Currency>(currencies);
    }
  }
}
