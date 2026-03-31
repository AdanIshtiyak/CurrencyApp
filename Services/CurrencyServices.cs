using CurrencyApp.DTOs;
using CurrencyApp.Entity;
using CurrencyApp.Entity.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;

namespace CurrencyApp.Services
{
  public class CurrencyServices
  {
    private const string _apiUrl = "https://www.cbr-xml-daily.ru/daily_json.js";

    private readonly HttpClient _httpClient;

    private readonly AppDbContext _context;

    public CurrencyServices(AppDbContext context, HttpClient httpClient)
    {
      _context = context;
      _httpClient = httpClient;
    }

    public async Task<List<Currency>> FetchCurrencyDataAsync()
    {
      var json = await _httpClient.GetStringAsync(_apiUrl);
      var currencyDataDto = JsonSerializer.Deserialize<CurrencyJsonDto>(json);

      if (currencyDataDto == null)
        return [];

      var currencies = _context.Currencies;

      var changedCurrenciesIds = currencies
        .Where(c => c.IsDeleted || c.IsCustom)
        .Select(c => c.Id)
        .ToList();

      var currenciesForUpdate = currencies
        .Where(c => !c.IsCustom && !c.IsDeleted)
        .ToList();

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

      _context.Currencies.RemoveRange(currenciesForUpdate);
      _context.Currencies.AddRange(newCurrencies);
      await _context.SaveChangesAsync();

      var currenciesToDisplay = currencies.Where(c => !c.IsDeleted).ToList();

      return currenciesToDisplay;
    }

    public async Task CreateCustomCurrency(CurrencyCreateDto createDto)
    {
      var currenciesIds = await _context.Currencies
        .Where(c => !c.IsDeleted)
        .Select(c => c.Id)
        .AsNoTracking()
        .ToListAsync();

      if (currenciesIds.Any(c => c == createDto.Id))
        return; //todo: Сделать вывод ошибки

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

      _context.Currencies.Add(newCurrency);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteCurrency(int currencyInternalId)
    {
      var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.InternalId == currencyInternalId);

      if (currency == null)
        return;

      currency.IsDeleted = true;

      _context.Currencies.Update(currency);
      await _context.SaveChangesAsync();
    }
  }
}
