using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using CurrencyApp.ViewModels.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CurrencyApp.ViewModels
{
  public class CurrencyListViewModel : NotifyPropertyChanged
  {
    private readonly ICurrencyListServices _currencyListServices;
    private readonly ISessionServices _sessionService;

    private ObservableCollection<Currency> _currencies = new ObservableCollection<Currency>();

    public ObservableCollection<Currency> Currencies
    {
      get => _currencies;
      set
      {
        _currencies = value;
        OnPropertyChanged();
      }
    }
    public ICommand RefreshCommand { get; }
    public ICommand DeleteCurrencyCommand { get; }

    private bool _isLoading;

    public bool IsLoading
    {
      get => _isLoading;
      set
      {
        _isLoading = value;
        OnPropertyChanged();
      }
    }

    public CurrencyListViewModel(ICurrencyListServices currencyListServices, ISessionServices sessionService)
    {
      _currencyListServices = currencyListServices;
      _sessionService = sessionService;

      IsLoading = false;
      RefreshCommand = new AsyncRelayCommand(RefreshData, _sessionService);
      DeleteCurrencyCommand = new AsyncRelayCommand<Currency>(DeleteCurrency, _sessionService);
    }

    public async Task InitCurrencies()
    {
      var currencies = await _currencyListServices.GetCurrenciesAsync();

      currencies.ForEach(Currencies.Add);
    }

    #region Private methods

    private async Task RefreshData()
    {
      IsLoading = true;

      try
      {
        var currencies = await _currencyListServices.FetchCurrencyDataAsync();

        Currencies.Clear();
        currencies.ForEach(Currencies.Add);
      }
      finally
      {
        IsLoading = false;
      }
    }

    private async Task DeleteCurrency(Currency currency)
    {
      await _currencyListServices.DeleteCurrencyAsync(currency.InternalId);

      Currencies.Remove(currency);
    }

    #endregion
  }
}
