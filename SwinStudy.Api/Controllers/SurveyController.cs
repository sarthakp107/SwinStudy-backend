using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwinStudy.Api.Dtos;
using SwinStudy.Api.Extensions;
using SwinStudy.Api.Services;

namespace SwinStudy.Api.Controllers;

[ApiController]
[Route("api/survey")]
[Authorize]
public class SurveyController : ControllerBase
{
    private readonly SurveyService _survey;

    public SurveyController(SurveyService survey) => _survey = survey;

    /// <summary>Get the current user's linked units (from survey).</summary>
    [HttpGet("my-units")]
    public async Task<ActionResult<List<UnitResponseDto>>> GetMyUnits()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var units = await _survey.GetUserUnitsAsync(userId.Value);
        return Ok(units);
    }

    /// <summary>Get whether the current user has submitted the survey.</summary>
    [HttpGet("status")]
    public async Task<ActionResult<object>> GetStatus()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var hasSubmitted = await _survey.GetHasSubmittedSurveyAsync(userId.Value);
        return Ok(new { hasSubmittedSurvey = hasSubmitted });
    }

    /// <summary>Submit the survey (degree, semester, units).</summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitSurveyRequestDto dto)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.Degree) || dto.Semester < 1 || dto.Semester > 8)
            return BadRequest(new { error = "Degree and semester (1-8) are required." });

        if (dto.SelectedUnits is null || dto.SelectedUnits.Count < 4)
            return BadRequest(new { error = "Please select 4 units." });

        var success = await _survey.SubmitSurveyAsync(userId.Value, dto);
        if (!success) return BadRequest(new { error = "Failed to save survey. Check that unit names are valid." });

        return Ok(new { success = true });
    }
}
