namespace LangLe.ApiService.Models;

public class UserStreak
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime LastActivityDate { get; set; }

    public AppUser User { get; set; } = null!;
}
