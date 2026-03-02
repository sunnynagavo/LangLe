using LangLe.Shared.Enums;

namespace LangLe.ApiService.Models;

public class Exercise
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public ExerciseType Type { get; set; }
    public string Question { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string OptionsJson { get; set; } = "[]";
    public string? ImageUrl { get; set; }
    public string? HintText { get; set; }

    public Lesson Lesson { get; set; } = null!;
}
