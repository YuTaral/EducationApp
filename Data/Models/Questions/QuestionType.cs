using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Models.Questions
{
    public class QuestionType
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }
    }
}
