namespace LangLe.ApiService.Models;

public class UserWordBank
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int WordEntryId { get; set; }
    public DateTime LearnedAt { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;
    public WordEntry WordEntry { get; set; } = null!;
}
