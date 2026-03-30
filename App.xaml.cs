using CurrencyApp.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CurrencyApp
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public static ServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      var services = new ServiceCollection();

      #region DI

      services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=data.db"));

      services.AddHttpClient();

      #endregion

      ServiceProvider = services.BuildServiceProvider();

      using (var scope = ServiceProvider.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
      }

      base.OnStartup(e);
    }
  }

}
