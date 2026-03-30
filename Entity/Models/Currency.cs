using System.ComponentModel.DataAnnotations;

namespace CurrencyApp.Entity.Models
{
  public class Currency
  {
    [Key]
    public int InternalId { get; set; }
    public string Id { get; set; } = string.Empty;
    public int CurrencyDataId { get; set; }
    public string NumCode { get; set; } = string.Empty;
    public string CharCode { get; set; } = string.Empty;
    public int Nominal { get; set; }
    public decimal Value { get; set; }
    public decimal Previous { get; set; }

    #region Relationships

    public virtual CurrencyData CurrencyData { get; set; } = null!;

    #endregion
  }
}
