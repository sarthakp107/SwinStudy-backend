namespace SwinStudy.Api.Models;

public class Unit
{
    public long UnitId { get; set; }
    public string UnitName { get; set; } = default!;
    public string? UnitCode { get; set; }
    public decimal? CreditPoints { get; set; }
    public DateTime CreatedAt { get; set; }
}