using CurrencyApp.Entity;
using CurrencyApp.Interfaces;
using CurrencyApp.Repositories;
using CurrencyApp.Repositories.Json;
using CurrencyApp.Repositories.Sqlite;
using CurrencyApp.Services;
using CurrencyApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CurrencyApp
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : System.Windows.Application
  {
    public static ServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      var services = new ServiceCollection();

      #region DI

      services.AddDbContextFactory<AppDbContext>(options =>
       options.UseSqlite("Data Source=data.db"));

      services.AddHttpClient<CurrencyServices>();

      // Concrete repositories
      services.AddScoped<SqliteCurrencyRepository>();
      services.AddScoped<SqliteSessionRepository>();
      services.AddSingleton<JsonCurrencyRepository>();
      services.AddSingleton<JsonSessionRepository>();

      // Dual-write repositories (write to both storages simultaneously, read from SQLite)
      services.AddScoped<ICurrencyRepository, DualWriteCurrencyRepository>();
      services.AddSingleton<ISessionRepository, DualWriteSessionRepository>();

      // Services
      services.AddScoped<ICurrencyListServices, CurrencyServices>();
      services.AddScoped<ICurrencyAddServices, CurrencyServices>();
      services.AddSingleton<ISessionServices, SessionService>();
      services.AddScoped<ILocalSettingServices, LocalSettingsServices>();

      // ViewModels
      services.AddTransient<CurrencyListViewModel>();
      services.AddTransient<AddCurrencyViewModel>();
      services.AddTransient<SettingsViewModel>();

      // Windows
      services.AddTransient<MainWindow>();

      #endregion

      ServiceProvider = services.BuildServiceProvider();

      using (var scope = ServiceProvider.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
      }

      var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
      mainWindow.Show();

      base.OnStartup(e);
    }
  }
}
