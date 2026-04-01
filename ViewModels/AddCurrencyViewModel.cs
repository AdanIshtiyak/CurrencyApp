using CurrencyApp.DTOs;
using CurrencyApp.Interfaces;
using CurrencyApp.ViewModels.Helper;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace CurrencyApp.ViewModels
{
  public class AddCurrencyViewModel : NotifyPropertyChanged
  {
    #region View bindings

    public string CharCode
    {
      get => _charCode;
      set
      {
        _charCode = value;
        OnPropertyChanged();
      }
    }
    public string NumCode
    {
      get => _numCode;
      set
      {
        _numCode = value;
        OnPropertyChanged();
      }
    }
    public string Id
    {
      get => _id;
      set
      {
        _id = value;
        OnPropertyChanged();
      }
    }
    public string Nominal
    {
      get => _nominal;
      set
      {
        _nominal = value;
        OnPropertyChanged();
      }
    }
    public string Value
    {
      get => _value;
      set
      {
        _value = value;
        OnPropertyChanged();
      }
    }
    public string Previous
    {
      get => _previous;
      set
      {
        _previous = value;
        OnPropertyChanged();
      }
    }
    public bool HasValidationError
    {
      get => _hasValidadionError;
      set
      {
        _hasValidadionError = value;
        OnPropertyChanged();
      }
    }
    public string ValidationMessage
    {
      get => _validationMessage;
      set
      {
        _validationMessage = value;
        OnPropertyChanged();
      }
    }

    public ICommand AddCurrencyCommand { get; set; }
    public ICommand ClearFormCommand { get; set; }

    #endregion

    #region Helper class fields

    private string _charCode;
    private string _numCode;
    private string _id;
    private string _nominal;
    private string _value;
    private string _previous;
    private bool _hasValidadionError;
    private string _validationMessage;
    private HashSet<string> _existingIds = new();

    #endregion

    private readonly ICurrencyAddServices _currencyAddServices;
    private readonly ISessionServices _sessionService;

    public AddCurrencyViewModel(ICurrencyAddServices currencyAddServices, ISessionServices sessionService)
    {
      _currencyAddServices = currencyAddServices;
      _sessionService = sessionService;

      ClearFormCommand = new RelayCommand(ClearForm);
      AddCurrencyCommand = new AsyncRelayCommand(AddCurrency, _sessionService);
    }

    public async Task LoadAllExistingCurrencyIds()
    {
      if (_existingIds.Any())
        return;

      _existingIds = await _currencyAddServices.GetAllCurrenciesIdsAsync();
    }

    #region Private methods

    private async Task AddCurrency()
    {
      if (CanAddCurrency())
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
    }

    private bool CanAddCurrency()
    {
      var isValidFields = !string.IsNullOrEmpty(Value) && decimal.TryParse(Value, out _);
      isValidFields = isValidFields && !string.IsNullOrEmpty(Nominal) && int.TryParse(Nominal, out _);
      isValidFields = isValidFields && (string.IsNullOrEmpty(Previous) || decimal.TryParse(Previous, out _));

      var canAdd = new List<string>() { CharCode, NumCode, Id }.All(field => !string.IsNullOrWhiteSpace(field));

      var isSomethingWrong = !(canAdd && isValidFields && !_existingIds.Contains(Id));

      HasValidationError = isSomethingWrong;

      if (isSomethingWrong)
        ValidationMessage = "Some field contains wrong data or Id is duplicated";

      return isSomethingWrong;
    }

    #endregion
  }
}
