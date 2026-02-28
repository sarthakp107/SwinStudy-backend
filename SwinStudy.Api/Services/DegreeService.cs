using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Data;
using SwinStudy.Api.Models;

namespace SwinStudy.Api.Services;

public class DegreesService
{
    private readonly AppDbContext _db;
    public DegreesService(AppDbContext db) => _db = db;

    public Task<List<Degree>> GetAllAsync() =>
        _db.Degrees
           .AsNoTracking()
           .OrderBy(d => d.DegreeName)
           .ToListAsync();
}