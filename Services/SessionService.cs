using CurrencyApp.Entity.Models;
using CurrencyApp.Interfaces;

namespace CurrencyApp.Services
{
  public class SessionService : ISessionServices
  {
    private readonly ISessionRepository _repository;
    private readonly Guid _sessionGid = Guid.NewGuid();

    public SessionService(ISessionRepository repository)
    {
      _repository = repository;
    }

    public string GetLastActivityAt()
    {
      var offsetHours = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours;

      return _repository
        .GetLastOtherSession(_sessionGid)
        ?.LastActivityAt.AddHours(offsetHours).ToString("MM/dd/yyyy HH:mm") ?? string.Empty;
    }

    public async Task UpdateLastActivityAt()
    {
      await _repository.AddOrUpdateAsync(new Session
      {
        Gid = _sessionGid,
        LastActivityAt = DateTime.UtcNow
      });
    }
  }
}
