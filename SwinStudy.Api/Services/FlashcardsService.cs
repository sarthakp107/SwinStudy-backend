using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using UglyToad.PdfPig;
using SwinStudy.Api.Data;
using SwinStudy.Api.Dtos;
using SwinStudy.Api.Models;

namespace SwinStudy.Api.Services;

/// <summary>Service interface for all flashcard-related endpoints.</summary>
public interface IFlashcardsService
{
    Task<GenerateFlashcardsResponseDto> GenerateFromPdfAsync(byte[] pdfBytes, CancellationToken cancellationToken = default);
    Task<List<SavedFlashcardResponseDto>> GetUserSavedAsync(string userId);
    Task<SavedFlashcardResponseDto?> GetSpecificSavedAsync(string userId, string question, string answer);
    Task<SavedFlashcardResponseDto> SaveFlashcardAsync(string userId, string question, string answer);
    Task<bool> DeleteSavedAsync(long id, string userId);
    Task<bool> DeleteSavedByQuestionAnswerAsync(string userId, string question, string answer);
    Task<List<GeneratedFlashcardResponseDto>> GetUserGeneratedAsync(string userId);
    Task<GeneratedFlashcardResponseDto> AddGeneratedAsync(string userId, string question, string answer);
}

public class FlashcardsService : IFlashcardsService
{
    private static readonly Regex QnaRegex = new(
        @"Question:\s*(.*?)\s*Answer:\s*(.*?)(?=Question:|$)",
        RegexOptions.Singleline | RegexOptions.Compiled);


    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public FlashcardsService(AppDbContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<GenerateFlashcardsResponseDto> GenerateFromPdfAsync(byte[] pdfBytes, CancellationToken cancellationToken = default)
    {
        var text = ExtractTextFromPdf(pdfBytes);
        if (string.IsNullOrWhiteSpace(text))
            return new GenerateFlashcardsResponseDto(new List<GenerateFlashcardItemDto>());

        var apiKey = _configuration["OpenRouter:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return new GenerateFlashcardsResponseDto(new List<GenerateFlashcardItemDto>());

        var prompt = _configuration["OpenRouter:FlashcardPrompt"];
        var qnaContent = await CallOpenRouterAsync(text, prompt, apiKey, cancellationToken);
        var flashcards = ParseQnaResponse(qnaContent);
        return new GenerateFlashcardsResponseDto(flashcards);
    }

    private static string ExtractTextFromPdf(byte[] pdfBytes)
    {
        try
        {
            using var document = PdfDocument.Open(pdfBytes);
            var sb = new StringBuilder();
            foreach (var page in document.GetPages())
            {
                var pageText = string.Join("", page.Letters.Select(x => x.Value));
                if (sb.Length > 0) sb.AppendLine();
                sb.Append(pageText);
            }
            return sb.ToString().Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<string> CallOpenRouterAsync(string text, string systemPrompt, string apiKey, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var payload = new
        {
            model = _configuration["OpenRouter:Model"] ?? "openrouter/free",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = text }
            }
        };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var OpenRouterUrl = _configuration["OpenRouter:Url"];
        using var request = new HttpRequestMessage(HttpMethod.Post, OpenRouterUrl);
        request.Headers.Add("Authorization", "Bearer " + apiKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc = System.Text.Json.JsonDocument.Parse(body);
        var content = doc.RootElement
            .TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0
            ? choices[0].GetProperty("message").GetProperty("content").GetString()
            : null;
        return content?.Trim() ?? string.Empty;
    }

    private static List<GenerateFlashcardItemDto> ParseQnaResponse(string qnaContent)
    {
        var list = new List<GenerateFlashcardItemDto>();
        var matches = QnaRegex.Matches(qnaContent);
        foreach (Match m in matches)
        {
            if (m.Success && m.Groups.Count >= 3)
            {
                var question = m.Groups[1].Value.Trim();
                var answer = m.Groups[2].Value.Trim();
                if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(answer))
                    list.Add(new GenerateFlashcardItemDto(question, answer));
            }
        }
        return list;
    }

    public Task<List<SavedFlashcardResponseDto>> GetUserSavedAsync(string userId) =>
        _db.UserSavedFlashcards
           .AsNoTracking()
           .Where(f => f.UserId == userId)
           .OrderByDescending(f => f.CreatedAt)
           .Select(f => new SavedFlashcardResponseDto(
               f.Id,
               f.Question,
               f.Answer,
               f.CreatedAt))
           .ToListAsync();

    public Task<SavedFlashcardResponseDto?> GetSpecificSavedAsync(string userId, string question, string answer) =>
        _db.UserSavedFlashcards
           .AsNoTracking()
           .Where(f => f.UserId == userId &&
                       f.Question == question &&
                       f.Answer == answer)
           .OrderByDescending(f => f.CreatedAt)
           .Select(f => new SavedFlashcardResponseDto(
               f.Id,
               f.Question,
               f.Answer,
               f.CreatedAt))
           .FirstOrDefaultAsync();

    public async Task<SavedFlashcardResponseDto> SaveFlashcardAsync(string userId, string question, string answer)
    {
        var entity = new UserSavedFlashcard
        {
            UserId = userId,
            Question = question,
            Answer = answer,
            CreatedAt = DateTime.UtcNow
        };

        _db.UserSavedFlashcards.Add(entity);
        await _db.SaveChangesAsync();

        return new SavedFlashcardResponseDto(entity.Id, entity.Question, entity.Answer, entity.CreatedAt);
    }

    public async Task<bool> DeleteSavedAsync(long id, string userId)
    {
        var entity = await _db.UserSavedFlashcards
            .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

        if (entity is null)
        {
            return false;
        }

        _db.UserSavedFlashcards.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSavedByQuestionAnswerAsync(string userId, string question, string answer)
    {
        var entity = await _db.UserSavedFlashcards
            .FirstOrDefaultAsync(f => f.UserId == userId && f.Question == question && f.Answer == answer);

        if (entity is null)
        {
            return false;
        }

        _db.UserSavedFlashcards.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<List<GeneratedFlashcardResponseDto>> GetUserGeneratedAsync(string userId) =>
        _db.UserGeneratedFlashcards
           .AsNoTracking()
           .Where(x => x.UserId == userId)
           .Include(x => x.Flashcard)
           .OrderByDescending(x => x.CreatedAt)
           .Select(x => new GeneratedFlashcardResponseDto(
               x.Id,
               x.UserId,
               x.Flashcard.Question,
               x.Flashcard.Answer,
               x.CreatedAt))
           .ToListAsync();

    public async Task<GeneratedFlashcardResponseDto> AddGeneratedAsync(string userId, string question, string answer)
    {
        var flashcard = new AllFlashcard
        {
            Question = question,
            Answer = answer,
            CreatedDate = DateTime.UtcNow
        };

        _db.AllFlashcards.Add(flashcard);
        await _db.SaveChangesAsync();

        var generated = new UserGeneratedFlashcard
        {
            UserId = userId,
            QnaReferenceId = flashcard.Id,
            CreatedAt = DateTime.UtcNow
        };

        _db.UserGeneratedFlashcards.Add(generated);
        await _db.SaveChangesAsync();

        return new GeneratedFlashcardResponseDto(
            generated.Id,
            generated.UserId,
            flashcard.Question,
            flashcard.Answer,
            generated.CreatedAt);
    }
}

