using CurrencyApp.Entity;
using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyApp.Repositories.Sqlite
{
  public class SqliteSessionRepository : ISessionRepository
  {
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SqliteSessionRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
      _contextFactory = contextFactory;
    }

    public Session? GetLastOtherSession(Guid excludeGid)
    {
      using var context = _contextFactory.CreateDbContext();
      return context.Sessions
        .Where(s => s.Gid != excludeGid)
        .OrderByDescending(s => s.LastActivityAt)
        .FirstOrDefault();
    }

    public async Task<Session?> FindByGidAsync(Guid gid)
    {
      using var context = _contextFactory.CreateDbContext();
      return await context.Sessions.FindAsync(gid);
    }

    public async Task AddOrUpdateAsync(Session session)
    {
      using var context = _contextFactory.CreateDbContext();
      var existing = await context.Sessions.FindAsync(session.Gid);
      if (existing == null)
        context.Sessions.Add(session);
      else
      {
        existing.LastActivityAt = session.LastActivityAt;
        context.Sessions.Update(existing);
      }
      await context.SaveChangesAsync();
    }
  }
}
