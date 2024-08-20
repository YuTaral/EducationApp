using EducationApp.Data;
using EducationApp.Data.Services.Courses.Models;
using EducationApp.Data.Services.Subjects;
using EducationApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EducationApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICourseService coursesService;

        public HomeController(EduAppDbContext dbContext, ICourseService service)
        {
            coursesService = service;
        }


        public IActionResult Index([FromQuery] CoursesQueryModel query)
        {
            try
            {
                var course = coursesService.FetchCourses(query);
                return View(course);
            }
            catch (Exception)
            {
                return ReturnRequestError();
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}