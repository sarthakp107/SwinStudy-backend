namespace SwinStudy.Api.Dtos;

public record CreateSavedFlashcardRequestDto(
    string UserId,
    string Question,
    string Answer);

