using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using System.IO;
using System.Text.Json;

namespace CurrencyApp.Repositories.Json
{
  public class JsonCurrencyRepository : ICurrencyRepository
  {
    private const string _fileName = "currencies_data.json";
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    private async Task<CurrencyStorage> ReadStorageAsync()
    {
      if (!File.Exists(_fileName))
        return new CurrencyStorage();

      var json = await File.ReadAllTextAsync(_fileName);
      return JsonSerializer.Deserialize<CurrencyStorage>(json) ?? new CurrencyStorage();
    }

    private async Task WriteStorageAsync(CurrencyStorage storage)
    {
      var json = JsonSerializer.Serialize(storage, _jsonOptions);
      await File.WriteAllTextAsync(_fileName, json);
    }

    public async Task<List<Currency>> GetAllAsync(bool includeDeleted = false)
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        return includeDeleted
          ? storage.Currencies
          : storage.Currencies.Where(c => !c.IsDeleted).ToList();
      }
      finally { _lock.Release(); }
    }

    public async Task<List<string>> GetAllIdsAsync(bool includeDeleted = false)
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        var list = includeDeleted ? storage.Currencies : storage.Currencies.Where(c => !c.IsDeleted).ToList();
        return list.Select(c => c.Id).ToList();
      }
      finally { _lock.Release(); }
    }

    public async Task<Currency?> GetByInternalIdAsync(int internalId)
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        return storage.Currencies.FirstOrDefault(c => c.InternalId == internalId);
      }
      finally { _lock.Release(); }
    }

    public async Task AddAsync(Currency currency)
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        currency.InternalId = storage.NextId++;
        storage.Currencies.Add(currency);
        await WriteStorageAsync(storage);
      }
      finally { _lock.Release(); }
    }

    public async Task AddRangeAsync(IEnumerable<Currency> currencies)
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        foreach (var currency in currencies)
        {
          currency.InternalId = storage.NextId++;
          storage.Currencies.Add(currency);
        }
        await WriteStorageAsync(storage);
      }
      finally { _lock.Release(); }
    }

    public async Task UpdateAsync(Currency currency)
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        var index = storage.Currencies.FindIndex(c => c.InternalId == currency.InternalId);
        if (index >= 0)
          storage.Currencies[index] = currency;
        await WriteStorageAsync(storage);
      }
      finally { _lock.Release(); }
    }

    public async Task DeleteNonCustomNonDeletedAsync()
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        storage.Currencies.RemoveAll(c => !c.IsCustom && !c.IsDeleted);
        await WriteStorageAsync(storage);
      }
      finally { _lock.Release(); }
    }

    private class CurrencyStorage
    {
      public int NextId { get; set; } = 1;
      public List<Currency> Currencies { get; set; } = [];
    }
  }
}
