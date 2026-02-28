using Microsoft.AspNetCore.Mvc;
using SwinStudy.Api.Dtos;
using SwinStudy.Api.Services;

namespace SwinStudy.Api.Controllers;

[ApiController]
[Route("api/flashcards")]
public class FlashcardsController : ControllerBase
{
    private readonly FlashcardsService _flashcards;

    public FlashcardsController(FlashcardsService flashcards) => _flashcards = flashcards;

    // GET api/flashcards/saved/{userId}
    [HttpGet("saved/{userId}")]
    public async Task<ActionResult<List<SavedFlashcardResponseDto>>> GetUserSaved(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("userId is required.");
        }

        var result = await _flashcards.GetUserSavedAsync(userId);
        return Ok(result);
    }

    // GET api/flashcards/saved/specific?userId=...&question=...&answer=...
    [HttpGet("saved/specific")]
    public async Task<ActionResult<SavedFlashcardResponseDto?>> GetSpecificSaved(
        [FromQuery] string userId,
        [FromQuery] string question,
        [FromQuery] string answer)
    {
        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(question) ||
            string.IsNullOrWhiteSpace(answer))
        {
            return BadRequest("userId, question and answer are required.");
        }

        var result = await _flashcards.GetSpecificSavedAsync(userId, question, answer);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // POST api/flashcards/saved
    [HttpPost("saved")]
    public async Task<ActionResult<SavedFlashcardResponseDto>> SaveFlashcard(
        [FromBody] CreateSavedFlashcardRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) ||
            string.IsNullOrWhiteSpace(request.Question) ||
            string.IsNullOrWhiteSpace(request.Answer))
        {
            return BadRequest("userId, question and answer are required.");
        }

        var result = await _flashcards.SaveFlashcardAsync(
            request.UserId,
            request.Question,
            request.Answer);

        return CreatedAtAction(
            nameof(GetSpecificSaved),
            new { userId = request.UserId, question = request.Question, answer = request.Answer },
            result);
    }

    // DELETE api/flashcards/saved/{id}?userId=...
    [HttpDelete("saved/{id:long}")]
    public async Task<IActionResult> DeleteSaved(long id, [FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("userId is required.");
        }

        var deleted = await _flashcards.DeleteSavedAsync(id, userId);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // GET api/flashcards/generated/{userId}
    [HttpGet("generated/{userId}")]
    public async Task<ActionResult<List<GeneratedFlashcardResponseDto>>> GetUserGenerated(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("userId is required.");
        }

        var result = await _flashcards.GetUserGeneratedAsync(userId);
        return Ok(result);
    }

    // POST api/flashcards/generated
    // This endpoint lets you store a generated flashcard (e.g. after calling an LLM).
    [HttpPost("generated")]
    public async Task<ActionResult<GeneratedFlashcardResponseDto>> AddGenerated(
        [FromBody] CreateGeneratedFlashcardRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) ||
            string.IsNullOrWhiteSpace(request.Question) ||
            string.IsNullOrWhiteSpace(request.Answer))
        {
            return BadRequest("userId, question and answer are required.");
        }

        var result = await _flashcards.AddGeneratedAsync(
            request.UserId,
            request.Question,
            request.Answer);

        return CreatedAtAction(
            nameof(GetUserGenerated),
            new { userId = request.UserId },
            result);
    }
}

