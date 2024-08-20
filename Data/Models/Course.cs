using System.ComponentModel.DataAnnotations;

namespace eUni.Data.Models
{
    public class Course
    {

        public int Id { get; set; }
        
        [Required]
        [MinLength(DataConstants.CourseTitleMinLen)]
        [MaxLength(DataConstants.CourseTitleMaxLen)]
        public required string Title { get; set; }
        
        [Required]
        [MinLength(DataConstants.CourseDescMinLen)]
        [MaxLength(DataConstants.CourseDescMaxLen)]
        public required string Description { get; set; }
        
        [Required]
        public required string ImageUrl { get; set; }

        public string? TeacherId { get; set; }



    }

}
