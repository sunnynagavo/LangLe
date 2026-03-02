namespace LangLe.Shared.DTOs;

public record DashboardDto(
    int CurrentStreak,
    int LongestStreak,
    int TotalXp,
    int TodayXp,
    int WordsLearned,
    int LessonsCompleted,
    string CurrentLevel,
    LessonSuggestionDto? SuggestedLesson,
    List<WeeklyXpDto> WeeklyXp,
    List<GoalProgressDto> Goals,
    List<AchievementDto> RecentAchievements);

public record LessonSuggestionDto(int LessonId, int TopicId, string TopicName, string LessonTitle, string TopicEmoji);
public record WeeklyXpDto(string DayLabel, int Xp);
public record GoalProgressDto(int Id, string Description, int TargetValue, int CurrentValue, double ProgressPercent);
