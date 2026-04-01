using CurrencyApp.DTOs;
using CurrencyApp.Entity.Enums;
using CurrencyApp.Interfaces;
using CurrencyApp.ViewModels.Helper;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace CurrencyApp.ViewModels
{
  public class SettingsViewModel : NotifyPropertyChanged
  {
    #region View bindings

    public string LastSessionDateTime
    {
      get => _lastSessionDateTime;
      set
      {
        _lastSessionDateTime = value;
        OnPropertyChanged();
      }
    }
    public bool IsSqliteStorage
    {
      get => _iIsSqliteStorage;
      set
      {
        _iIsSqliteStorage = value;
        OnPropertyChanged();

        if (!_isInternalUpdate && _isDataLoaded)
        {
          _isInternalUpdate = true;
          IsJsonStorage = !IsSqliteStorage;
        }
        else
          _isInternalUpdate = false;
      }
    }
    public bool IsJsonStorage
    {
      get => _isJsonStorage;
      set
      {
        _isJsonStorage = value;
        OnPropertyChanged();

        if (!_isInternalUpdate && _isDataLoaded)
        {
          _isInternalUpdate = true;
          IsSqliteStorage = !IsJsonStorage;
        }
        else
          _isInternalUpdate = false;
      }
    }
    public ICommand ApplyStorageCommand { get; set; }

    private string _lastSessionDateTime;

    #endregion

    #region Helper class fields

    private bool _iIsSqliteStorage;
    private bool _isJsonStorage;
    private bool _isInternalUpdate;
    private bool _isDataLoaded = false;

    #endregion

    #region Ctor + DI

    private readonly ISessionServices _sessionService;
    private readonly ILocalSettingServices _localSettingServices;

    public SettingsViewModel(ISessionServices sessionServices, ILocalSettingServices localSettingServices)
    {
      _sessionService = sessionServices;
      _localSettingServices = localSettingServices;

      LastSessionDateTime = _sessionService.GetLastActivityAt();
      LoadSettings();
      _isDataLoaded = true;

      ApplyStorageCommand = new RelayCommand(ApplySettingChanges);
    }

    #endregion

    #region Private methods

    private void ApplySettingChanges()
    {
      _localSettingServices.SaveSettings(new AppSettingsDto()
      {
        StorageType = IsSqliteStorage ? StorageType.Sqlite : StorageType.Json
      });
    }

    private void LoadSettings()
    {
      var storageType = _localSettingServices.LoadSettings();
      IsSqliteStorage = storageType == StorageType.Sqlite;
      IsJsonStorage = storageType == StorageType.Json;
    }

    #endregion
  }
}
