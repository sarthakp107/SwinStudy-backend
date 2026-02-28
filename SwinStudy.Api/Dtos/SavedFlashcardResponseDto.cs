namespace SwinStudy.Api.Dtos;

public record SavedFlashcardResponseDto(
    long Id,
    string Question,
    string Answer,
    DateTime CreatedAt);

