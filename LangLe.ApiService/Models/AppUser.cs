using Microsoft.AspNetCore.Identity;

namespace LangLe.ApiService.Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = "Learner";
    public string AvatarEmoji { get; set; } = "🦜";
    public int DailyGoalMinutes { get; set; } = 10;
    public string SourceLanguage { get; set; } = "en";
    public string TargetLanguage { get; set; } = "es";
    public bool DarkMode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserStreak? Streak { get; set; }
    public ICollection<UserProgress> Progress { get; set; } = [];
    public ICollection<UserGoal> Goals { get; set; } = [];
    public ICollection<UserWordBank> WordBank { get; set; } = [];
    public ICollection<UserAchievement> Achievements { get; set; } = [];
}
