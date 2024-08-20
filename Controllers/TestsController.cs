using EducationApp.Data.Services.Tests;
using eUni.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EducationApp.Data.Services.Tests.Models;
using EducationApp.Data.Models.Questions;
using EducationApp.Data.Services.User.Models;
using EducationApp.Data.Services.User;

namespace EducationApp.Controllers
{
    public class TestsController : BaseController
    {
        private readonly ITestService testService;

        public TestsController(ITestService service)
        {
            testService = service;
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Add(int id, int lessonId, int questionId, string type)
        {
            try {
                // Check whether the test has started
                if (!testService.CanEditTest(lessonId))
                {
                    return ReturnCustomError(DataConstants.ErrorTestAlreadyStarted, DataConstants.Unavailable);
                }

                // When entering add test page, create test to get test ID
                var testId = 0;

                if (id == 0)
                {
                    testId = testService.Create(lessonId);
                }
                else
                {
                    testId = id;
                }

                if (lessonId != 0 && type != null)
                {
                    var question = testService.GetCreateQuestionFormModel(questionId, type);

                    if (question != null)
                    {
                        return View(question);
                    }
                }

                var qSequence = testService.GetSequence(id);
                var qTypes = testService.GetQuestionTypes();


                var model = new QuestionFormModel
                {
                    QuestionTypes = qTypes,
                    Sequence = qSequence,
                    Type = qTypes[0].Name,
                    Test = new TestFormModel
                    {
                        Id = testId,
                        Questions = testService.GetQuestionsForTest(testId),
                        LessonId = lessonId
                    },
                    AnswersCount = DataConstants.DefaultAnswersCount,
                    EditQuestion = false
                };

                return View(model);
            }
            catch (Exception) {
                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Add(QuestionFormModel question, int lessonId)
        {
            try
            {

                if (question.EditQuestion)
                {
                    var success = testService.EditQuestion(question.Id, question.Text, question.PossibleAnswers, question.Type, question.Points);

                    if (success)
                    {
                        return RedirectToAction("Add", "Tests", new { id = question.Test.Id, lessonId });
                    }

                    ReturnCustomError(DataConstants.ErrorEditQuestion, DataConstants.UnexpectedError);
                }

                question.QuestionTypes = testService.GetQuestionTypes();

                if (!ValidateQuestionType(question.QuestionTypes, question.Type))
                {
                    AddQuestionsToResponse(question);

                    return ReturnCustomError(DataConstants.ErrorInvalidQuestionType, DataConstants.InvalidValue);
                }

                if (!ModelState.IsValid)
                {
                    AddQuestionsToResponse(question);

                    return View(question);
                }

                var successId = testService.CreateQuestion(question.Text, question.Test.Id, question.Type, question.Sequence, question.PossibleAnswers, question.Points);

                if (successId == 0) {
                    return ReturnCustomError(DataConstants.ErrorCreateQuestion, DataConstants.UnexpectedError);
                }

                return RedirectToAction("Add", "Tests", new { id = question.Test.Id, lessonId });
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult EditQuestion(int testId, int lessonId, int questionId, string type)
        {
            try
            {
                return RedirectToAction("Add", "Tests", new { id = testId, lessonId, questionId, type });
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult RemoveQuestion(int testId, int lessonId, int questionId, string type)
        {
            try
            {
                if (!testService.IsOwnerOfLesson(lessonId, GetUserId()))
                {
                    return ReturnCustomError(DataConstants.ErrorNotOwner, DataConstants.Unauthorized);
                }

                var success = testService.RemoveQuestion(questionId, type);

                if (!success)
                {
                    ReturnCustomError(DataConstants.ErrorRemoveQuestion, DataConstants.UnexpectedError);
                }

                return RedirectToAction("Add", "Tests", new { id = testId, lessonId });
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize(Roles = DataConstants.StudentRoleName)]
        public IActionResult AnswerQuestion(int testId, int sequence, int submit)
        {
            try
            {
                if (sequence == 1)
                {
                    testService.CreateQuestionAnswersForStudent(testId, GetUserId());
                }

                var question = testService.GetAnswerQuestionFormModel(testId, sequence, GetUserId());

                if (question == null)
                {
                    return ReturnCustomError(DataConstants.ErrorUnexpectedAnsweringQ, DataConstants.UnexpectedError);
                }
                else
                {
                    if (question.EndsAt < DateTime.Now || question.StartsAt > DateTime.Now)
                    {
                        return ReturnCustomError(DataConstants.ErrorCantAccessTestNow, DataConstants.Unavailable);
                    }

                    if (submit == 1)
                    {
                        question.AskForSubmit = true;
                    }
                    else
                    {
                        question.AskForSubmit = false;
                    }

                    return View(question);
                }
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.StudentRoleName)]
        public IActionResult AnswerQuestion(AnswerQuestionFormModel model)
        {
            try
            {
                var success = testService.AnswerQuestion(model.Id, model.Answer);

                if (success)
                {
                    var redirectToQuestion = model.Sequence + 1;

                    if (redirectToQuestion > model.AllQuestionsCount)
                    {
                        redirectToQuestion = model.Sequence;
                        return RedirectToAction("AnswerQuestion", "Tests", new { testId = model.TestId, sequence = redirectToQuestion, submit = 1 });
                    }

                    return RedirectToAction("AnswerQuestion", "Tests", new { testId = model.TestId, sequence = redirectToQuestion });
                }
                else
                {
                    return ReturnCustomError(DataConstants.ErrorSavingQuestion, DataConstants.UnexpectedError);
                }
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize(Roles = DataConstants.StudentRoleName)]
        public IActionResult SubmitTest(int testId)
        {
            try
            {
                var lessonId = testService.SubmitTest(testId, GetUserId());

                if (lessonId != 0)
                {
                    return RedirectToAction("Details", "Lessons", new { id = lessonId });
                }

                return ReturnCustomError(DataConstants.ErrorUnexpectedWhileSubmitTest, DataConstants.UnexpectedError);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
            
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult CheckParticipants(int testId)
        {
            try
            {
                var participats = testService.GetTestParticipants(testId);

                return View(participats);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult CheckAnswers(string studentId, int testId, int sequence, int ratedAll)
        {
            try
            {
                var model = testService.GetStudentAnswer(studentId, testId, sequence);

                if (ratedAll == 1)
                {
                    model.ShowResult = true;
                    testService.SetGradeResultString(studentId, testId, model);
                }

                return View(model);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult CheckAnswers(TestParticipantModel model)
        {
            try
            {
                var success = testService.RateStudentAnswer(model.CurrentQuestionAnswer.Id, model.GradeId, model.RateCurrentQuestionAnswer.GivenPoints);

                if (success)
                {
                    var nextExists = model.RateCurrentQuestionAnswer.Sequence + 1 <= model.RateCurrentQuestionAnswer.AllQuestionsCount;

                    if (nextExists)
                    {
                        return RedirectToAction("CheckAnswers", "Tests", new { studentId = model.StudentId, testId = model.TestId, sequence = model.RateCurrentQuestionAnswer.Sequence + 1 });
                    }

                    return RedirectToAction("CheckAnswers", "Tests", new { studentId = model.StudentId, testId = model.TestId, sequence = model.RateCurrentQuestionAnswer.Sequence, ratedAll = 1 });
                }
                else
                {
                    return ReturnCustomError(DataConstants.ErrorUnexpectedWhileRatingQ, DataConstants.UnexpectedError);
                }
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult SubmitGrade(TestParticipantModel model)
        {
            try
            {
                var success = testService.SubmitGrade(model.GradeId, model.Percent);

                if (success)
                {
                    return RedirectToAction("CheckParticipants", "Tests", new { testId = model.TestId });
                }
                else
                {
                    return ReturnCustomError(DataConstants.ErrorUnexpectedWhileSubmittingGrade, DataConstants.UnexpectedError);
                }
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        private Boolean ValidateQuestionType(List<QuestionType> questionTypes, string type) {

            foreach(var q in questionTypes)
            {
                if (q.Name == type) {
                    return true;
                }
            }

            return false;
        }


        private void AddQuestionsToResponse(QuestionFormModel question) {
            List<Question> allQuestions = testService.GetQuestionsForTest(question.Test.Id);
            question.Test.Questions = allQuestions;
        }
        
    }
}
