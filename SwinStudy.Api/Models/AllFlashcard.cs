namespace SwinStudy.Api.Models;

public class AllFlashcard
{
    public long Id { get; set; }
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
    public DateTime CreatedDate { get; set; }

    public ICollection<UserGeneratedFlashcard> GeneratedByUsers { get; set; } = new List<UserGeneratedFlashcard>();
}

