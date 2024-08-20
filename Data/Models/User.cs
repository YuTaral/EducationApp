using eUni.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Models
{
    public class User : IdentityUser
    {
        [StringLength(DataConstants.FacNumberLen)]
        public int? FacultyNumber { get; set; }

        [MinLength(DataConstants.FirstNameMinLen)]
        [MaxLength(DataConstants.FirstNameMaxLen)]
        public string? FirstName { get; set; }

        [MinLength(DataConstants.LastNameMinLen)]
        [MaxLength(DataConstants.LastNameMaxLen)]
        public string? LastName { get; set;}

        public bool IsApproved { get; set; } 
    }
}
