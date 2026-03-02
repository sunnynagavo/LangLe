namespace LangLe.ApiService.Models;

public class Topic
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconEmoji { get; set; } = "📚";
    public int SortOrder { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = [];
    public ICollection<WordEntry> WordEntries { get; set; } = [];
}
