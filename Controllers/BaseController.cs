using CarRentingSystem.Infrastructure.Extensions;
using EducationApp.Data.Models;
using eUni.Data;
using Microsoft.AspNetCore.Mvc;

namespace EducationApp.Controllers
{
    public class BaseController : Controller
    {

        protected IActionResult ReturnCustomError(string message, string title)
        {
            var error = new CustomErrorModel
            {
                ErrorTitle = title,
                ErrorMessage = message
            };

            return View("ReturnCustomError", error);
        }


        protected IActionResult ReturnRequestError() {

            var error = new CustomErrorModel
            {
                ErrorTitle = DataConstants.UnexpectedError,
                ErrorMessage = DataConstants.ErrorWhileProccessingRequest
            };

            return View("ReturnCustomError", error);
        }


        protected string GetUserId()
        {
            return User.getId();
        }

        protected bool IsTeacher()
        {
            if (User.IsInRole(DataConstants.AdministratorRoleName))
            {
                return false;
            }

            return User.IsInRole(DataConstants.TeacherRoleName);
        }


        protected bool IsStudent()
        {
            if (User.IsInRole(DataConstants.AdministratorRoleName))
            {
                return false;
            }

            return User.IsInRole(DataConstants.StudentRoleName);
        }

    }
}
