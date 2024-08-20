using EducationApp.Data.Models;
using EducationApp.Data.Models.Questions;
using EducationApp.Data.Services.Tests.Models;
using eUni.Data.Models;

namespace EducationApp.Data.Services.Tests
{
    public interface ITestService
    {

        public int Create(int lessonId);
        public int CreateQuestionAnswersForStudent(int testId, string studentId);
        public AnswerQuestionFormModel GetAnswerQuestionFormModel(int testId, int sequence, string studentId);
        public bool AnswerQuestion(int id, string answer);
        public int SubmitTest(int testId, string studentId);
        public CheckParticipantsModel GetTestParticipants(int testId);
        public bool SubmitGrade(int id, double percent);
        public List<QuestionType> GetQuestionTypes();
        public int CreateQuestion(string text, int testId, string type, int sequence, string answers, double points);
        public int GetSequence(int testId);
        public QuestionFormModel? GetCreateQuestionFormModel(int id, string type);
        public List<Question> GetQuestionsForTest(int testId);
        public bool EditQuestion(int id, string text, string answers, string type, double points);
        public bool RemoveQuestion(int id, string type);
        public bool IsOwnerOfLesson(int id, string userId);
        public TestParticipantModel GetStudentAnswer(string studentId, int testId, int sequence);
        public bool RateStudentAnswer(int id, int gradeId, double? points);
        public bool SetGradeResultString(string studentId, int testId, TestParticipantModel model);
        public bool CanEditTest(int lessonId);

    }
}
