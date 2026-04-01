using CurrencyApp.DTOs;
using CurrencyApp.Entity;
using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;

namespace CurrencyApp.Services
{
  public class CurrencyServices : ICurrencyListServices, ICurrencyAddServices
  {
    private const string _apiUrl = "https://www.cbr-xml-daily.ru/daily_json.js";

    private readonly HttpClient _httpClient;

    private readonly IDbContextFactory<AppDbContext> _context;

    public CurrencyServices(HttpClient httpClient, IDbContextFactory<AppDbContext> context)
    {
      _httpClient = httpClient;
      _context = context;
    }

    public async Task<List<Currency>> FetchCurrencyDataAsync()
    {
      var json = await _httpClient.GetStringAsync(_apiUrl);
      var currencyDataDto = JsonSerializer.Deserialize<CurrencyJsonDto>(json);

      if (currencyDataDto == null)
        return [];

      using var context = await _context.CreateDbContextAsync();

      var changedCurrenciesIds = await context.Currencies
        .Where(c => c.IsDeleted || c.IsCustom)
        .Select(c => c.Id)
        .ToListAsync();

      var currenciesForUpdate = await context.Currencies
        .Where(c => !c.IsCustom && !c.IsDeleted)
        .ToListAsync();

      var newCurrencies = currencyDataDto.Valute
        .Select(c => c.Value)
        .Where(c => !changedCurrenciesIds.Contains(c.Id))
        .Select(c => new Currency()
        {
          Id = c.Id,
          CharCode = c.CharCode,
          Nominal = c.Nominal,
          NumCode = c.NumCode,
          Previous = c.Previous,
          Value = c.Value
        }).ToList();

      await context.Currencies
        .Where(c => !c.IsCustom && !c.IsDeleted)
        .ExecuteDeleteAsync();

      context.Currencies.AddRange(newCurrencies);
      await context.SaveChangesAsync();

      var currenciesToDisplay = await context.Currencies
        .Where(c => !c.IsDeleted)
        .ToListAsync();

      return currenciesToDisplay;
    }

    public async Task<List<Currency>> GetCurrenciesAsync()
    {
      using var context = await _context.CreateDbContextAsync();

      var currencies = await context.Currencies
        .Where(c => !c.IsDeleted)
        .AsNoTracking()
        .ToListAsync();

      return currencies;
    }

    public async Task CreateCustomCurrencyAsync(CurrencyCreateDto createDto)
    {
      using var context = await _context.CreateDbContextAsync();

      var currenciesIds = await context.Currencies
        .Where(c => !c.IsDeleted)
        .Select(c => c.Id)
        .AsNoTracking()
        .ToListAsync();

      if (currenciesIds.Any(c => c == createDto.Id))
        return;

      var newCurrency = new Currency()
      {
        Id = createDto.Id,
        CharCode = createDto.CharCode,
        Nominal = createDto.Nominal,
        NumCode = createDto.NumCode,
        Value = createDto.Value,
        Previous = createDto.Previous,
        IsCustom = true
      };

      context.Currencies.Add(newCurrency);
      await context.SaveChangesAsync();
    }

    public async Task DeleteCurrencyAsync(int currencyInternalId)
    {
      using var context = await _context.CreateDbContextAsync();

      var currency = await context.Currencies.FirstOrDefaultAsync(c => c.InternalId == currencyInternalId);

      if (currency == null)
        return;

      currency.IsDeleted = true;

      context.Currencies.Update(currency);
      await context.SaveChangesAsync();
    }

    public async Task<HashSet<string>> GetAllCurrenciesIdsAsync()
    {
      using var context = await _context.CreateDbContextAsync();

      var hashSet = (await context.Currencies
        .Where(c => !c.IsDeleted)
        .Select(c => c.Id)
        .ToListAsync())
        .ToHashSet();

      return hashSet;
    }
  }
}
