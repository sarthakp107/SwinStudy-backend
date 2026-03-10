namespace SwinStudy.Api.Dtos;

/// <summary>Single Q&amp;A pair returned from generate (not yet persisted).</summary>
public record GenerateFlashcardItemDto(string Question, string Answer);

/// <summary>Response for POST /api/flashcards/generate (PDF → generated Q&amp;A pairs).</summary>
public record GenerateFlashcardsResponseDto(List<GenerateFlashcardItemDto> Flashcards);
