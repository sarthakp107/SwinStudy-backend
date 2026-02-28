using Microsoft.AspNetCore.Mvc;
using SwinStudy.Api.Services;
using SwinStudy.Api.Dtos;

namespace SwinStudy.Api.Controllers;

[ApiController]
[Route("api/degrees")]
public class DegreesController : ControllerBase
{
    private readonly DegreesService _degrees;
    public DegreesController(DegreesService degrees) => _degrees = degrees;

    [HttpGet]
    public async Task<ActionResult<List<DegreeResponseDto>>> GetAll()
    {
        var degrees = await _degrees.GetAllAsync();
        return Ok(degrees);
    }
}