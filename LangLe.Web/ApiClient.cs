using System.Net.Http.Json;
using LangLe.Shared.DTOs;

namespace LangLe.Web;

public class ApiClient(HttpClient http)
{
    // Auth
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest req) =>
        await PostAsync<AuthResponse>("/api/auth/register", req);

    public async Task<AuthResponse?> LoginAsync(LoginRequest req) =>
        await PostAsync<AuthResponse>("/api/auth/login", req);

    public async Task LogoutAsync() =>
        await http.PostAsync("/api/auth/logout", null);

    public async Task<ProfileDto?> GetMeAsync() =>
        await GetAsync<ProfileDto>("/api/auth/me");

    // Profile
    public async Task<ProfileDto?> UpdateProfileAsync(UpdateProfileRequest req) =>
        await PutAsync<ProfileDto>("/api/profile", req);

    // Topics & Lessons
    public async Task<List<TopicDto>> GetTopicsAsync() =>
        await GetAsync<List<TopicDto>>("/api/topics") ?? [];

    public async Task<List<LessonDto>> GetLessonsAsync(int topicId) =>
        await GetAsync<List<LessonDto>>($"/api/topics/{topicId}/lessons") ?? [];

    public async Task<List<ExerciseDto>> GetExercisesAsync(int lessonId) =>
        await GetAsync<List<ExerciseDto>>($"/api/lessons/{lessonId}/exercises") ?? [];

    public async Task<LessonCompleteResponse?> CompleteLessonAsync(LessonCompleteRequest req) =>
        await PostAsync<LessonCompleteResponse>("/api/lessons/complete", req);

    // Dashboard
    public async Task<DashboardDto?> GetDashboardAsync() =>
        await GetAsync<DashboardDto>("/api/dashboard");

    // Word Bank
    public async Task<List<WordBankEntryDto>> GetWordBankAsync() =>
        await GetAsync<List<WordBankEntryDto>>("/api/wordbank") ?? [];

    // Achievements
    public async Task<List<AchievementDto>> GetAchievementsAsync() =>
        await GetAsync<List<AchievementDto>>("/api/achievements") ?? [];

    private async Task<T?> GetAsync<T>(string url)
    {
        try { return await http.GetFromJsonAsync<T>(url); }
        catch { return default; }
    }

    private async Task<T?> PostAsync<T>(string url, object data)
    {
        try
        {
            var response = await http.PostAsJsonAsync(url, data);
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch { return default; }
    }

    private async Task<T?> PutAsync<T>(string url, object data)
    {
        try
        {
            var response = await http.PutAsJsonAsync(url, data);
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch { return default; }
    }
}
