namespace LangLe.ApiService.Models;

public class UserProgress
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int LessonId { get; set; }
    public int XpEarned { get; set; }
    public int Stars { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
}
