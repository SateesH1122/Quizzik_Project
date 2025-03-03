using Microsoft.EntityFrameworkCore;

namespace Quizzik_Project.Models
{
    public class EFCoreDbContext : DbContext
    {
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Leaderboard> Leaderboards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //Each quiz will have its own Leaderboard
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Leaderboard)
                .WithOne(l => l.Quiz)
                .HasForeignKey<Leaderboard>(l => l.QuizID)
                .OnDelete(DeleteBehavior.Cascade);
            
            // A Quiz can have multiple questions
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizID)
                .OnDelete(DeleteBehavior.Cascade);

            //A Question can have many options
            modelBuilder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionID)
                .OnDelete(DeleteBehavior.Cascade);

            // A Quiz can be attempted multiple times
            modelBuilder.Entity<QuizAttempt>()
                .HasOne(qa => qa.Quiz)
                .WithMany(q => q.QuizAttempts)
                .HasForeignKey(qa => qa.QuizID)
                .OnDelete(DeleteBehavior.Cascade);

            
            //An user can attempt multiple Quiz Attempts
            modelBuilder.Entity<QuizAttempt>()
                .HasOne(qa => qa.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(qa => qa.UserID)
                .OnDelete(DeleteBehavior.NoAction); // Change to NoAction to avoid multiple cascade paths

            //Each quiz will have its own Leaderboard
            modelBuilder.Entity<Leaderboard>()
                .HasOne(l => l.Quiz)
                .WithOne(q => q.Leaderboard)
                .HasForeignKey<Leaderboard>(l => l.QuizID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();
        }
    }
}