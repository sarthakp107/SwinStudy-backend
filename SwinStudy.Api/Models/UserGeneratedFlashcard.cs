namespace SwinStudy.Api.Models;

public class UserGeneratedFlashcard
{
    public long Id { get; set; }
    public string UserId { get; set; } = default!;
    public long QnaReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }

    public AllFlashcard Flashcard { get; set; } = default!;
}

