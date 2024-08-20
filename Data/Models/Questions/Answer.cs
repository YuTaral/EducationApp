using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationApp.Data.Models.Questions
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        public required bool IsCorrect { get; set; }

        [Required]
        [ForeignKey("QuestionId")]
        public required int QuestionId { get; set; }

        [Required]
        public required string AnswerText { get; set; }

    }
}
