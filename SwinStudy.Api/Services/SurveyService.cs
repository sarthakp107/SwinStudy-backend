using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Data;
using SwinStudy.Api.Dtos;
using SwinStudy.Api.Models;

namespace SwinStudy.Api.Services;

public class SurveyService
{
    private readonly AppDbContext _db;

    public SurveyService(AppDbContext db) => _db = db;

    /// <summary>Get the units linked to the user (from user_units joined with all_units).</summary>
    public async Task<List<UnitResponseDto>> GetUserUnitsAsync(Guid userId)
    {
        return await _db.UserUnits
            .AsNoTracking()
            .Where(uu => uu.UserId == userId)
            .Join(_db.Units, uu => uu.UnitId, u => u.UnitId, (uu, u) => u)
            .OrderBy(u => u.UnitName)
            .Select(u => new UnitResponseDto(u.UnitId, u.UnitName, u.UnitCode, u.CreditPoints))
            .ToListAsync();
    }

    public async Task<bool> GetHasSubmittedSurveyAsync(Guid userId)
    {
        var user = await _db.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.HasSubmittedSurvey)
            .FirstOrDefaultAsync();
        return user;
    }

    public async Task<bool> SubmitSurveyAsync(Guid userId, SubmitSurveyRequestDto dto)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return false;

        // Look up unit IDs by unit names
        var unitIds = await _db.Units
            .Where(u => dto.SelectedUnits.Contains(u.UnitName))
            .Select(u => u.UnitId)
            .ToListAsync();

        if (unitIds.Count != dto.SelectedUnits.Count)
            return false; // Some unit names not found

        // Remove existing user units and add new ones
        var existing = await _db.UserUnits.Where(uu => uu.UserId == userId).ToListAsync();
        _db.UserUnits.RemoveRange(existing);

        user.HasSubmittedSurvey = true;
        user.Degree = dto.Degree;
        user.Semester = dto.Semester;
        user.UpdatedAt = DateTime.UtcNow;

        foreach (var unitId in unitIds)
        {
            _db.UserUnits.Add(new UserUnit { UserId = userId, UnitId = unitId });
        }

        await _db.SaveChangesAsync();
        return true;
    }
}
