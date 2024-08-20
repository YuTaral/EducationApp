using EducationApp.Data.Services.Subjects.Models;
using CustomUser = EducationApp.Data.Models.User;


namespace EducationApp.Data.Services.Courses.Models
{
    public class GrantAccessFormModel
    {
        public required CourseDetailsServiceModel Course { get; set; }
        public IEnumerable<CustomUser>? UsersWithAccess { get; init; }
        public IEnumerable<CustomUser>? UsersWithNoAccess { get; init; }
        public IEnumerable<CustomUser>? CurrentlyGrantedAccessUsers { get; init; }
        public string? GrantedUserIds { get; init; }
        public string? UnGrantedUserIds { get; init; }
        public string? ResultMessage { get; init; }
    }
}
