using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationApp.Data.Models.Questions
{
    public class Question

    {
        public int Id { get; set; }

        [Required]
        public required string Text { get; set; }

        [Required]
        [ForeignKey("TestId")]
        public int TestId { get; set; }

        [Required]
        public required string Type { get; set; }

        [Required]
        public int Sequence { get; set; }

        [Required]
        [Range(0.0, 100.0, ErrorMessage = "The points must be in range 0 - 100")]
        public double Points { get; set; }

    }
}
