using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationApp.Data.Models.Questions
{
    public class QuestionAnswer
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [ForeignKey("QuestionId")]
        public int QuestionId { get; set; }
        public string? Answer { get; set; }
        [Required]
        public required string StudentId { get; set; }
        public double? Points { get; set; }

    }
}
