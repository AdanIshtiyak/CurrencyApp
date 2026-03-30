namespace CurrencyApp.DTOs
{
  public class CurrencyJsonDto
  {
    public DateTimeOffset Date { get; set; }
    public Dictionary<string, CurrencyJsonDetailDto> Valute { get; set; } = new();
  }
}
