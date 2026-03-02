namespace LangLe.Shared.DTOs;

public record AchievementDto(
    int Id,
    string Name,
    string Description,
    string IconEmoji,
    bool IsUnlocked,
    DateTime? UnlockedAt);
