namespace LangLe.Shared.DTOs;

public record ProfileDto(
    string Email,
    string DisplayName,
    string AvatarEmoji,
    int DailyGoalMinutes,
    string SourceLanguage,
    string TargetLanguage,
    bool DarkMode);

public record UpdateProfileRequest(
    string? DisplayName,
    string? AvatarEmoji,
    int? DailyGoalMinutes,
    string? SourceLanguage,
    string? TargetLanguage,
    bool? DarkMode);

public record LessonCompleteRequest(int LessonId, int CorrectAnswers, int TotalExercises);
public record LessonCompleteResponse(int XpEarned, int Stars, int NewStreak, List<string> NewAchievements, List<WordBankEntryDto> NewWords);
