namespace SwinStudy.Api.Models;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool HasSubmittedSurvey { get; set; }
    public string? Degree { get; set; }
    public int? Semester { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
