using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LangLe.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LangLe.Web;

public sealed record AiCoachCallResult(AiCoachResponse? Response, string? ErrorMessage)
{
    public bool IsSuccess => Response is not null && string.IsNullOrWhiteSpace(ErrorMessage);
}

public class ApiClient(HttpClient http)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

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

    public async Task<AiCoachCallResult> GetAiCoachAsync(AiCoachRequest req, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await http.PostAsJsonAsync("/api/lessons/coach", req, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    return new AiCoachCallResult(null, "LeLe AI Coach returned an empty response.");
                }

                AiCoachResponse? payload;
                try
                {
                    payload = JsonSerializer.Deserialize<AiCoachResponse>(responseBody, JsonOptions);
                }
                catch (JsonException)
                {
                    return new AiCoachCallResult(null, "LeLe AI Coach returned an unexpected response.");
                }

                return payload is null
                    ? new AiCoachCallResult(null, "LeLe AI Coach returned an empty response.")
                    : new AiCoachCallResult(payload, null);
            }

            var errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                ? "Sign in to use LeLe AI Coach."
                : ExtractProblemDetail(responseBody) ?? $"LeLe AI Coach request failed with status {(int)response.StatusCode}.";

            return new AiCoachCallResult(null, errorMessage);
        }
        catch (HttpRequestException ex)
        {
            return new AiCoachCallResult(null, $"Couldn't reach LeLe AI Coach: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return new AiCoachCallResult(null, "LeLe AI Coach timed out. Please try again.");
        }
    }

    private static string? ExtractProblemDetail(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        var trimmed = responseBody.Trim();
        if (trimmed.StartsWith('<'))
        {
            return null;
        }

        try
        {
            var problem = JsonSerializer.Deserialize<ProblemDetails>(trimmed, JsonOptions);
            return problem?.Detail ?? problem?.Title;
        }
        catch (JsonException)
        {
            return null;
        }
    }

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
