using CurrencyApp.DTOs;
using CurrencyApp.Interfaces;
using CurrencyApp.ViewModels.Helper;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CurrencyApp.ViewModels
{
  public class AddCurrencyViewModel : INotifyPropertyChanged
  {
    #region INotifyPropertyChanged implementation

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region View bindings

    public string CharCode { get; set; }
    public string NumCode { get; set; }
    public string Id
    {
      get => _id;
      set
      {
        _id = value;
        OnPropertyChanged();
        CanAddCurrency(Id);
      }
    }
    public string Nominal { get; set; }
    public string Value { get; set; }
    public string Previous { get; set; }
    public string ValidationMessage { get; set; }
    public bool CanAdd
    {
      get => _canAdd;
      set
      {
        _canAdd = value;
        OnPropertyChanged();
      }
    }

    public ICommand AddCurrencyCommand { get; set; }
    public ICommand ClearFormCommand { get; set; }

    #endregion

    #region Helper class fields

    private string _id;
    private bool _canAdd;
    private HashSet<string> _existingIds = new();

    #endregion

    private readonly ICurrencyAddServices _currencyAddServices;

    public AddCurrencyViewModel(ICurrencyAddServices currencyAddServices)
    {
      _currencyAddServices = currencyAddServices;

      ClearFormCommand = new RelayCommand(ClearForm);
      AddCurrencyCommand = new AsyncRelayCommand(AddCurrency);
    }

    public async Task LoadAllExistingCurrencyIds()
    {
      if (!_existingIds.Any())
        return;

      _existingIds = await _currencyAddServices.GetAllCurrenciesIdsAsync();
    }

    #region Private methods

    private async Task AddCurrency()
    {
      if (!CanAdd)
        return;

      var newCurrency = new CurrencyCreateDto()
      {
        Id = Id,
        CharCode = CharCode,
        Nominal = int.TryParse(Nominal, out var nominal) ? nominal : 0,
        NumCode = NumCode,
        Previous = decimal.TryParse(Previous, out var previous) ? previous : 0,
        Value = decimal.TryParse(Value, out var value) ? value : 0
      };
      await _currencyAddServices.CreateCustomCurrencyAsync(newCurrency);
      _existingIds.Add(Id);
      ClearForm();
    }

    private void ClearForm()
    {
      CharCode = string.Empty;
      NumCode = string.Empty;
      Id = string.Empty;
      Nominal = string.Empty;
      Value = string.Empty;
      Previous = string.Empty;
      ValidationMessage = string.Empty;
    }

    private void CanAddCurrency(string Id)
    {
      CanAdd = string.IsNullOrWhiteSpace(Id) || !_existingIds.Contains(Id);
    }

    #endregion
  }
}
