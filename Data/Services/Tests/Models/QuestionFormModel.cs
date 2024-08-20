using EducationApp.Data.Models.Questions;
using eUni.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationApp.Data.Services.Tests.Models
{
    public class QuestionFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter question text")]
        public string? Text { get; set; }

        public string? PossibleAnswers { get; set; }

        public string? Type { get; set; }

        public int Sequence { get; set; }

        public List<QuestionType> QuestionTypes;

        public TestFormModel Test { get; set; }

        [Range(1, DataConstants.QuestionMaxAnswers)]
        public int AnswersCount { get; set; }

        public bool EditQuestion { get; set; }
        public double Points { get; set; }

    }
}
