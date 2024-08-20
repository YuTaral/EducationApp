namespace EducationApp.Data.Services.Tests.Models
{
    public class RateAnswerFormModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string? PossibleAnswers { get; set; }
        public string? StudentId { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public int Sequence { get; set; }
        public int AllQuestionsCount { get; set; }
        public int TestId { get; set; }
        public bool NextExists { get; set; }
        public bool PreviousExists { get; set; }
        public double MaxPoints { get; set; }
        public double? GivenPoints { get; set; }
        public int CorrectAnswersCount { get; set; }
        public int WrongAnswersCount { get; set; }
    }
}
