using LangLe.ApiService.Data;
using LangLe.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LangLe.ApiService.Services;

public class DashboardService(LangLeDbContext db)
{
    public async Task<DashboardDto> GetDashboardAsync(string userId)
    {
        var streak = await db.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
        var progress = await db.UserProgress.Where(p => p.UserId == userId).ToListAsync();
        var wordsLearned = await db.UserWordBank.CountAsync(w => w.UserId == userId);
        var totalXp = progress.Sum(p => p.XpEarned);
        var todayXp = progress.Where(p => p.CompletedAt.Date == DateTime.UtcNow.Date).Sum(p => p.XpEarned);

        var level = totalXp switch
        {
            >= 5000 => "Advanced",
            >= 2000 => "Intermediate",
            >= 500 => "Elementary",
            _ => "Beginner"
        };

        // Suggested next lesson
        var completedLessonIds = progress.Select(p => p.LessonId).ToHashSet();
        var nextLesson = await db.Lessons
            .Include(l => l.Topic)
            .OrderBy(l => l.Topic.SortOrder).ThenBy(l => l.SortOrder)
            .FirstOrDefaultAsync(l => !completedLessonIds.Contains(l.Id));

        var suggestion = nextLesson != null
            ? new LessonSuggestionDto(nextLesson.Id, nextLesson.TopicId, nextLesson.Topic.Name, nextLesson.Title, nextLesson.Topic.IconEmoji)
            : null;

        // Weekly XP
        var weekAgo = DateTime.UtcNow.Date.AddDays(-6);
        var weeklyData = progress.Where(p => p.CompletedAt.Date >= weekAgo)
            .GroupBy(p => p.CompletedAt.Date)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.XpEarned));

        var weeklyXp = Enumerable.Range(0, 7)
            .Select(i => weekAgo.AddDays(i))
            .Select(d => new WeeklyXpDto(d.ToString("ddd"), weeklyData.GetValueOrDefault(d, 0)))
            .ToList();

        // Goals
        var goals = await db.UserGoals.Where(g => g.UserId == userId).ToListAsync();
        var goalDtos = goals.Select(g => new GoalProgressDto(
            g.Id, g.Description, g.TargetValue, g.CurrentValue,
            g.TargetValue > 0 ? Math.Round(g.CurrentValue * 100.0 / g.TargetValue, 1) : 0)).ToList();

        // Recent achievements
        var achievements = await db.UserAchievements
            .Include(a => a.Achievement)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.UnlockedAt)
            .Take(5)
            .Select(a => new AchievementDto(a.Achievement.Id, a.Achievement.Name, a.Achievement.Description,
                a.Achievement.IconEmoji, true, a.UnlockedAt))
            .ToListAsync();

        return new DashboardDto(
            streak?.CurrentStreak ?? 0, streak?.LongestStreak ?? 0,
            totalXp, todayXp, wordsLearned, progress.Count, level,
            suggestion, weeklyXp, goalDtos, achievements);
    }

    public async Task<List<WordBankEntryDto>> GetWordBankAsync(string userId)
    {
        return await db.UserWordBank
            .Include(w => w.WordEntry)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.LearnedAt)
            .Select(w => new WordBankEntryDto(
                w.WordEntry.Id, w.WordEntry.English, w.WordEntry.Spanish,
                w.WordEntry.Telugu, w.WordEntry.ImageUrl, w.LearnedAt))
            .ToListAsync();
    }

    public async Task<List<AchievementDto>> GetAllAchievementsAsync(string userId)
    {
        var all = await db.Achievements.ToListAsync();
        var unlocked = await db.UserAchievements
            .Where(a => a.UserId == userId)
            .ToDictionaryAsync(a => a.AchievementId, a => a.UnlockedAt);

        return all.Select(a => new AchievementDto(
            a.Id, a.Name, a.Description, a.IconEmoji,
            unlocked.ContainsKey(a.Id), unlocked.GetValueOrDefault(a.Id))).ToList();
    }
}
