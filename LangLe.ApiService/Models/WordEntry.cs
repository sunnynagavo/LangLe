namespace LangLe.ApiService.Models;

public class WordEntry
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public string English { get; set; } = string.Empty;
    public string Spanish { get; set; } = string.Empty;
    public string Telugu { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public Topic Topic { get; set; } = null!;
}
