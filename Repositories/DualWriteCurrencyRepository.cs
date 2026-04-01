using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using CurrencyApp.Repositories.Json;
using CurrencyApp.Repositories.Sqlite;

namespace CurrencyApp.Repositories
{
  public class DualWriteCurrencyRepository : ICurrencyRepository
  {
    private readonly SqliteCurrencyRepository _sqlite;
    private readonly JsonCurrencyRepository _json;

    public DualWriteCurrencyRepository(SqliteCurrencyRepository sqlite, JsonCurrencyRepository json)
    {
      _sqlite = sqlite;
      _json = json;
    }

    public Task<List<Currency>> GetAllAsync(bool includeDeleted = false) =>
      _sqlite.GetAllAsync(includeDeleted);

    public Task<List<string>> GetAllIdsAsync(bool includeDeleted = false) =>
      _sqlite.GetAllIdsAsync(includeDeleted);

    public Task<Currency?> GetByInternalIdAsync(int internalId) =>
      _sqlite.GetByInternalIdAsync(internalId);

    public async Task AddAsync(Currency currency)
    {
      await _sqlite.AddAsync(currency);
      await _json.AddAsync(CloneWithoutId(currency));
    }

    public async Task AddRangeAsync(IEnumerable<Currency> currencies)
    {
      var list = currencies.ToList();
      await _sqlite.AddRangeAsync(list);
      await _json.AddRangeAsync(list.Select(CloneWithoutId));
    }

    public async Task UpdateAsync(Currency currency)
    {
      await _sqlite.UpdateAsync(currency);

      var jsonAll = await _json.GetAllAsync(includeDeleted: true);
      var jsonMatch = jsonAll.FirstOrDefault(c => c.Id == currency.Id);
      if (jsonMatch != null)
      {
        jsonMatch.CharCode = currency.CharCode;
        jsonMatch.NumCode = currency.NumCode;
        jsonMatch.Nominal = currency.Nominal;
        jsonMatch.Value = currency.Value;
        jsonMatch.Previous = currency.Previous;
        jsonMatch.IsCustom = currency.IsCustom;
        jsonMatch.IsDeleted = currency.IsDeleted;
        await _json.UpdateAsync(jsonMatch);
      }
    }

    public async Task DeleteNonCustomNonDeletedAsync()
    {
      await _sqlite.DeleteNonCustomNonDeletedAsync();
      await _json.DeleteNonCustomNonDeletedAsync();
    }

    // JSON repo manages its own auto-increment InternalId
    private static Currency CloneWithoutId(Currency c) => new()
    {
      Id = c.Id,
      NumCode = c.NumCode,
      CharCode = c.CharCode,
      Nominal = c.Nominal,
      Value = c.Value,
      Previous = c.Previous,
      IsCustom = c.IsCustom,
      IsDeleted = c.IsDeleted
    };
  }
}
