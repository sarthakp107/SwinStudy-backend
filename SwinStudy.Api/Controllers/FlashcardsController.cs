using Microsoft.AspNetCore.Mvc;
using SwinStudy.Api.Dtos;
using SwinStudy.Api.Extensions;
using SwinStudy.Api.Services;

namespace SwinStudy.Api.Controllers;

[ApiController]
[Route("api/flashcards")]
public class FlashcardsController : ControllerBase
{
    private readonly FlashcardsService _flashcards;

    public FlashcardsController(FlashcardsService flashcards) => _flashcards = flashcards;

    private string? GetUserId() => User.GetUserIdString();

    /// <summary>Get current user's saved flashcards.</summary>
    /// 
    [HttpGet("saved")]
    public async Task<ActionResult<List<SavedFlashcardResponseDto>>> GetUserSaved()
    {
        var userId = GetUserId();

        var result = await _flashcards.GetUserSavedAsync(userId);
        return Ok(result);
    }

    /// <summary>Get a specific saved flashcard by question and answer.</summary>
    [HttpGet("saved/specific")]
    public async Task<ActionResult<SavedFlashcardResponseDto>> GetSpecificSaved(
        [FromQuery] string question,
        [FromQuery] string answer)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(question) || string.IsNullOrWhiteSpace(answer))
            return BadRequest("question and answer are required.");

        var result = await _flashcards.GetSpecificSavedAsync(userId, question, answer);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>Save a flashcard for the current user.</summary>
    [HttpPost("saved")]
    public async Task<ActionResult<SavedFlashcardResponseDto>> SaveFlashcard(
        [FromBody] CreateSavedFlashcardRequestDto request)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(request.Question) || string.IsNullOrWhiteSpace(request.Answer))
            return BadRequest("question and answer are required.");

        var result = await _flashcards.SaveFlashcardAsync(userId, request.Question, request.Answer);
        return CreatedAtAction(nameof(GetSpecificSaved),
            new { question = request.Question, answer = request.Answer }, result);
    }

    /// <summary>Delete a saved flashcard by id.</summary>
    [HttpDelete("saved/{id:long}")]
    public async Task<IActionResult> DeleteSaved(long id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var deleted = await _flashcards.DeleteSavedAsync(id, userId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>Delete a saved flashcard by question and answer.</summary>
    [HttpDelete("saved")]
    public async Task<IActionResult> DeleteSavedByQuestionAnswer([FromQuery] string question, [FromQuery] string answer)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(question) || string.IsNullOrWhiteSpace(answer))
            return BadRequest("question and answer are required.");

        var deleted = await _flashcards.DeleteSavedByQuestionAnswerAsync(userId, question, answer);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>Get current user's generated flashcards.</summary>
    [HttpGet("generated")]
    public async Task<ActionResult<List<GeneratedFlashcardResponseDto>>> GetUserGenerated()
    {
        var userId = GetUserId();

        var result = await _flashcards.GetUserGeneratedAsync(userId);
        return Ok(result);
    }

    /// <summary>Add a generated flashcard for the current user.</summary>
    [HttpPost("generated")]
    public async Task<ActionResult<GeneratedFlashcardResponseDto>> AddGenerated(
        [FromBody] CreateGeneratedFlashcardRequestDto request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Question) || string.IsNullOrWhiteSpace(request.Answer))
            return BadRequest("question and answer are required.");

        var result = await _flashcards.AddGeneratedAsync(userId, request.Question, request.Answer);
        return CreatedAtAction(nameof(GetUserGenerated), new { }, result);
    }
}
