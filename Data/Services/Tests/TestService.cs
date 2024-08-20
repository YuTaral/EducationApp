using EducationApp.Data.Models.Questions;
using EducationApp.Data.Models;
using eUni.Data;
using EducationApp.Data.Services.Tests.Models;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;

namespace EducationApp.Data.Services.Tests
{
    public class TestService : ITestService
    {
        private readonly EduAppDbContext DBAccess;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TestService(EduAppDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            DBAccess = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public int Create(int lessonId) {

            var test = new Test { LessonId = lessonId };

            DBAccess.Tests.Add(test);
            DBAccess.SaveChanges();

            return test.Id;
        }


#pragma warning disable CS8601 // we are sure question will always have test id so disable the warning
        public AnswerQuestionFormModel GetAnswerQuestionFormModel(int testId, int sequence, string studentId) {

            var question = DBAccess.Questions.Where(q => q.TestId == testId && q.Sequence == sequence).FirstOrDefault();

            if (question != null) {
                var test = DBAccess.Tests.Find(testId);
                var lesson = DBAccess.Lessons.Where(l => l.Id == test.LessonId).FirstOrDefault();

                var questonAnswer = DBAccess.QuestionAnswers
                                    .Where(q => q.QuestionId == question.Id && q.StudentId == studentId)
                                    .Select(q => new AnswerQuestionFormModel
                                    {
                                        Id = q.Id,
                                        QuestionId = q.Id,
                                        Answer = q.Answer,
                                        StudentId = studentId,
                                        QuestionText = question.Text,
                                        QuestionType = question.Type,
                                        Sequence = question.Sequence,
                                        TestId = testId,
                                        StartsAt = lesson.FromDateTime,
                                        EndsAt = lesson.ToDateTime,
                                    }).FirstOrDefault();

                if (questonAnswer != null) {

                    questonAnswer.AllQuestionsCount = DBAccess.Questions.Where(q => q.TestId == testId).ToList().Count();
                    questonAnswer.PreviousExists = sequence > 1;
                    questonAnswer.NextExists = questonAnswer.AllQuestionsCount > sequence;

                    if (questonAnswer.QuestionType == DataConstants.QTypeMulti || questonAnswer.QuestionType == DataConstants.QTypeSingle)
                    {
                        var answers = DBAccess.Answers.Where(a => a.QuestionId == question.Id).ToList();

                        foreach (var a in answers)
                        {
                            questonAnswer.PossibleAnswers += a.Id + "_" +  a.AnswerText + "|";
                        }

                        questonAnswer.PossibleAnswers = questonAnswer.PossibleAnswers.Substring(0, questonAnswer.PossibleAnswers.Length - 1);
                    }

                    return questonAnswer;
                }
            }
            return null;
        }



#pragma warning restore CS8601 // Possible null reference assignment.


        public int CreateQuestionAnswersForStudent(int testId, string studentId) {
            var questions = DBAccess.Questions.Where(q => q.TestId == testId).ToList();
            var questionIds = questions.Select(q => q.Id);

            if (questions != null) {
                var questionsExist = DBAccess.QuestionAnswers.Where(q => questionIds.Contains(q.QuestionId) && q.StudentId == studentId).ToList();

                if (questionsExist.Count() == 0) {

                    foreach (var q in questions) {
                        var questionAnswer = new QuestionAnswer
                        {
                           QuestionId = q.Id,
                           StudentId = studentId,
                           Points = 0
                        };

                        DBAccess.QuestionAnswers.Add(questionAnswer);
                        DBAccess.SaveChanges();
                    }

                    return 1;
                }
            }

            return 0;
        }


        public int SubmitTest(int testId, string studentId) {
            try {
                var grade = new Grade
                {
                    TestId = testId,
                    StudentId = studentId,
                    Result = 0,
                    TotalPoints = 0,
                    IsSubmited = false
                };

                DBAccess.Grades.Add(grade);
                DBAccess.SaveChanges();

                var test = DBAccess.Tests.Where(t => t.Id == testId).FirstOrDefault();

                if (test != null)
                {
                    return test.LessonId;
                }
                return 0;
            }
            catch (Exception e) {
                return 0;
            }
        }
    

        public bool AnswerQuestion(int id, string answer) {
            var answerQuestion = DBAccess.QuestionAnswers.Find(id);

            if (answerQuestion != null)
            {
                answerQuestion.Answer = answer;
                DBAccess.SaveChanges();
                return true;
            }

            return false;
        }


        public CheckParticipantsModel GetTestParticipants(int testId)
        {
            var model = new CheckParticipantsModel();
            model.Participants = new List<TestParticipantModel>();

            var grades = DBAccess.Grades.Where(g => g.TestId == testId).ToList();

            if (grades != null) {

                foreach (var g in grades) {

                    var student = DBAccess.Users.Where(s => s.Id == g.StudentId).FirstOrDefault();
                    if (student != null) {

                        var participant = new TestParticipantModel
                        {
                            TestId = testId,
                            StudentId = student.Id,
                            UserName = student.UserName,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            FacultyNumber = student.FacultyNumber,
                            Points = (double) g.TotalPoints,
                            Percent = g.Result,
                            IsGradeSubmitted = g.IsSubmited
                        };

                        model.Participants.Add(participant);
                    }
                }
            }

            return model;
        }


        public bool SubmitGrade(int id, double percent) {
            var grade = DBAccess.Grades.Find(id);

            if (grade != null) {

                grade.Result = percent;
                grade.IsSubmited = true;
                DBAccess.SaveChanges();

                return true;
            }

            return false;
        }


        public List<QuestionType> GetQuestionTypes()
        {

            var questionTypes = DBAccess.QuestionTypes
                                .Select(qt => new QuestionType
                                {
                                    Id = qt.Id,
                                    Name = qt.Name
                                }).ToList();

            return questionTypes;

        }


        public int GetSequence(int testId)
        {
            return DBAccess.Questions.Where(q => q.TestId == testId).Count() + 1;
        }


        public List<Question> GetQuestionsForTest(int testId)
        {

            List<Question> allQuestions = DBAccess.Questions.Where(q => q.TestId == testId)
                                            .Select(q => new Question
                                            {
                                                Id = q.Id,
                                                Text = q.Text,
                                                Type = q.Type,
                                                Sequence = q.Sequence,

                                            }).ToList();

            return allQuestions.OrderBy(q => q.Sequence).ToList();
        }


        public int CreateQuestion(string text, int testId, string type, int sequence, string answers, double points)
        {
            var success = false;

            var question = new Question
            {
                Text = text,
                TestId = testId,
                Type = type,
                Sequence = sequence,
                Points = points
            };

            DBAccess.Questions.Add(question);
            DBAccess.SaveChanges();

            if (type == DataConstants.QTypeSingle || type == DataConstants.QTypeMulti)
            {
                success = CreateQuestionAnswers(question, answers, type);
            }
            else
            {
                success = true;
            }

            return success ? question.Id : 0;
        }


#pragma warning disable CS8601 // Disable the warning as we are sure that the question will always have test id
        public QuestionFormModel? GetCreateQuestionFormModel(int id, string type)
        {
            var question = DBAccess.Questions.Where(q => q.Id == id)
                            .Select(q => new QuestionFormModel
                            {
                                Id = q.Id,
                                Type = type,
                                Text = q.Text,
                                Sequence = q.Sequence,
                                Test = DBAccess.Tests.Where(t => q.TestId == t.Id).Select(t => new TestFormModel
                                {
                                    Id = t.Id,
                                    LessonId = t.LessonId
                                }).FirstOrDefault(),
                                EditQuestion = true,
                                Points = q.Points
                            }).FirstOrDefault();


            if (question != null)
            {
                var allQuestions = GetQuestionsForTest(question.Test.Id);
                question.Test.Questions = allQuestions;
                question.QuestionTypes = GetQuestionTypes();

                if (question.Type == DataConstants.QTypeMulti || question.Type == DataConstants.QTypeSingle)
                {
                    setQuestionAnswers(question);
                }
            }

            return question == null ? null : question;
        }
#pragma warning restore CS8601 // Possible null reference assignment.


        public bool EditQuestion(int id, string text, string answer, string type, double points)
        {
            var questionData = DBAccess.Questions.Find(id);

            if (questionData == null)
            {
                return false;
            }

            questionData.Text = text;
            questionData.Points = points;

            if (type == DataConstants.QTypeMulti || type == DataConstants.QTypeSingle)
            {
                var editted = editQuestionAnswers(questionData, answer, type);

                if (!editted) {
                    return false;
                }
            }

            DBAccess.SaveChanges();

            return true;
        }


        public bool RemoveQuestion(int id, string type)
        {
            var question = DBAccess.Questions.Find(id);

            if (question == null)
            {
                return false;
            }

            DBAccess.Questions.Remove(question);

            UpdateQuestionsSequence(question);

            if (type == DataConstants.QTypeMulti || type == DataConstants.QTypeSingle)
            {
                removeQuestionAnswers(question);
            }

            DBAccess.SaveChanges();

            return true;
        }


        private void UpdateQuestionsSequence(Question question)
        {
            // Update the sequence of all questions for this test if neccessary
            var allQuestions = GetQuestionsForTest(question.TestId);
            var startingIndex = allQuestions.Count - 1;

            if (question.Sequence <= allQuestions.Count)
            {
                for (var i = startingIndex; i >= question.Sequence; i--)
                {
                    var currentQuestion = allQuestions[i];

                    var questionToUpdate = DBAccess.Questions.Find(currentQuestion.Id);

                    if (questionToUpdate != null)
                    {
                        questionToUpdate.Sequence -= 1;
                    }
                }
            }
            DBAccess.SaveChanges();
        }

        private bool CreateQuestionAnswers(Question question, string answers, string type)
        {
            try
            {
                var possibleAnswers = answers.Split("|", StringSplitOptions.None);

                for (int i = 0; i < possibleAnswers.Length; i++)
                {
                    var isCrrectAnswerText = possibleAnswers[i].Split("_", StringSplitOptions.None);
                    var isCorrect = isCrrectAnswerText[0] == "true";
                    var answerText = isCrrectAnswerText[1];

                    if (answerText.IsNullOrEmpty() || isCrrectAnswerText[0].IsNullOrEmpty() || IsTextInvalid(answerText)) {
                        return false;
                    }

                    var answer = new Answer
                    {
                        AnswerText = answerText,
                        IsCorrect = isCorrect,
                        QuestionId = question.Id
                    };

                    DBAccess.Answers.Add(answer);
                }

                DBAccess.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private bool IsTextInvalid(string text) {

            if (text.Contains("#") || text.Contains('|') || text.Contains("_")) {
                return true;
            }

            return false;
        }


        private bool editQuestionAnswers(Question question, string newAnswers, string type)
        {
            var oldAnswers = DBAccess.Answers.Where(a => a.QuestionId == question.Id).ToList();
            var answersFormat = "";
         
            foreach (var answer in oldAnswers)
            {
                answersFormat += answer.IsCorrect + "_" + answer.AnswerText + "|";
            }

            if (answersFormat != newAnswers)
            {
                removeQuestionAnswers(question);

                var createdAnswers = CreateQuestionAnswers(question, newAnswers, type);

                if (!createdAnswers) {
                    return false;
                }
            }

            return true;
        }


        private void setQuestionAnswers(QuestionFormModel question)
        {
            var answers = DBAccess.Answers.Where(a => a.QuestionId == question.Id).ToList();

            if (answers.Count > 0)
            {
                question.AnswersCount = answers.Count();

                foreach (var answer in answers)
                {
                    question.PossibleAnswers += answer.IsCorrect + "_" + answer.AnswerText + "|";
                }

                question.PossibleAnswers = question.PossibleAnswers.Substring(0, question.PossibleAnswers.Length - 1);
            }
        }


        private void populateSelectableQuestionData(RateAnswerFormModel question, QuestionAnswer answer)
        {
            var answers = DBAccess.Answers.Where(a => a.QuestionId == question.Id).ToList();

            if (answers.Count > 0)
            {
                var allCorrectAnswers = 0;

                foreach (var a in answers)
                {
                    bool isSelected;

                    question.PossibleAnswers += a.Id + "#" + a.IsCorrect + "_" + a.AnswerText + "|";

                    if (answer.Answer == null) {
                        isSelected = false;
                    }
                    else {
                        isSelected = answer.Answer.Contains(a.Id.ToString());
                    }
                    

                    if (a.IsCorrect)
                    {
                        allCorrectAnswers++;
                    }

                    if (a.IsCorrect && isSelected)
                    {
                        question.CorrectAnswersCount++;
                    }
                    else if (isSelected)
                    {
                        question.WrongAnswersCount++;
                    }
                }

                if (question.CorrectAnswersCount > 0 && question.GivenPoints == 0)
                {
                    if (question.QuestionType == DataConstants.QTypeSingle)
                    {
                        question.GivenPoints = question.MaxPoints;
                    }
                    else
                    {
                        var pointsPerAnswer = question.MaxPoints / allCorrectAnswers;
                        question.GivenPoints = Math.Round(pointsPerAnswer * question.CorrectAnswersCount - pointsPerAnswer * question.WrongAnswersCount, 2);
                        if (question.GivenPoints < 0)
                        {
                            question.GivenPoints = 0;
                        }
                    }
                }


                question.PossibleAnswers = question.PossibleAnswers.Substring(0, question.PossibleAnswers.Length - 1);
            }
        }


        private void removeQuestionAnswers(Question question)
        {
            var answers = DBAccess.Answers.Where(a => a.QuestionId == question.Id).ToList();

            if (answers.Count > 0)
            {
                foreach (var a in answers)
                {
                    var answer = DBAccess.Answers.Find(a.Id);

                    if (answer != null)
                    {
                        DBAccess.Answers.Remove(answer);
                    }
                }
            }
        }


        public TestParticipantModel GetStudentAnswer(string studentId, int testId, int sequence)
        {

            var grade = DBAccess.Grades.Where(g => g.TestId == testId && g.StudentId == studentId).FirstOrDefault();

            if (grade != null)
            {
                var student = DBAccess.Users.Where(s => s.Id == studentId).FirstOrDefault();
                if (student != null)
                {

                    var participant = new TestParticipantModel
                    {
                        TestId = testId,
                        StudentId = student.Id,
                        UserName = student.UserName,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        FacultyNumber = student.FacultyNumber,
                        GradeId = grade.Id
                    };

                    var test = DBAccess.Tests.Where(t => t.Id == testId).FirstOrDefault();

                    if (test != null)
                    {
                        var lesson = DBAccess.Lessons.Where(l => l.Id == test.LessonId).FirstOrDefault();

                        if (lesson != null)
                        {
                            var question = DBAccess.Questions.Where(q => q.TestId == testId && q.Sequence == sequence)
                                                           .Select(q => new RateAnswerFormModel
                                                           {
                                                               Id = q.Id,
                                                               QuestionId = q.Id,
                                                               StudentId = studentId,
                                                               QuestionText = q.Text,
                                                               QuestionType = q.Type,
                                                               Sequence = q.Sequence,
                                                               TestId = testId,
                                                               MaxPoints = q.Points
                                                           })
                                                           .FirstOrDefault();

                            if (question != null)
                            {
                                question.AllQuestionsCount = DBAccess.Questions.Where(q => q.TestId == testId).ToList().Count();
                                question.PreviousExists = sequence > 1;
                                question.NextExists = question.AllQuestionsCount > sequence;
                                question.GivenPoints = DBAccess.QuestionAnswers
                                                        .Where(q => q.StudentId == studentId && q.QuestionId == question.QuestionId)
                                                        .Select(q => q.Points).FirstOrDefault();

                                var questionAnswer = DBAccess.QuestionAnswers.Where(a => a.StudentId == student.Id && a.QuestionId == question.Id).FirstOrDefault();

                                if (questionAnswer != null)
                                {
                                    participant.CurrentQuestionAnswer = questionAnswer;

                                    if (question.QuestionType == DataConstants.QTypeMulti || question.QuestionType == DataConstants.QTypeSingle)
                                    {
                                        populateSelectableQuestionData(question, participant.CurrentQuestionAnswer);
                                    }

                                    participant.RateCurrentQuestionAnswer = question;

                                    return participant;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }


        public bool RateStudentAnswer(int id, int gradeId, double? points)
        {
            var answer = DBAccess.QuestionAnswers.Find(id);

            if (answer != null)
            {

                double? oldPoints = -1;

                if (answer.Points != 0)
                {
                    oldPoints = answer.Points;
                }

                answer.Points = points;

                var grade = DBAccess.Grades.Where(g => g.Id == gradeId).FirstOrDefault();

                if (grade != null)
                {
                    var gradeToUpdate = DBAccess.Grades.Find(grade.Id);

                    if (gradeToUpdate != null)
                    {
                        double? newGradePoints = gradeToUpdate.TotalPoints;

                        if (oldPoints != -1)
                        {
                            newGradePoints += points - oldPoints;
                        }
                        else
                        {
                            newGradePoints += points;
                        }
                        gradeToUpdate.TotalPoints = newGradePoints;

                    }

                }
                DBAccess.SaveChanges();
                return true;
            }

            return false;
        }


        public bool SetGradeResultString(string studentId, int testId, TestParticipantModel model)
        {
            var questions = DBAccess.Questions.Where(q => q.TestId == testId).ToList();

            if (questions.Count > 0)
            {
                var questionIds = questions.Select(q => q.Id).ToList();
                var allAnswers = DBAccess.QuestionAnswers.Where(a => a.StudentId == studentId && questionIds.Contains(a.QuestionId)).ToList();
                double maxPoints = questions.Sum(q => q.Points);
                double grantedPoints = (double) allAnswers.Sum(a => a.Points);
                double percentPerPoint = Math.Round(100 / maxPoints, 2);
                double resultInPercents = Math.Round((double)(percentPerPoint * grantedPoints), 2);

                if (resultInPercents > 100 || maxPoints == grantedPoints) {
                    resultInPercents = 100;
                }

                model.MaxPoints = maxPoints;
                model.Points = grantedPoints;
                model.Percent = resultInPercents;

                return true;
            }

            return false;
        }


        public bool CanEditTest(int lessonId) {
            var lesson = DBAccess.Lessons.Where(l => l.Id == lessonId).FirstOrDefault();

            if (lesson != null)
            {
                return lesson.FromDateTime > DateTime.Now;
            }

            return false;
        }


        public bool IsOwnerOfLesson(int id, string userId)
        {
            var lesson = DBAccess.Lessons.Where(l => l.Id == id).FirstOrDefault();

            if (lesson != null)
            {
                var course = DBAccess.Courses.Where(c => c.Id == lesson.CourseId).FirstOrDefault();

                if (course != null)
                {
                    return course.TeacherId == userId;
                }
            }
            return false;
        }
    }
}
