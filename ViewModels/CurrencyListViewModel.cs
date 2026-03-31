using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using CurrencyApp.Services;
using CurrencyApp.ViewModels.Helper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CurrencyApp.ViewModels
{
  public class CurrencyListViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly ICurrencyListServices _currencyListServices;

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

    public CurrencyListViewModel(ICurrencyListServices currencyListServices)
    {
      _currencyListServices = currencyListServices;

      IsLoading = false;
      RefreshCommand = new AsyncRelayCommand(RefreshData);
      DeleteCurrencyCommand = new AsyncRelayCommand<Currency>(DeleteCurrency);
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
