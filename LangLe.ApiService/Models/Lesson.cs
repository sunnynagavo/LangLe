namespace LangLe.ApiService.Models;

public class Lesson
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Topic Topic { get; set; } = null!;
    public ICollection<Exercise> Exercises { get; set; } = [];
}
