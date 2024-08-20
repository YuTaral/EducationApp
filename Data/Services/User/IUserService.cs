namespace EducationApp.Data.Services.User;

using EducationApp.Data.Services.User.Models;
using CustomUser = EducationApp.Data.Models.User;


public interface IUserService
{
    public int IsAccountApproved(string username);
    public Task<bool> IsUserAdmin(string userEmail);
    public List<UserModel> GetNotApprovedAccounts();
    public Task<bool> ApproveAccounts(string[] ids);

    
}
