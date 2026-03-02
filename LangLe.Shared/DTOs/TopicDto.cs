namespace LangLe.Shared.DTOs;

public record TopicDto(
    int Id,
    string Name,
    string Description,
    string IconEmoji,
    int SortOrder,
    int TotalLessons,
    int CompletedLessons,
    double ProgressPercent);
