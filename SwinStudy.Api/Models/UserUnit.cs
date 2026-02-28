namespace SwinStudy.Api.Models;

public class UserUnit
{
    public Guid UserId { get; set; }
    public long UnitId { get; set; }

    public User User { get; set; } = default!;
    public Unit Unit { get; set; } = default!;
}
