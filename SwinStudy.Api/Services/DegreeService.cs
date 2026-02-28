using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Data;
using SwinStudy.Api.Dtos;

namespace SwinStudy.Api.Services;

public class DegreesService
{
    private readonly AppDbContext _db;
    public DegreesService(AppDbContext db) => _db = db;

    public Task<List<DegreeResponseDto>> GetAllAsync() =>
        _db.Degrees
           .AsNoTracking()
           .OrderBy(d => d.DegreeName)
           .Select(d => new DegreeResponseDto(
               d.DegreeId,
               d.DegreeName,
               d.DegreeCode))
           .ToListAsync();
}