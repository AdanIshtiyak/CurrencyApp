using CurrencyApp.Entity;
using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyApp.Services
{
  public class SessionService : ISessionServices
  {
    private readonly IDbContextFactory<AppDbContext> _context;
    private readonly Guid _sessionGid = Guid.NewGuid();

    public SessionService(IDbContextFactory<AppDbContext> context)
    {
      _context = context;
    }

    public string GetLastActivityAt()
    {
      using var context = _context.CreateDbContext();

      var offsetHours = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours;

      var result = context.Sessions
        .Where(c => c.Gid != _sessionGid)
        .OrderByDescending(c => c.LastActivityAt)
        .FirstOrDefault()
        ?.LastActivityAt.AddHours(offsetHours).ToString("MM/dd/yyyy HH:mm") ?? string.Empty;

      return result;
    }

    public async Task UpdateLastActivityAt()
    {
      using var context = _context.CreateDbContext();

      var session = await context.Sessions.FindAsync(_sessionGid);
      if (session == null)
      {
        session = new Session { Gid = _sessionGid, LastActivityAt = DateTime.UtcNow };
        context.Sessions.Add(session);
      }
      else
      {
        session.LastActivityAt = DateTime.UtcNow;
        context.Sessions.Update(session);
      }
      await context.SaveChangesAsync();
    }
  }
}
