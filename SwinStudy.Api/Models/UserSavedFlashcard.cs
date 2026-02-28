namespace SwinStudy.Api.Models;

public class UserSavedFlashcard
{
    public long Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

