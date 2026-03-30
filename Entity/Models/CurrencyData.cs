using System.ComponentModel.DataAnnotations;

namespace CurrencyApp.Entity.Models
{
  public class CurrencyData
  {
    [Key]
    public int Id { get; set; }
    public DateTime DataAt { get; set; }

    #region Relationships

    public virtual ICollection<Currency> Currencies { get; set; } = new List<Currency>();

    #endregion
  }
}
