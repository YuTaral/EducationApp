using EducationApp.Data.Models;
using EducationApp.Data.Services.User;
using EducationApp.Data.Services.User.Models;
using eUni.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EducationApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService userService;

        public UsersController(IUserService service)
        {
            userService = service;
        }


        [Authorize(Roles = DataConstants.AdministratorRoleName)]
        public IActionResult ManageAccounts()
        {
            try {
                var users = userService.GetNotApprovedAccounts();
                var model = new ApproveAccountsModel { Users = users };

                return View(model);
            }
            catch (Exception) {

                return ReturnRequestError();
            }
        }


        [HttpPost]
        [Authorize(Roles = DataConstants.AdministratorRoleName)]
        public async Task<IActionResult> ManageAccounts(ApproveAccountsModel users)
        {
            try {
                if (users.approveUserIds != null) {
                    string[] ids = users.approveUserIds.Split(",");
                    await userService.ApproveAccounts(ids);
                }

                return RedirectToAction(nameof(ManageAccounts));
            }
            catch (Exception) {
                return ReturnRequestError();
            }
        }
    }
}
