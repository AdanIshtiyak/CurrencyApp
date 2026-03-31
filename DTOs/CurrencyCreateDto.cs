namespace CurrencyApp.DTOs
{
  public class CurrencyCreateDto
  {
    public string Id { get; set; }
    public string NumCode { get; set; }
    public string CharCode { get; set; } = string.Empty;
    public int Nominal { get; set; }
    public decimal Value { get; set; }
    public decimal? Previous { get; set; }
  }
}
