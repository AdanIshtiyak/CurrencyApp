using CurrencyApp.Entity.Models;

namespace CurrencyApp.Interfaces
{
  public interface ISessionRepository
  {
    Session? GetLastOtherSession(Guid excludeGid);
    Task<Session?> FindByGidAsync(Guid gid);
    Task AddOrUpdateAsync(Session session);
  }
}
