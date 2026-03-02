using LangLe.Shared.Enums;

namespace LangLe.ApiService.Models;

public class UserGoal
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public GoalType GoalType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TargetValue { get; set; }
    public int CurrentValue { get; set; }

    public AppUser User { get; set; } = null!;
}
