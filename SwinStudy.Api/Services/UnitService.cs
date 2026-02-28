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
}