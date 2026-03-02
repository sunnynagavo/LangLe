namespace LangLe.Shared.DTOs;

public record LessonDto(
    int Id,
    string Title,
    int SortOrder,
    bool IsCompleted,
    int ExerciseCount);
