using CarRentingSystem.Infrastructure.Extensions;
using EducationApp.Data.Models;
using EducationApp.Data.Services.Courses.Models;
using EducationApp.Data.Services.Lessons;
using EducationApp.Data.Services.Subjects;
using eUni.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EducationApp.Controllers
{
    public class CoursesController : BaseController
    {
        private readonly ICourseService coursesService;
        private readonly ILessonService lessonsService;


        public CoursesController(ICourseService service, ILessonService lessonServ) 
        {
            coursesService = service;
            lessonsService = lessonServ;
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Add() 
        {
            try
            {
                return View();

            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Add(CourseFormModel course)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(course);
                }

                int result = coursesService.Create(course.Title, course.Description, course.ImageUrl, User.getId());

                if (result != 0)
                {
                    return RedirectToAction(nameof(MyCourses));
                }

                return ReturnCustomError(DataConstants.ErrorCreateQuestion, DataConstants.UnexpectedError);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Edit(int id)
        {
            try
            {
                var result = coursesService.Details(id, User.getId(), IsTeacher());

                if (!canEditCourse(id))
                {
                    return ReturnCustomError(DataConstants.ErrorCannotEditCourse, DataConstants.Unauthorized);
                }

                return View(new CourseFormModel
                {
                    Title = result.Title,
                    Description = result.Description,
                    ImageUrl = result.ImageUrl
                });
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Edit(int id, CourseFormModel course)
        {
            try
            {
                if (!canEditCourse(id))
                {
                    return ReturnCustomError(DataConstants.ErrorCannotEditCourse, DataConstants.Unauthorized);
                }

                if (!ModelState.IsValid)
                {
                    return View(course);
                }

                bool success = coursesService.Edit(id, course.Title, course.Description, course.ImageUrl, User.getId());

                if (success)
                {
                    return RedirectToAction(nameof(MyCourses));
                }

                return ReturnCustomError(DataConstants.ErrorUnexpectedCannotEditCourse, DataConstants.UnexpectedError);
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
                var result = coursesService.Details(id, User.getId(), IsTeacher());
                return View(result);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        public IActionResult MyCourses([FromQuery] CoursesQueryModel query) {
            try
            {
                if (!IsTeacher() && !IsStudent())
                {
                    return ReturnCustomError(DataConstants.ErrorUnauthorized, DataConstants.Unauthorized);
                }

                var result = coursesService.FetchMine(query, User.getId(), IsTeacher());
                return View(result);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        public IActionResult AllCourses([FromQuery] CoursesQueryModel query)
        {
            try
            {
                var result = coursesService.FetchCourses(query);
                return View(result);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult Delete(int Id)
        {
            try
            {
                if (!coursesService.IsOwnerOfCourse(Id, GetUserId()))
                {
                    return ReturnCustomError(DataConstants.ErrorNotOwner, DataConstants.Unauthorized);
                }

                var success = coursesService.Delete(Id);

                if (success)
                {
                    return RedirectToAction(nameof(MyCourses));
                }

                return ReturnCustomError(DataConstants.ErrorCannotDeleteCourse, DataConstants.UnexpectedError);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
            
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult ManageAccess([FromQuery] CoursesQueryModel query)
        {
            try
            {
                if (IsTeacher())
                {
                    var result = coursesService.FetchMine(query, GetUserId(), true);
                    return View(result);
                }

                return ReturnCustomError(DataConstants.ErrorUnauthorized, DataConstants.Unauthorized);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult ManageAccessForCourse(int id)
        {
            try
            {
                var courseDeitals = coursesService.Details(id, User.getId(), IsTeacher());

                if (courseDeitals.TeacherId != User.getId())
                {
                    return ReturnCustomError(DataConstants.ErrorNotOwner, DataConstants.Unauthorized);
                }

                var result = coursesService.FetchAccessForCourse(courseDeitals);

                return View(result);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.TeacherRoleName)]
        public IActionResult ManageAccessForCourse(int id, GrantAccessFormModel studentsAccess)
        {
            try
            {
                var success = false;

                if (studentsAccess.UnGrantedUserIds != null && studentsAccess.UnGrantedUserIds.Length > 0)
                {
                    var UnGrantAccessToIds = studentsAccess.UnGrantedUserIds.Remove(studentsAccess.UnGrantedUserIds.Length - 1).Split(",");
                    success = coursesService.UnGrantAccess(UnGrantAccessToIds, id);
                }

                if (studentsAccess.GrantedUserIds != null && studentsAccess.GrantedUserIds.Length > 0)
                {
                    var grantAccessToIds = studentsAccess.GrantedUserIds.Remove(studentsAccess.GrantedUserIds.Length - 1).Split(",");
                    success = coursesService.GrantAccess(grantAccessToIds, id);
                }

                if (success)
                {
                    return RedirectToAction("ManageAccessForCourse", "Courses", new { id });
                }

                return ReturnCustomError(DataConstants.ErrorUnexpectedGrantAccess, DataConstants.UnexpectedError);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        private bool canEditCourse(int courseId)
        {
            var courseDetails = coursesService.Details(courseId, GetUserId(), IsTeacher());
            return courseDetails.TeacherId == GetUserId();
        }
    }
}
