namespace EducationApp.Data.Services.Tests.Models
{
    public class AnswerQuestionFormModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string? Answer { get; set; }
        public string? PossibleAnswers { get; set; }
        public string? StudentId { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public int Sequence { get; set; }
        public int AllQuestionsCount { get; set; }
        public int TestId { get; set; }
        public DateTime EndsAt { get; set; }
        public DateTime StartsAt { get; set; }
        public bool NextExists { get; set; }
        public bool PreviousExists { get; set; }
        public bool AskForSubmit { get; set; }
    }
}
