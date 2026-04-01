namespace CurrencyApp.Interfaces
{
  public interface ISessionServices
  {
    string GetLastActivityAt();
    Task UpdateLastActivityAt();
  }
}
