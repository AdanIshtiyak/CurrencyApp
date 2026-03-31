using CurrencyApp.Entity.Models;
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

    private readonly CurrencyServices _currencyServices;

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

    public CurrencyListViewModel(CurrencyServices currencyServices)
    {

      _currencyServices = currencyServices;

      IsLoading = false;
      RefreshCommand = new AsyncRelayCommand(RefreshData);
      DeleteCurrencyCommand = new AsyncRelayCommand<Currency>(DeleteCurrency);
    }

    public async Task RefreshData()
    {
      IsLoading = true;

      try
      {
        var currencies = await _currencyServices.FetchCurrencyDataAsync();

        Currencies.Clear();
        currencies.ForEach(Currencies.Add);
      }
      finally
      {
        IsLoading = false;
      }
    }

    public async Task DeleteCurrency(Currency currency)
    {
      await _currencyServices.DeleteCurrency(currency.InternalId);

      Currencies.Remove(currency);
    }
  }
}
