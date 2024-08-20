using CarRentingSystem.Infrastructure.Extensions;
using EducationApp.Data.Models;
using EducationApp.Data.Services.Lessons;
using EducationApp.Data.Services.Lessons.Models;
using eUni.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationApp.Controllers
{
    public class LessonsController : BaseController
    {
        private readonly ILessonService lessonsService;

        public LessonsController(ILessonService service)
        {
            lessonsService = service;
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Add(int id)
        {
            try
            {
                var courseDetails = lessonsService.getCourseDetails(id, GetUserId());
                var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var model = new LessonFormModel
                {
                    LessonType = "",
                    Title = "",
                    CourseDetails = courseDetails,
                    LessonTypes = lessonsService.GetLessonTypes(),
                    FromDateTime = today,
                    ToDateTime = today,

                };

                if (courseDetails != null)
                {
                    return View(model);
                }

                return ReturnCustomError(DataConstants.ErrorAddLessonCourseNotFound, DataConstants.ResourceNotFound);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }

           
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Add(LessonFormModel lesson, int id)
        {
            try
            {
                if (lesson.FromDateTime >= lesson.ToDateTime)
                {
                    ModelState.AddModelError("ToDateTime", "Lesson ending time cannot be before starting time");
                }

                if (!ModelState.IsValid)
                {
                    var courseDetails = lessonsService.getCourseDetails(id, GetUserId());

                    if (courseDetails != null)
                    {
                        lesson.CourseDetails = courseDetails;
                        lesson.LessonTypes = lessonsService.GetLessonTypes();

                        return View(lesson);
                    }
                }

                int result = lessonsService.Create(lesson.Title, lesson.Description, lesson.CourseId, lesson.LessonType, lesson.FromDateTime, lesson.ToDateTime);

                if (result != 0)
                {
                    return RedirectToAction("Details", "Courses", new { id = lesson.CourseId });
                }

                return View();
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize]
        public IActionResult Details(int id)
        {
            try
            {
                var uploadResult = TempData["UploadResult"] as string;
                SetUploadResult(uploadResult);

                var result = lessonsService.Details(id, GetUserId(), IsTeacher());

                return View(result);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize]
        [HttpPost]
        public IActionResult Details(int id, IEnumerable<IFormFile> files)
        {
            try
            {
                var resultString = "";

                if (IsTeacher())
                {
                    resultString = lessonsService.UploadFiles(files, GetUserId(), id, false);

                }
                else if (IsStudent())
                {
                    resultString = lessonsService.UploadFiles(files, GetUserId(), id, true);
                }

                TempData["UploadResult"] = resultString;

                return RedirectToAction("Details", "Lessons", new { id });
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpGet]
        public IActionResult DownloadMaterials(int fileId)
        {
            try
            {
                if (!IsTeacher() && !IsStudent())
                {
                    return ReturnCustomError(DataConstants.ErrorNotAuthorizedToDownload, DataConstants.Unauthorized);
                }

                var file = lessonsService.GetFile(fileId);

                if (file != null)
                {

                    if (System.IO.File.Exists(file.DownloadPath))
                    {
                        return PhysicalFile(file.DownloadPath, "application/octet-stream", file.Name);
                    }
                }

                return ReturnCustomError(DataConstants.ErroDownloadFileNotFound, DataConstants.ResourceNotFound);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpGet]
        public IActionResult RemoveMaterials(int fileId)
        {
            try
            {
                if (!IsTeacher() && !IsStudent())
                {
                    return ReturnCustomError(DataConstants.ErrorNotAuthorizedToRemoveFile, DataConstants.Unauthorized);
                }

                var lessonId = lessonsService.RemoveFile(fileId);

                if (lessonId != -1)
                {
                    return RedirectToAction("Details", "Lessons", new { id = lessonId });
                }

                return ReturnCustomError(DataConstants.ErroDeleteFileNotFound, DataConstants.ResourceNotFound);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize]
        public IActionResult Edit(int id)
        {
            try
            {
                if (!IsTeacher() && !IsStudent())
                {
                    return ReturnCustomError(DataConstants.ErrorNotOwner, DataConstants.Unauthorized);
                }

                var result = lessonsService.Details(id, GetUserId(), IsTeacher());

                if (!CanEditLesson(id, result))
                {
                    return ReturnCustomError(DataConstants.ErrorNotOwner, DataConstants.Unauthorized);
                }

                return View(new LessonFormModel
                {
                    Title = result.Title,
                    Description = result.Description,
                    CourseId = result.CourseId,
                    LessonType = result.LessonType,
                    FromDateTime = result.FromDateTime,
                    ToDateTime = result.ToDateTime,
                    CourseDetails = lessonsService.getCourseDetails(result.CourseId, GetUserId()),
                    LessonTypes = lessonsService.GetLessonTypes()
                });
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, LessonFormModel lesson)
        {
            try
            {
                if (!IsTeacher() && !IsStudent())
                {
                    return ReturnCustomError(DataConstants.ErrorUnauthorized, DataConstants.Unauthorized);
                }

                if (!CanEditLesson(id))
                {
                    return ReturnCustomError(DataConstants.ErrorNotOwner, DataConstants.Unauthorized);
                }

                if (lesson.FromDateTime >= lesson.ToDateTime)
                {
                    ModelState.AddModelError("ToDateTime", "Lesson ending time cannot be before starting time");
                }

                if (!ModelState.IsValid)
                {
                    lesson.CourseDetails = lessonsService.getCourseDetails(lesson.CourseId, GetUserId()); ;
                    lesson.LessonTypes = lessonsService.GetLessonTypes();

                    return View(lesson);
                }

                bool result = lessonsService.Edit(id, lesson.Title, lesson.Description, lesson.CourseId, lesson.LessonType, lesson.FromDateTime, lesson.ToDateTime, User.getId());

                if (!result)
                {
                    return ReturnCustomError(DataConstants.ErrorUnexpectedEditLesson, DataConstants.UnexpectedError);
                }

                return RedirectToAction("Details", "Lessons", new { id });
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        public IActionResult Delete(int Id)
        {
            try
            {
                if (!lessonsService.IsOwnerOfLesson(Id, GetUserId()))
                {
                    return ReturnCustomError(DataConstants.ErrorNotOwner, DataConstants.Unauthorized);
                }

                var courseId = lessonsService.Delete(Id);

                if (courseId != -1)
                {
                    var courseDetails = lessonsService.getCourseDetails(courseId, GetUserId());
                    return RedirectToAction("Details", "Courses", new { id = courseDetails.Id });
                }

                return ReturnCustomError(DataConstants.ErrorLessonDoesNotExists, DataConstants.ResourceNotFound);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        private void SetUploadResult(string resultString)
        {
            if (resultString != null)
            {
                ViewData["Result"] = resultString;
            }
        }


        private bool CanEditLesson(int lessonId, LessonDetailsServiceModel? lessonDetails = null)
        {
            var userId = GetUserId();

            if (lessonDetails == null) {
                lessonDetails = lessonsService.Details(lessonId, userId, IsTeacher());
            }

            var courseDetails = lessonsService.getCourseDetails(lessonDetails.CourseId, userId);

            return courseDetails.TeacherId == userId;
        }
    }
}

 