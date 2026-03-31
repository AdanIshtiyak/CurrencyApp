using CurrencyApp.Entity;
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

      services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=data.db"));

      services.AddHttpClient();

      //Services
      services.AddScoped<CurrencyServices>();

      //ViewModels
      services.AddTransient<MainWindowViewModel>();
      services.AddTransient<CurrencyListViewModel>();
      services.AddTransient<AddCurrencyViewModel>();
      services.AddTransient<SettingsViewModel>();

      //Wimdows
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
