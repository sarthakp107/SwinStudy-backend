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

    /// <summary>Get users (unit buddies) who have selected this unit. Unit name from URL-encoded path.</summary>
    [HttpGet("members")]
    public async Task<ActionResult<List<UnitMemberDto>>> GetMembers([FromQuery] string unitName)
    {
        if (string.IsNullOrWhiteSpace(unitName)) return BadRequest("unitName is required.");

        var members = await _units.GetUnitMembersByUnitNameAsync(unitName);
        return Ok(members);
    }
}

