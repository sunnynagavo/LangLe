using LangLe.Shared.Enums;

namespace LangLe.Shared.DTOs;

public record AiCoachRequest(int ExerciseId, AiCoachMode Mode, string? UserAnswer);

public record AiCoachResponse(
    AiCoachMode Mode,
    string Title,
    string Message,
    string Takeaway,
    string? Example,
    List<string> Tips);
