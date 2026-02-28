namespace SwinStudy.Api.Dtos;

public record SubmitSurveyRequestDto(
    string Degree,
    int Semester,
    IReadOnlyList<string> SelectedUnits);
