using eUni.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Services.Subjects.Models
{
    public class CourseDetailsServiceModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }

        [Required(ErrorMessage = "The Image field is required")]
        public required string ImageUrl { get; set; }
        public required string TeacherId { get; set; }
        public bool CanEdit { get; set; }
        public bool CanViewMore { get; set; }
        public List<Lesson>? Lessons { get; set; }
    }
}
