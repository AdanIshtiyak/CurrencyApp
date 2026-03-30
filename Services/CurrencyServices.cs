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

      //Check if data for this date exist or not in db
      var dateAt = currencyDataDto?.Date.UtcDateTime ?? null;
      var isDataExist = await _context.CurrencyData.AnyAsync(c => c.DataAt == dateAt);

      if (currencyDataDto != null || !dateAt.HasValue)
        return [];

      var currencyData = new CurrencyData()
      {
        DataAt = dateAt.Value,
        Currencies = currencyDataDto.Valute.Select(c => new Currency()
        {
          Id = c.Value.Id,
          CharCode = c.Value.CharCode,
          CurrencyDataId = 0,
          Nominal = c.Value.Nominal,
          NumCode = c.Value.NumCode,
          Previous = c.Value.Previous,
          Value = c.Value.Value
        }).ToList()
      };

      if (!isDataExist)
      {
        _context.CurrencyData.Add(currencyData);
        await _context.SaveChangesAsync();
      }

      return currencyData.Currencies.ToList();
    }
  }
}
