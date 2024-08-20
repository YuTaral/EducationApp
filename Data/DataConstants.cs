using eUni.Data.Models;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Identity.Client;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eUni.Data
{
    public class DataConstants
    {
        public const string AdministratorRoleName = "Admin";
        public const string TeacherRoleName = "Teacher";
        public const string StudentRoleName = "Student";
 
        public const string LTypeLecture = "Lecture";
        public const string LTypeExercise = "Exercise";
        public const string LTypeTest = "Test";

        public const string QTypeMulti= "Multiple answer";
        public const string QTypeSingle = "Single answer";
        public const string QTypeText = "Text";
        public const string QTypeNumeric = "Numeric";

        public const int CourseTitleMinLen = 1;
        public const int CourseTitleMaxLen = 100;
        public const int CourseDescMinLen = 1;
        public const int CourseDescMaxLen = 4000;

        public const int LessonTitleMinLen = 1;
        public const int LessonTitleMaxLen = 100;
        public const int LessonDescMaxLen = 4000;

        public const int CoursesPerPage = 10;

        public const int FacNumberLen = 6;
        public const int FirstNameMinLen = 2;
        public const int FirstNameMaxLen = 20;
        public const int LastNameMinLen = 2;
        public const int LastNameMaxLen = 20;
        public const int AgeMin = 18;
        public const int AgeMax = 100;

        public const int QuestionMaxAnswers = 30;
        public const int DefaultAnswersCount = 4;

        public const string UnexpectedError = "Unexpected error";
        public const string InvalidValue = "Invalid value";
        public const string Unauthorized = "Unauthorized";
        public const string ResourceNotFound = "Resource not found";
        public const string Unavailable = "Unavailable";

        public const string ErrorWhileProccessingRequest = "Unexpected error occurred while proccessing your request, please check your internet connection and try again";
        public const string ErrorEditQuestion = "Error occurred while trying edit question. Please try again";
        public const string ErrorRemoveQuestion = "Error occurred while trying remove question.Please try again";
        public const string ErrorInvalidQuestionType = "Invalid question type";
        public const string ErrorCreateQuestion = "Error occured while trying to create course";
        public const string ErrorCannotEditCourse = "You are not owner of this course and you cannot edit it";
        public const string ErrorUnexpectedCannotEditCourse = "Error occured while trying to edit course. Please try again";
        public const string ErrorUnauthorized = "You are not authorized to access this page";
        public const string ErrorCannotDeleteCourse = "Error occured while trying to delete course. Please try again";
        public const string ErrorNotOwner = "You are not owner of this resource";
        public const string ErrorUnexpectedGrantAccess = "Error occured while trying to grant access. Please try again";
        public const string ErrorAddLessonCourseNotFound = "Course for which you are trying to add lesson not found";
        public const string ErrorNotAuthorizedToDownload = "You are not authorized to download this file";
        public const string ErroDownloadFileNotFound = "File you are trying to download is not found";
        public const string ErroDeleteFileNotFound = "File you are trying to delete is not found";
        public const string ErrorNotAuthorizedToRemoveFile = "You are not authorized to remove this file";
        public const string ErrorUnexpectedEditLesson = "Error occurred while trying to edit this lesson. Please try again";
        public const string ErrorUnauthorizedToPerformAction = "You are not authorized to perform this action";
        public const string ErrorLessonDoesNotExists = "Lesson does not exists";
        public const string ErrorAccountNotApproved = "Your account is not approved yet";
        public const string ErrorUnexpectedWhileApprovingAcc = "Error occurred while approving accounts. Please check if all acounts which should have been approved are approved";
        public const string ErrorSavingQuestion = "Error occured while saving the question, please try again";
        public const string ErrorCantAccessTestNow = "The test is unavailable at this time";
        public const string ErrorUnexpectedAnsweringQ = "Error occured trying to show question, please try again";
        public const string ErrorUnexpectedWhileSubmitTest = "Error occured trying submit your test, please try again";
        public const string ErrorUnexpectedWhileRatingQ = "Error occured trying to rate question, please try again";
        public const string ErrorUnexpectedWhileSubmittingGrade = "Error occured trying to submit grade, please try again";
        public const string ErrorTestAlreadyStarted = "Cannot edit test questions, test already started";
        public const string ErrorInvalidSymbol= "Symbols #,_ and | are invalid for answers";


    }
}
