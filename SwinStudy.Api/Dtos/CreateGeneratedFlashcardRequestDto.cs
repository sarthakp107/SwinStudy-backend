namespace SwinStudy.Api.Dtos;

public record CreateGeneratedFlashcardRequestDto(
    string UserId,
    string Question,
    string Answer);

