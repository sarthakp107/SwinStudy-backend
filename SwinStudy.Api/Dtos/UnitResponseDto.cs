namespace SwinStudy.Api.Dtos;

public record UnitResponseDto(
    long UnitId,
    string UnitName,
    string? UnitCode,
    decimal? CreditPoints);

