using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Data;
using SwinStudy.Api.Dtos;
using SwinStudy.Api.Models;

namespace SwinStudy.Api.Services;

public class FlashcardsService
{
    private readonly AppDbContext _db;

    public FlashcardsService(AppDbContext db) => _db = db;

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

