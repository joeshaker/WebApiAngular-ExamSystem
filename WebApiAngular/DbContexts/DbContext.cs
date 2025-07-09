using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAngular.DtoModel;
using WebApiAngular.Models;

namespace WebApiAngular.DbContexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options) { }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Answer> Answers { get; set; }

        public DbSet<RevokedToken> RevokedTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RevokedToken>()
            .HasKey(rt => rt.Id);

            // Exam relationships
            builder.Entity<Exam>()
                .HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Question relationships
            builder.Entity<Question>()
                .HasOne(q => q.Exam)
                .WithMany(e => e.Questions)
                .HasForeignKey(q => q.ExamId)
                .OnDelete(DeleteBehavior.Cascade); // Keep this for Exam->Questions

            // Option relationships
            builder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade); // Keep this for Question->Options

            // Result relationships
            builder.Entity<Result>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Result>()
                .HasOne(r => r.Exam)
                .WithMany()
                .HasForeignKey(r => r.ExamId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            // Answer relationships - THIS IS WHERE WE MAKE KEY CHANGES
            builder.Entity<Answer>()
                .HasOne(a => a.Result)
                .WithMany(r => r.Answers)
                .HasForeignKey(a => a.ResultId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

            builder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

            builder.Entity<Answer>()
                .HasOne(a => a.SelectedOption)
                .WithMany()
                .HasForeignKey(a => a.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict
        }



    }
}
