using CurrencyApp.DTOs;
using CurrencyApp.Entity.Enums;
using CurrencyApp.Interfaces;
using System.IO;
using System.Text.Json;

namespace CurrencyApp.Services;

public class LocalSettingsServices : ILocalSettingServices
{
  private readonly string _settingsFileName = "settings.json";
  private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

  public StorageType LoadSettings()
  {
    if (!File.Exists(_settingsFileName))
      return StorageType.Sqlite;

    try
    {
      var json = File.ReadAllText(_settingsFileName);
      return JsonSerializer.Deserialize<AppSettingsDto>(json)?.StorageType ?? StorageType.Sqlite;
    }
    catch
    {
      return StorageType.Sqlite;
    }
  }

  public void SaveSettings(AppSettingsDto settings)
  {
    var json = JsonSerializer.Serialize(settings, JsonOptions);
    File.WriteAllText(_settingsFileName, json);
  }
}
