using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using System.IO;
using System.Text.Json;

namespace CurrencyApp.Repositories.Json
{
  public class JsonSessionRepository : ISessionRepository
  {
    private const string _fileName = "sessions_data.json";
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    private async Task<SessionStorage> ReadStorageAsync()
    {
      if (!File.Exists(_fileName))
        return new SessionStorage();

      var json = await File.ReadAllTextAsync(_fileName);
      return JsonSerializer.Deserialize<SessionStorage>(json) ?? new SessionStorage();
    }

    private async Task WriteStorageAsync(SessionStorage storage)
    {
      var json = JsonSerializer.Serialize(storage, _jsonOptions);
      await File.WriteAllTextAsync(_fileName, json);
    }

    public Session? GetLastOtherSession(Guid excludeGid)
    {
      _lock.Wait();
      try
      {
        var storage = ReadStorageAsync().GetAwaiter().GetResult();
        return storage.Sessions
          .Where(s => s.Gid != excludeGid)
          .OrderByDescending(s => s.LastActivityAt)
          .FirstOrDefault();
      }
      finally { _lock.Release(); }
    }

    public async Task<Session?> FindByGidAsync(Guid gid)
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        return storage.Sessions.FirstOrDefault(s => s.Gid == gid);
      }
      finally { _lock.Release(); }
    }

    public async Task AddOrUpdateAsync(Session session)
    {
      await _lock.WaitAsync();
      try
      {
        var storage = await ReadStorageAsync();
        var existing = storage.Sessions.FirstOrDefault(s => s.Gid == session.Gid);
        if (existing == null)
          storage.Sessions.Add(session);
        else
          existing.LastActivityAt = session.LastActivityAt;
        await WriteStorageAsync(storage);
      }
      finally { _lock.Release(); }
    }

    private class SessionStorage
    {
      public List<Session> Sessions { get; set; } = [];
    }
  }
}
