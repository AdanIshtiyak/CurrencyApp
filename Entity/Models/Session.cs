using System.ComponentModel.DataAnnotations;

namespace CurrencyApp.Entity.Models
{
  public class Session
  {
    [Key]
    public Guid Gid { get; set; }
    public DateTime LastActivityAt { get; set; }
  }
}
