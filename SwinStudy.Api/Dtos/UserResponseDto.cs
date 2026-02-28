namespace SwinStudy.Api.Dtos;

public record UserResponseDto(Guid Id, string FullName, string Email, DateTime CreatedAt);
