using LangLe.ApiService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LangLe.ApiService.Data;

public class LangLeDbContext : IdentityDbContext<AppUser>
{
    public LangLeDbContext(DbContextOptions<LangLeDbContext> options) : base(options) { }

    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WordEntry> WordEntries => Set<WordEntry>();
    public DbSet<UserProgress> UserProgress => Set<UserProgress>();
    public DbSet<UserStreak> UserStreaks => Set<UserStreak>();
    public DbSet<UserGoal> UserGoals => Set<UserGoal>();
    public DbSet<UserWordBank> UserWordBank => Set<UserWordBank>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserStreak>()
            .HasIndex(s => s.UserId).IsUnique();

        builder.Entity<UserProgress>()
            .HasIndex(p => new { p.UserId, p.LessonId }).IsUnique();

        builder.Entity<UserWordBank>()
            .HasIndex(w => new { w.UserId, w.WordEntryId }).IsUnique();

        builder.Entity<UserAchievement>()
            .HasIndex(a => new { a.UserId, a.AchievementId }).IsUnique();

        builder.Entity<Exercise>()
            .Property(e => e.Type)
            .HasConversion<string>();

        builder.Entity<UserGoal>()
            .Property(g => g.GoalType)
            .HasConversion<string>();
    }
}
