using Microsoft.AspNetCore.Mvc;
using SwinStudy.Api.Services;
using SwinStudy.Api.Dtos;

namespace SwinStudy.Api.Controllers;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    private readonly UnitsService _units;

    public UnitsController(UnitsService units) => _units = units;

    [HttpGet]
    public async Task<ActionResult<List<UnitResponseDto>>> GetAll()
    {
        var units = await _units.GetAllAsync();
        return Ok(units);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UnitResponseDto>> GetById(long id)
    {
        var unit = await _units.GetByIdAsync(id);
        if (unit is null) return NotFound();
        return Ok(unit);
    }

    [HttpGet("by-code/{unitCode}")]
    public async Task<ActionResult<UnitResponseDto>> GetByCode(string unitCode)
    {
        if (string.IsNullOrWhiteSpace(unitCode)) return BadRequest("unitCode is required.");

        var unit = await _units.GetByCodeAsync(unitCode);
        if (unit is null) return NotFound();
        return Ok(unit);
    }
}

