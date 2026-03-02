using System.Text.Json;
using LangLe.ApiService.Data;
using LangLe.ApiService.Models;
using LangLe.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LangLe.ApiService.Services;

public class LearningService(LangLeDbContext db)
{
    public async Task<List<TopicDto>> GetTopicsAsync(string? userId)
    {
        var topics = await db.Topics
            .Include(t => t.Lessons)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        var completedLessonIds = userId != null
            ? await db.UserProgress.Where(p => p.UserId == userId).Select(p => p.LessonId).ToListAsync()
            : [];

        return topics.Select(t =>
        {
            var total = t.Lessons.Count;
            var completed = t.Lessons.Count(l => completedLessonIds.Contains(l.Id));
            return new TopicDto(t.Id, t.Name, t.Description, t.IconEmoji, t.SortOrder,
                total, completed, total > 0 ? Math.Round(completed * 100.0 / total, 1) : 0);
        }).ToList();
    }

    public async Task<List<LessonDto>> GetLessonsAsync(int topicId, string? userId)
    {
        var lessons = await db.Lessons
            .Where(l => l.TopicId == topicId)
            .Include(l => l.Exercises)
            .OrderBy(l => l.SortOrder)
            .ToListAsync();

        var completedLessonIds = userId != null
            ? await db.UserProgress.Where(p => p.UserId == userId).Select(p => p.LessonId).ToListAsync()
            : [];

        return lessons.Select(l => new LessonDto(
            l.Id, l.Title, l.SortOrder,
            completedLessonIds.Contains(l.Id),
            l.Exercises.Count)).ToList();
    }

    public async Task<List<ExerciseDto>> GetExercisesAsync(int lessonId)
    {
        var exercises = await db.Exercises
            .Where(e => e.LessonId == lessonId)
            .ToListAsync();

        return exercises.Select(e => new ExerciseDto(
            e.Id, e.Type, e.Question, e.CorrectAnswer,
            JsonSerializer.Deserialize<List<string>>(e.OptionsJson) ?? [],
            e.ImageUrl, e.HintText)).ToList();
    }

    public async Task<LessonCompleteResponse> CompleteLessonAsync(string userId, LessonCompleteRequest request)
    {
        var xpEarned = request.CorrectAnswers * 10;
        var stars = request.TotalExercises > 0
            ? (int)Math.Ceiling(request.CorrectAnswers * 3.0 / request.TotalExercises)
            : 0;

        // Save progress (upsert)
        var existing = await db.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == request.LessonId);

        if (existing != null)
        {
            if (xpEarned > existing.XpEarned)
            {
                existing.XpEarned = xpEarned;
                existing.Stars = stars;
                existing.CompletedAt = DateTime.UtcNow;
            }
        }
        else
        {
            db.UserProgress.Add(new UserProgress
            {
                UserId = userId, LessonId = request.LessonId,
                XpEarned = xpEarned, Stars = stars
            });
        }

        // Update streak
        var streak = await db.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
        var today = DateTime.UtcNow.Date;
        if (streak == null)
        {
            streak = new UserStreak { UserId = userId, CurrentStreak = 1, LongestStreak = 1, LastActivityDate = today };
            db.UserStreaks.Add(streak);
        }
        else if (streak.LastActivityDate.Date < today)
        {
            streak.CurrentStreak = streak.LastActivityDate.Date == today.AddDays(-1) ? streak.CurrentStreak + 1 : 1;
            streak.LongestStreak = Math.Max(streak.LongestStreak, streak.CurrentStreak);
            streak.LastActivityDate = today;
        }

        // Add words to word bank
        var lesson = await db.Lessons.Include(l => l.Topic).ThenInclude(t => t.WordEntries)
            .FirstOrDefaultAsync(l => l.Id == request.LessonId);

        var newWords = new List<WordBankEntryDto>();
        if (lesson?.Topic?.WordEntries != null)
        {
            foreach (var word in lesson.Topic.WordEntries)
            {
                if (!await db.UserWordBank.AnyAsync(w => w.UserId == userId && w.WordEntryId == word.Id))
                {
                    db.UserWordBank.Add(new UserWordBank { UserId = userId, WordEntryId = word.Id });
                    newWords.Add(new WordBankEntryDto(word.Id, word.English, word.Spanish, word.Telugu, word.ImageUrl, DateTime.UtcNow));
                }
            }
        }

        await db.SaveChangesAsync();

        // Check achievements
        var newAchievements = await CheckAchievementsAsync(userId);

        return new LessonCompleteResponse(xpEarned, stars, streak.CurrentStreak, newAchievements, newWords);
    }

    private async Task<List<string>> CheckAchievementsAsync(string userId)
    {
        var allAchievements = await db.Achievements.ToListAsync();
        var unlocked = await db.UserAchievements.Where(a => a.UserId == userId).Select(a => a.AchievementId).ToListAsync();
        var newlyUnlocked = new List<string>();

        var lessonsCompleted = await db.UserProgress.CountAsync(p => p.UserId == userId);
        var streak = await db.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
        var wordsLearned = await db.UserWordBank.CountAsync(w => w.UserId == userId);
        var totalXp = await db.UserProgress.Where(p => p.UserId == userId).SumAsync(p => p.XpEarned);
        var topicsCompleted = await CountCompletedTopicsAsync(userId);

        foreach (var a in allAchievements.Where(a => !unlocked.Contains(a.Id)))
        {
            bool earned = a.CriteriaType switch
            {
                "lessons_completed" => lessonsCompleted >= a.CriteriaValue,
                "streak" => (streak?.LongestStreak ?? 0) >= a.CriteriaValue,
                "words_learned" => wordsLearned >= a.CriteriaValue,
                "total_xp" => totalXp >= a.CriteriaValue,
                "topics_completed" => topicsCompleted >= a.CriteriaValue,
                _ => false
            };

            if (earned)
            {
                db.UserAchievements.Add(new UserAchievement { UserId = userId, AchievementId = a.Id });
                newlyUnlocked.Add(a.Name);
            }
        }

        if (newlyUnlocked.Count > 0) await db.SaveChangesAsync();
        return newlyUnlocked;
    }

    private async Task<int> CountCompletedTopicsAsync(string userId)
    {
        var topics = await db.Topics.Include(t => t.Lessons).ToListAsync();
        var completedLessons = await db.UserProgress.Where(p => p.UserId == userId).Select(p => p.LessonId).ToListAsync();
        return topics.Count(t => t.Lessons.All(l => completedLessons.Contains(l.Id)) && t.Lessons.Count > 0);
    }
}
