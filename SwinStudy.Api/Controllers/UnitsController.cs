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
}

