namespace EducationApp.Data.Services.User.Models
{
    public class UserModel
    {
        public required string UserName { get; set; }    
        public required string Id { get; set; }
        public int? FacultyNumber { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public bool IsApproved { get; set; }

        public string? RoleName { get; set; }
    }
}
