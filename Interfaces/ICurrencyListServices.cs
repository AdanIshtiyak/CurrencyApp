using CurrencyApp.Entity.Models;
using System.Collections.ObjectModel;

namespace CurrencyApp.Interfaces
{
  public interface ICurrencyListServices
  {
    Task<List<Currency>> FetchCurrencyDataAsync();
    Task DeleteCurrencyAsync(int currencyInternalId);
    Task<List<Currency>> GetCurrenciesAsync();
  }
}
