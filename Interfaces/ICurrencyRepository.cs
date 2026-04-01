using CurrencyApp.Entity.Models;

namespace CurrencyApp.Interfaces
{
  public interface ICurrencyRepository
  {
    Task<List<Currency>> GetAllAsync(bool includeDeleted = false);
    Task<List<string>> GetAllIdsAsync(bool includeDeleted = false);
    Task<Currency?> GetByInternalIdAsync(int internalId);
    Task AddAsync(Currency currency);
    Task AddRangeAsync(IEnumerable<Currency> currencies);
    Task UpdateAsync(Currency currency);
    Task DeleteNonCustomNonDeletedAsync();
  }
}
