using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace eUni.Data.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        [MinLength(DataConstants.LessonTitleMinLen)]
        [MaxLength(DataConstants.LessonTitleMaxLen)]
        public required string Title { get; set; }

        [AllowNull]
        [MaxLength(DataConstants.LessonDescMaxLen)]
        public string Description { get; set; }

        [Required]
        [ForeignKey("CourseId")]
        public required int CourseId { get; set; }

        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        [Required]
        public required String LessonType { get; set; }
    }
}
