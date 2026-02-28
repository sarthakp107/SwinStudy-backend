namespace SwinStudy.Api.Dtos;

public record GeneratedFlashcardResponseDto(
    long Id,
    string UserId,
    string Question,
    string Answer,
    DateTime CreatedAt);

