using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Models
{
    public class LessonType
    {
        public int Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
    }
}
