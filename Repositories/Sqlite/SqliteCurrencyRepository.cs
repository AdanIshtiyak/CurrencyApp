using CurrencyApp.Entity;
using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyApp.Repositories.Sqlite
{
  public class SqliteCurrencyRepository : ICurrencyRepository
  {
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SqliteCurrencyRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
      _contextFactory = contextFactory;
    }

    public async Task<List<Currency>> GetAllAsync(bool includeDeleted = false)
    {
      using var context = await _contextFactory.CreateDbContextAsync();
      var query = context.Currencies.AsNoTracking();
      if (!includeDeleted)
        query = query.Where(c => !c.IsDeleted);
      return await query.ToListAsync();
    }

    public async Task<List<string>> GetAllIdsAsync(bool includeDeleted = false)
    {
      using var context = await _contextFactory.CreateDbContextAsync();
      var query = context.Currencies.AsNoTracking();
      if (!includeDeleted)
        query = query.Where(c => !c.IsDeleted);
      return await query.Select(c => c.Id).ToListAsync();
    }

    public async Task<Currency?> GetByInternalIdAsync(int internalId)
    {
      using var context = await _contextFactory.CreateDbContextAsync();
      return await context.Currencies.FirstOrDefaultAsync(c => c.InternalId == internalId);
    }

    public async Task AddAsync(Currency currency)
    {
      using var context = await _contextFactory.CreateDbContextAsync();
      context.Currencies.Add(currency);
      await context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Currency> currencies)
    {
      using var context = await _contextFactory.CreateDbContextAsync();
      context.Currencies.AddRange(currencies);
      await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Currency currency)
    {
      using var context = await _contextFactory.CreateDbContextAsync();
      context.Currencies.Update(currency);
      await context.SaveChangesAsync();
    }

    public async Task DeleteNonCustomNonDeletedAsync()
    {
      using var context = await _contextFactory.CreateDbContextAsync();
      await context.Currencies
        .Where(c => !c.IsCustom && !c.IsDeleted)
        .ExecuteDeleteAsync();
    }
  }
}
