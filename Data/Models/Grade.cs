using Humanizer.Localisation.TimeToClockNotation;

namespace EducationApp.Data.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public required int TestId { get; set; }
        public required string StudentId { get; set; }
        public double Result { get; set; }
        public double? TotalPoints { get; set; }
        public Boolean IsSubmited { get; set; }
    }
}
