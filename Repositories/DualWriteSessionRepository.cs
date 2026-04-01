using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using CurrencyApp.Repositories.Json;
using CurrencyApp.Repositories.Sqlite;

namespace CurrencyApp.Repositories
{
  public class DualWriteSessionRepository : ISessionRepository
  {
    private readonly SqliteSessionRepository _sqlite;
    private readonly JsonSessionRepository _json;

    public DualWriteSessionRepository(SqliteSessionRepository sqlite, JsonSessionRepository json)
    {
      _sqlite = sqlite;
      _json = json;
    }

    public Session? GetLastOtherSession(Guid excludeGid) =>
      _sqlite.GetLastOtherSession(excludeGid);

    public Task<Session?> FindByGidAsync(Guid gid) =>
      _sqlite.FindByGidAsync(gid);

    public async Task AddOrUpdateAsync(Session session)
    {
      await _sqlite.AddOrUpdateAsync(session);
      await _json.AddOrUpdateAsync(session);
    }
  }
}
