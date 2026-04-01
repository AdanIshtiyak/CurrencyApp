using CurrencyApp.DTOs;
using CurrencyApp.Entity.Enums;

namespace CurrencyApp.Interfaces
{
  public interface ILocalSettingServices
  {
    StorageType LoadSettings();
    void SaveSettings(AppSettingsDto settings);
  }
}
