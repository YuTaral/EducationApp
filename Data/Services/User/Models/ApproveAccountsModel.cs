using CustomUser = EducationApp.Data.Models.User;

namespace EducationApp.Data.Services.User.Models
{
    public class ApproveAccountsModel
    {
        public List<UserModel>? Users { get; set; }

        public string? approveUserIds { get; set; }

    }
}
