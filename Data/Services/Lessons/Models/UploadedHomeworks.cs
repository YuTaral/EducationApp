using EducationApp.Data.Models;
using CustomUser = EducationApp.Data.Models.User;


namespace EducationApp.Data.Services.Lessons.Models
{
    public class UploadedHomeworks : UploadedFile
    {
        public required CustomUser Uploader { get; set; }
    }
}
