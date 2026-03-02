namespace LangLe.ApiService.Models;

public class Achievement
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconEmoji { get; set; } = "🏅";
    public string CriteriaType { get; set; } = string.Empty;
    public int CriteriaValue { get; set; }
}
