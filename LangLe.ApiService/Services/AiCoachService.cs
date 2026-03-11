using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LangLe.ApiService.Data;
using LangLe.ApiService.Models;
using LangLe.ApiService.Options;
using LangLe.Shared.DTOs;
using LangLe.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LangLe.ApiService.Services;

public class AiCoachService(
    HttpClient httpClient,
    LangLeDbContext db,
    IOptions<AiCoachOptions> options)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly AiCoachOptions _options = options.Value;

    public async Task<AiCoachResponse> GetCoachResponseAsync(
        AiCoachRequest request,
        AppUser user,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        if (request.Mode == AiCoachMode.ExplainMistake && string.IsNullOrWhiteSpace(request.UserAnswer))
        {
            throw new ArgumentException("A learner answer is required to explain a mistake.", nameof(request));
        }

        var exercise = await db.Exercises
            .Include(e => e.Lesson)
            .ThenInclude(l => l.Topic)
            .FirstOrDefaultAsync(e => e.Id == request.ExerciseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Exercise {request.ExerciseId} was not found.");

        ConfigureClient();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = JsonContent.Create(
                new OpenAiChatCompletionsRequest(
                    _options.Model,
                    [
                        new OpenAiMessage("system", BuildSystemPrompt()),
                        new OpenAiMessage("user", BuildUserPrompt(exercise, user, request))
                    ],
                    _options.Temperature),
                options: JsonOptions)
        };

        httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_options.ApiKey}");
        httpRequest.Headers.TryAddWithoutValidation("Accept", "application/json");

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var detail = string.IsNullOrWhiteSpace(errorBody)
                ? "The provider returned an empty error response."
                : errorBody.Trim();

            throw new HttpRequestException(
                $"The configured AI provider returned {(int)response.StatusCode} ({response.StatusCode}). {detail}",
                null,
                response.StatusCode);
        }

        var completion = await response.Content.ReadFromJsonAsync<OpenAiChatCompletionsResponse>(JsonOptions, cancellationToken)
            ?? throw new AiCoachResponseFormatException("The AI provider returned an empty response body.");

        var content = completion.Choices.FirstOrDefault()?.Message.Content;
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new AiCoachResponseFormatException("The AI provider response did not include coach content.");
        }

        var coachPayload = ParseCoachPayload(content);

        return new AiCoachResponse(
            request.Mode,
            coachPayload.Title,
            coachPayload.Message,
            coachPayload.Takeaway,
            coachPayload.Example,
            coachPayload.Tips ?? []);
    }

    private void EnsureConfigured()
    {
        if (!_options.Enabled)
        {
            throw new AiCoachConfigurationException("LeLe AI Coach is disabled. Set AiCoach:Enabled to true to turn it on.");
        }

        if (string.IsNullOrWhiteSpace(_options.Endpoint) ||
            string.IsNullOrWhiteSpace(_options.ApiKey) ||
            string.IsNullOrWhiteSpace(_options.Model))
        {
            throw new AiCoachConfigurationException(
                "LeLe AI Coach is not configured yet. Set AiCoach:Endpoint, AiCoach:ApiKey, and AiCoach:Model to enable it.");
        }

        if (!Uri.TryCreate(NormalizeEndpoint(_options.Endpoint), UriKind.Absolute, out var endpoint))
        {
            throw new AiCoachConfigurationException("AiCoach:Endpoint must be an absolute URL.");
        }

        httpClient.BaseAddress = endpoint;
        httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    private void ConfigureClient()
    {
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
    }

    private static string BuildSystemPrompt() =>
        """
        You are LeLe, a warm and concise language coach inside the LangLe learning app.
        You help learners studying English, Spanish, and Telugu.

        Return only valid JSON with this exact shape:
        {
          "title": "short heading",
          "message": "2-4 short sentences",
          "takeaway": "one-sentence lesson",
          "example": "optional short example or null",
          "tips": ["short tip", "short tip", "short tip"]
        }

        Rules:
        - Keep the total reply concise and encouraging.
        - Be specific to the exercise.
        - In hint mode, never reveal, quote, spell out, or directly translate the exact correct answer.
        - In explain mode, explain the likely confusion and how to fix it next time.
        - In bonus example mode, celebrate briefly and give a fresh practice example.
        - Tips must contain 2 or 3 short items.
        """;

    private static string BuildUserPrompt(Exercise exercise, AppUser user, AiCoachRequest request)
    {
        var options = JsonSerializer.Deserialize<List<string>>(exercise.OptionsJson) ?? [];
        var modeInstructions = request.Mode switch
        {
            AiCoachMode.Hint =>
                "Give a strategic nudge. Focus on pattern recognition, elimination, grammar, or context. Do not reveal the answer.",
            AiCoachMode.ExplainMistake =>
                "Explain why the learner answer missed the mark, why the correct answer fits better, and what to watch for next time.",
            AiCoachMode.BonusExample =>
                "Give a short bonus example that uses the same concept, then add a tiny follow-up challenge the learner can think about.",
            _ =>
                "Be helpful."
        };

        return $$"""
        Mode: {{request.Mode}}
        Learner display name: {{user.DisplayName}}
        Learner source language: {{user.SourceLanguage}}
        Learner target language: {{user.TargetLanguage}}
        Topic: {{exercise.Lesson.Topic.Name}}
        Topic description: {{exercise.Lesson.Topic.Description}}
        Lesson: {{exercise.Lesson.Title}}
        Exercise type: {{exercise.Type}}
        Question: {{exercise.Question}}
        Answer options: {{JsonSerializer.Serialize(options)}}
        Existing static hint: {{exercise.HintText ?? "None"}}
        Learner answer: {{request.UserAnswer ?? "None"}}
        Correct answer: {{exercise.CorrectAnswer}}

        Task:
        {{modeInstructions}}
        Keep the tone playful, calm, and useful for an active learning moment.
        """;
    }

    private static CoachPayload ParseCoachPayload(string content)
    {
        var json = ExtractJson(content);
        var payload = JsonSerializer.Deserialize<CoachPayload>(json, JsonOptions)
            ?? throw new AiCoachResponseFormatException("The AI provider returned content that could not be parsed.");

        if (string.IsNullOrWhiteSpace(payload.Title) ||
            string.IsNullOrWhiteSpace(payload.Message) ||
            string.IsNullOrWhiteSpace(payload.Takeaway))
        {
            throw new AiCoachResponseFormatException("The AI provider response was missing required fields.");
        }

        var tips = (payload.Tips ?? [])
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .ToList();

        if (tips.Count == 0)
        {
            tips =
            [
                "Slow down and scan the clue words.",
                "Say the phrase out loud once.",
                "Look for the pattern before guessing."
            ];
        }

        return payload with
        {
            Title = payload.Title.Trim(),
            Message = payload.Message.Trim(),
            Takeaway = payload.Takeaway.Trim(),
            Example = string.IsNullOrWhiteSpace(payload.Example) ? null : payload.Example.Trim(),
            Tips = tips
        };
    }

    private static string ExtractJson(string content)
    {
        var trimmed = content.Trim();

        if (trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            var lines = trimmed.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            trimmed = string.Join('\n', lines.Skip(1).SkipLast(1)).Trim();
        }

        var firstBrace = trimmed.IndexOf('{');
        var lastBrace = trimmed.LastIndexOf('}');

        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            return trimmed[firstBrace..(lastBrace + 1)];
        }

        throw new AiCoachResponseFormatException("The AI provider did not return a JSON object.");
    }

    private static string NormalizeEndpoint(string endpoint) =>
        endpoint.EndsWith("/", StringComparison.Ordinal) ? endpoint : $"{endpoint}/";

    private sealed record OpenAiChatCompletionsRequest(
        string Model,
        IReadOnlyList<OpenAiMessage> Messages,
        double Temperature);

    private sealed record OpenAiMessage(string Role, string Content);

    private sealed record OpenAiChatCompletionsResponse(List<OpenAiChoice> Choices);

    private sealed record OpenAiChoice(OpenAiMessageContent Message);

    private sealed record OpenAiMessageContent(string? Content);

    private sealed record CoachPayload(
        string Title,
        string Message,
        string Takeaway,
        string? Example,
        List<string>? Tips);
}

public sealed class AiCoachConfigurationException(string message) : Exception(message);

public sealed class AiCoachResponseFormatException(string message) : Exception(message);
