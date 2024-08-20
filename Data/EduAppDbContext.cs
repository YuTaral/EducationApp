using EducationApp.Data.Models;
using EducationApp.Data.Models.Questions;
using eUni.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EducationApp.Data
{
    public class EduAppDbContext : IdentityDbContext<User>
    {

        public EduAppDbContext(DbContextOptions<EduAppDbContext> options)
            : base(options)
        {}

        public DbSet<Course> Courses { get; init; }
        public DbSet<Lesson> Lessons { get; init; }
        public DbSet<LessonType> LessonType { get; init; }
        public DbSet<UploadedFile> UploadedFiles { get; init; }
        public DbSet<Access> Access { get; init; }
        public DbSet<Test> Tests { get; init; }
        public DbSet<QuestionType> QuestionTypes { get; init; }
        public DbSet<Question> Questions { get; init; }
        public DbSet<Answer> Answers { get; init; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; init; }
        public DbSet<Grade> Grades { get; init; }


    }
}