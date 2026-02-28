using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Data;
using SwinStudy.Api.Dtos;

namespace SwinStudy.Api.Services;

public class UnitsService
{
    private readonly AppDbContext _db;
    public UnitsService(AppDbContext db) => _db = db;

    public Task<List<UnitResponseDto>> GetAllAsync() =>
        _db.Units
           .AsNoTracking()
           .OrderBy(u => u.UnitName)
           .Select(u => new UnitResponseDto(
               u.UnitId,
               u.UnitName,
               u.UnitCode,
               u.CreditPoints))
           .ToListAsync();

    public Task<UnitResponseDto?> GetByIdAsync(long id) =>
        _db.Units
           .AsNoTracking()
           .Where(u => u.UnitId == id)
           .Select(u => new UnitResponseDto(
               u.UnitId,
               u.UnitName,
               u.UnitCode,
               u.CreditPoints))
           .FirstOrDefaultAsync();

    public Task<UnitResponseDto?> GetByCodeAsync(string unitCode) =>
        _db.Units
           .AsNoTracking()
           .Where(u => u.UnitCode == unitCode)
           .Select(u => new UnitResponseDto(
               u.UnitId,
               u.UnitName,
               u.UnitCode,
               u.CreditPoints))
           .FirstOrDefaultAsync();

    /// <summary>Get users who have the given unit in their survey (unit buddies).</summary>
    public async Task<List<UnitMemberDto>> GetUnitMembersByUnitNameAsync(string unitName)
    {
        if (string.IsNullOrWhiteSpace(unitName)) return new List<UnitMemberDto>();

        var unit = await _db.Units
            .AsNoTracking()
            .Where(u => u.UnitName == unitName)
            .Select(u => u.UnitId)
            .FirstOrDefaultAsync();

        if (unit == 0) return new List<UnitMemberDto>();

        return await _db.UserUnits
            .AsNoTracking()
            .Where(uu => uu.UnitId == unit)
            .Join(_db.Users, uu => uu.UserId, u => u.Id, (uu, u) => u)
            .OrderBy(u => u.FullName)
            .Select(u => new UnitMemberDto(u.Id, u.FullName))
            .ToListAsync();
    }
}