using EducationApp.Data.Models;
using EducationApp.Data.Services.User.Models;
using eUni.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using CustomUser = EducationApp.Data.Models.User;

namespace EducationApp.Data.Services.User
{
    public class UserService : IUserService
    {
        private readonly EduAppDbContext DBAccess;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<CustomUser> _userManager;
        List<CustomUser> studentsToApprove;
        List<CustomUser> teachersToApprove;



        public UserService(EduAppDbContext dbContext, IWebHostEnvironment webHostEnvironment, UserManager<CustomUser> userManager)
        {
            DBAccess = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            studentsToApprove = new List<CustomUser>();
            teachersToApprove = new List<CustomUser>();
        }

        public int IsAccountApproved(string username)
        {
            var user = DBAccess.Users.Where(u => u.UserName == username).FirstOrDefault();

            if (user == null) {
                return -1;
            }

            return user.IsApproved ? 1 : 0;
        }

        public async Task<bool> IsUserAdmin(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, DataConstants.AdministratorRoleName);
        }

        public List<UserModel> GetNotApprovedAccounts() {

            var users =  DBAccess.Users.Where(u => !u.IsApproved).ToList();
            List<UserModel> userModels = new List<UserModel>();

            if (users.Count > 0) {

                foreach (var user in users)
                {
                    var userModel = new UserModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FacultyNumber = user.FacultyNumber,
                        IsApproved = user.IsApproved,
                        RoleName = SetUserRole(user.UserName)
                    };

                    userModels.Add(userModel);
                }
            }

            return userModels;
        }

        private string SetUserRole(string username)
        {
            var role = DBAccess.Users
                .Where(u => u.UserName == username)
                .Join(
                    DBAccess.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole }
                )
                .Join(
                    DBAccess.Roles,
                    userUserRole => userUserRole.userRole.RoleId,
                    role => role.Id,
                    (userUserRole, role) => role.Name
                )
                .FirstOrDefault();

            return role ?? "";
        }

        public async Task<bool> ApproveAccounts(string[] ids) {
            await filterUsers(ids);

            if (studentsToApprove.Count > 0)
            {
                var approvedStudents = DBAccess.Users.Where(u => u.FacultyNumber.HasValue && u.IsApproved).ToList();
                var biggestFacNumber = 1;

                if (approvedStudents.Count > 0)
                {
                    biggestFacNumber = approvedStudents.Max(u => u.FacultyNumber.Value);
                }

                for (int i = 0; i < studentsToApprove.Count; i++)
                {
                    var user = DBAccess.Users.Find(studentsToApprove[i].Id);

                    if (user != null)
                    {
                        user.IsApproved = true;
                        biggestFacNumber++;
                        user.FacultyNumber = biggestFacNumber;
                        DBAccess.SaveChanges();
                    }
                }
            }

            
            for (int i = 0; i < teachersToApprove.Count; i++)
            {
                var user = DBAccess.Users.Find(teachersToApprove[i].Id);

                user.IsApproved = true;
                DBAccess.SaveChanges();
            }

            studentsToApprove.Clear();
            teachersToApprove.Clear();

            return true;
        }


        private async Task<bool> filterUsers(string[] ids) {

            for (int i = 0; i < ids.Length; i++)
            {
                var user = DBAccess.Users.Where(u => u.Id == ids[i]).FirstOrDefault();

                if (user != null)
                {
                    var isStudent = await _userManager.IsInRoleAsync(user, DataConstants.StudentRoleName);

                    if (isStudent) {
                        studentsToApprove.Add(user);
                    } else {
                        teachersToApprove.Add(user);
                    }

                }
            }

            return true;
        }
    }
}
