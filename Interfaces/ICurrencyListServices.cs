using CurrencyApp.Entity.Models;

namespace CurrencyApp.Interfaces
{
  public interface ICurrencyListServices
  {
    Task<List<Currency>> FetchCurrencyDataAsync();
    Task DeleteCurrencyAsync(int currencyInternalId);
    Task<List<Currency>> GetCurrenciesAsync();
  }
}
