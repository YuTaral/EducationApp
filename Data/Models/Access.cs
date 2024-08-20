using eUni.Data;
using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Models
{
    public class Access
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public required int CourseId { get; set; }
    }
}
