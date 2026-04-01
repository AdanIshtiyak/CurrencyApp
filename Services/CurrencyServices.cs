using CurrencyApp.DTOs;
using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using System.Net.Http;
using System.Text.Json;

namespace CurrencyApp.Services
{
  public class CurrencyServices : ICurrencyListServices, ICurrencyAddServices
  {
    private const string _apiUrl = "https://www.cbr-xml-daily.ru/daily_json.js";

    private readonly HttpClient _httpClient;
    private readonly ICurrencyRepository _repository;

    public CurrencyServices(HttpClient httpClient, ICurrencyRepository repository)
    {
      _httpClient = httpClient;
      _repository = repository;
    }

    public async Task<List<Currency>> FetchCurrencyDataAsync()
    {
      var json = await _httpClient.GetStringAsync(_apiUrl);
      var currencyDataDto = JsonSerializer.Deserialize<CurrencyJsonDto>(json);

      if (currencyDataDto == null)
        return [];

      var changedIds = (await _repository.GetAllIdsAsync(includeDeleted: true)).ToHashSet();

      var newCurrencies = currencyDataDto.Valute
        .Select(c => c.Value)
        .Where(c => !changedIds.Contains(c.Id))
        .Select(c => new Currency
        {
          Id = c.Id,
          CharCode = c.CharCode,
          Nominal = c.Nominal,
          NumCode = c.NumCode,
          Previous = c.Previous,
          Value = c.Value
        })
        .ToList();

      await _repository.DeleteNonCustomNonDeletedAsync();

      if (newCurrencies.Count > 0)
        await _repository.AddRangeAsync(newCurrencies);

      return await _repository.GetAllAsync();
    }

    public Task<List<Currency>> GetCurrenciesAsync() =>
      _repository.GetAllAsync();

    public async Task CreateCustomCurrencyAsync(CurrencyCreateDto createDto)
    {
      var existingIds = await _repository.GetAllIdsAsync();
      if (existingIds.Contains(createDto.Id))
        return;

      await _repository.AddAsync(new Currency
      {
        Id = createDto.Id,
        CharCode = createDto.CharCode,
        Nominal = createDto.Nominal,
        NumCode = createDto.NumCode,
        Value = createDto.Value,
        Previous = createDto.Previous,
        IsCustom = true
      });
    }

    public async Task DeleteCurrencyAsync(int currencyInternalId)
    {
      var currency = await _repository.GetByInternalIdAsync(currencyInternalId);
      if (currency == null)
        return;

      currency.IsDeleted = true;
      await _repository.UpdateAsync(currency);
    }

    public async Task<HashSet<string>> GetAllCurrenciesIdsAsync()
    {
      var ids = await _repository.GetAllIdsAsync();
      return [.. ids];
    }
  }
}
