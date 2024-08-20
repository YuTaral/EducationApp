using EducationApp.Data.Models.Questions;

namespace EducationApp.Data.Services.Tests.Models
{
    public class TestParticipantModel
    {
        public int TestId { get; set; }
        public required string StudentId { get; set; }
        public required string UserName { get; set; }
        public int? FacultyNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public RateAnswerFormModel? RateCurrentQuestionAnswer { get; set; }
        public QuestionAnswer? CurrentQuestionAnswer { get; set; }
        public Boolean ShowResult { get; set; }
        public double Points { get; set; }
        public double MaxPoints { get; set; }
        public double Percent { get; set; }
        public int GradeId { get; set; }
        public Boolean IsGradeSubmitted { get; set; }
    }
}
