namespace LangLe.ApiService.Options;

public class AiCoachOptions
{
    public const string SectionName = "AiCoach";

    public bool Enabled { get; set; } = true;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
    public double Temperature { get; set; } = 0.4;
}
