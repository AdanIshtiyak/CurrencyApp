using CurrencyApp.Entity.Enums;

namespace CurrencyApp.DTOs
{
  public class AppSettingsDto
  {
    public StorageType StorageType { get; set; } = StorageType.Sqlite;
  }
}
