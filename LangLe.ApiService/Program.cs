using LangLe.ApiService.Data;
using LangLe.ApiService.Models;
using LangLe.ApiService.Options;
using LangLe.ApiService.Services;
using LangLe.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

// Database via Aspire
builder.AddNpgsqlDbContext<LangLeDbContext>("langdb");

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<LangLeDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/api/auth/unauthorized";
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();
builder.Services.Configure<AiCoachOptions>(builder.Configuration.GetSection(AiCoachOptions.SectionName));
builder.Services.AddScoped<LearningService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddHttpClient<AiCoachService>();

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LangLeDbContext>();
    await SeedData.InitializeAsync(db);
}

// === AUTH ENDPOINTS ===
app.MapPost("/api/auth/register", async (RegisterRequest req, UserManager<AppUser> userManager, SignInManager<AppUser> signIn) =>
{
    var user = new AppUser { UserName = req.Email, Email = req.Email, DisplayName = req.DisplayName };
    var result = await userManager.CreateAsync(user, req.Password);
    if (!result.Succeeded) return Results.BadRequest(new AuthResponse(false, null, string.Join("; ", result.Errors.Select(e => e.Description))));
    await signIn.SignInAsync(user, isPersistent: true);
    return Results.Ok(new AuthResponse(true, "cookie", null));
});

app.MapPost("/api/auth/login", async (LoginRequest req, SignInManager<AppUser> signIn) =>
{
    var result = await signIn.PasswordSignInAsync(req.Email, req.Password, isPersistent: true, lockoutOnFailure: false);
    return result.Succeeded
        ? Results.Ok(new AuthResponse(true, "cookie", null))
        : Results.BadRequest(new AuthResponse(false, null, "Invalid email or password"));
});

app.MapPost("/api/auth/logout", async (SignInManager<AppUser> signIn) =>
{
    await signIn.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/api/auth/me", async (ClaimsPrincipal principal, UserManager<AppUser> userManager) =>
{
    var user = await userManager.GetUserAsync(principal);
    if (user == null) return Results.Unauthorized();
    return Results.Ok(new ProfileDto(user.Email!, user.DisplayName, user.AvatarEmoji,
        user.DailyGoalMinutes, user.SourceLanguage, user.TargetLanguage, user.DarkMode));
}).RequireAuthorization();

// === PROFILE ===
app.MapPut("/api/profile", async (UpdateProfileRequest req, ClaimsPrincipal principal, UserManager<AppUser> userManager) =>
{
    var user = await userManager.GetUserAsync(principal);
    if (user == null) return Results.Unauthorized();
    if (req.DisplayName != null) user.DisplayName = req.DisplayName;
    if (req.AvatarEmoji != null) user.AvatarEmoji = req.AvatarEmoji;
    if (req.DailyGoalMinutes.HasValue) user.DailyGoalMinutes = req.DailyGoalMinutes.Value;
    if (req.SourceLanguage != null) user.SourceLanguage = req.SourceLanguage;
    if (req.TargetLanguage != null) user.TargetLanguage = req.TargetLanguage;
    if (req.DarkMode.HasValue) user.DarkMode = req.DarkMode.Value;
    await userManager.UpdateAsync(user);
    return Results.Ok(new ProfileDto(user.Email!, user.DisplayName, user.AvatarEmoji,
        user.DailyGoalMinutes, user.SourceLanguage, user.TargetLanguage, user.DarkMode));
}).RequireAuthorization();

// === TOPICS & LESSONS ===
app.MapGet("/api/topics", async (ClaimsPrincipal principal, UserManager<AppUser> userManager, LearningService svc) =>
{
    var user = await userManager.GetUserAsync(principal);
    return Results.Ok(await svc.GetTopicsAsync(user?.Id));
});

app.MapGet("/api/topics/{topicId}/lessons", async (int topicId, ClaimsPrincipal principal, UserManager<AppUser> userManager, LearningService svc) =>
{
    var user = await userManager.GetUserAsync(principal);
    return Results.Ok(await svc.GetLessonsAsync(topicId, user?.Id));
});

app.MapGet("/api/lessons/{lessonId}/exercises", async (int lessonId, LearningService svc) =>
    Results.Ok(await svc.GetExercisesAsync(lessonId)));

app.MapPost("/api/lessons/complete", async (LessonCompleteRequest req, ClaimsPrincipal principal, UserManager<AppUser> userManager, LearningService svc) =>
{
    var user = await userManager.GetUserAsync(principal);
    if (user == null) return Results.Unauthorized();
    return Results.Ok(await svc.CompleteLessonAsync(user.Id, req));
}).RequireAuthorization();

app.MapPost("/api/lessons/coach", async (
    AiCoachRequest req,
    ClaimsPrincipal principal,
    UserManager<AppUser> userManager,
    AiCoachService svc,
    CancellationToken cancellationToken) =>
{
    var user = await userManager.GetUserAsync(principal) ?? new AppUser();

    try
    {
        return Results.Ok(await svc.GetCoachResponseAsync(req, user, cancellationToken));
    }
    catch (ArgumentException ex)
    {
        return Results.Problem(
            title: "Invalid AI coach request",
            detail: ex.Message,
            statusCode: StatusCodes.Status400BadRequest);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.Problem(
            title: "Exercise not found",
            detail: ex.Message,
            statusCode: StatusCodes.Status404NotFound);
    }
    catch (AiCoachConfigurationException ex)
    {
        return Results.Problem(
            title: "LeLe AI Coach is unavailable",
            detail: ex.Message,
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
    catch (AiCoachResponseFormatException ex)
    {
        return Results.Problem(
            title: "Unexpected AI coach response",
            detail: ex.Message,
            statusCode: StatusCodes.Status502BadGateway);
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem(
            title: "AI coach provider error",
            detail: ex.Message,
            statusCode: StatusCodes.Status502BadGateway);
    }
});

// === DASHBOARD & DATA ===
app.MapGet("/api/dashboard", async (ClaimsPrincipal principal, UserManager<AppUser> userManager, DashboardService svc) =>
{
    var user = await userManager.GetUserAsync(principal);
    if (user == null) return Results.Unauthorized();
    return Results.Ok(await svc.GetDashboardAsync(user.Id));
}).RequireAuthorization();

app.MapGet("/api/wordbank", async (ClaimsPrincipal principal, UserManager<AppUser> userManager, DashboardService svc) =>
{
    var user = await userManager.GetUserAsync(principal);
    if (user == null) return Results.Unauthorized();
    return Results.Ok(await svc.GetWordBankAsync(user.Id));
}).RequireAuthorization();

app.MapGet("/api/achievements", async (ClaimsPrincipal principal, UserManager<AppUser> userManager, DashboardService svc) =>
{
    var user = await userManager.GetUserAsync(principal);
    if (user == null) return Results.Unauthorized();
    return Results.Ok(await svc.GetAllAchievementsAsync(user.Id));
}).RequireAuthorization();

app.MapGet("/", () => "LangLe API is running! 🦜");

app.MapDefaultEndpoints();
app.Run();
