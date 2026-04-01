using CurrencyApp.DTOs;

namespace CurrencyApp.Interfaces
{
  public interface ICurrencyAddServices
  {
    Task CreateCustomCurrencyAsync(CurrencyCreateDto createDto);
    Task<HashSet<string>> GetAllCurrenciesIdsAsync();
  }
}
