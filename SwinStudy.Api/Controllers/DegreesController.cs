using Microsoft.AspNetCore.Mvc;
using SwinStudy.Api.Data;
using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Services;

namespace SwinStudy.Api.Controllers;

[ApiController]
[Route("api/degrees")]
public class DegreesController : ControllerBase
{
    private readonly AppDbContext _db;
    public DegreesController(AppDbContext db) => _db= db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var degrees = await _db.Degrees.Select(d => new {d.DegreeId, d.DegreeName, d.DegreeCode})
        .ToListAsync();

        return Ok(degrees);
    }
}