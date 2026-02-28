namespace SwinStudy.Api.Models;

public class Degree
{
    public long DegreeId { get; set; }
    public string DegreeName { get; set; } = default!;
    public string? DegreeCode { get; set; }
    public DateTime CreatedAt { get; set; }
}