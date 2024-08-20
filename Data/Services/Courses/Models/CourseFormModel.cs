using eUni.Data;
using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Services.Courses.Models
{
    public class CourseFormModel
    {

        [Required]
        [StringLength(DataConstants.CourseTitleMaxLen,
                      MinimumLength = DataConstants.CourseTitleMinLen,
                      ErrorMessage = "Title must be between {2} and {1} letters long.")]
        public required string Title { get; init; }

        [Required]
        [StringLength(DataConstants.CourseDescMaxLen,
                      MinimumLength = DataConstants.CourseDescMinLen,
                      ErrorMessage = "Description must be between {2} and {1} symbols long.")]
        public required string Description { get; init; }

        [Required(ErrorMessage = "The Image field is required")]
        [Url]
        public required string ImageUrl { get; init; }

    }
}
