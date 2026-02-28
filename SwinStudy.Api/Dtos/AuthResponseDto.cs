namespace SwinStudy.Api.Dtos;

public record AuthResponseDto(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds,
    UserResponseDto User);
