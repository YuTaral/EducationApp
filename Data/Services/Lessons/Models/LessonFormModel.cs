using EducationApp.Data.Models;
using EducationApp.Data.Services.Subjects.Models;
using eUni.Data;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EducationApp.Data.Services.Lessons.Models
{
    public class LessonFormModel
    {

        [MinLength(DataConstants.LessonTitleMinLen)]
        [MaxLength(DataConstants.LessonTitleMaxLen)]
        public required string Title { get; set; }

        [AllowNull]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please specify Lesson type.")]
        public required string LessonType { get; set; }

        public List<LessonType> LessonTypes;

        public int CourseId { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        public CourseDetailsServiceModel? CourseDetails { get; set; }
    }
}
