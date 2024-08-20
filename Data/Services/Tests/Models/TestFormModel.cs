using EducationApp.Data.Models.Questions;
using System.Collections;

namespace EducationApp.Data.Services.Tests.Models
{
    public class TestFormModel
    {
        public int Id { get; set; }

        public IEnumerable<Question>? Questions { get; set; }

        public int LessonId { get; set; }
    }
}
