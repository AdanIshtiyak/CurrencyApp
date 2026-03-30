using CurrencyApp.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CurrencyApp.Entity
{
  public class AppDbContext : DbContext
  {

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    #region DbSets

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<CurrencyData> CurrencyData { get; set; }

    #endregion
  }

  public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
  {
    public AppDbContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

      optionsBuilder.UseSqlite("Data Source=data.db");

      return new AppDbContext(optionsBuilder.Options);
    }
  }
}
