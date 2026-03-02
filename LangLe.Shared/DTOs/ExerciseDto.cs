using LangLe.Shared.Enums;

namespace LangLe.Shared.DTOs;

public record ExerciseDto(
    int Id,
    ExerciseType Type,
    string Question,
    string CorrectAnswer,
    List<string> Options,
    string? ImageUrl,
    string? HintText);
