using EducationApp.Data.Models;
using eUni.Data;
using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Services.Lessons.Models
{
    public class LessonDetailsServiceModel
    {
        public int Id { get; set; }

        public required string Title { get; set; }
        
        public string? Description { get; set; }

        public required int CourseId { get; set; }

        public required String LessonType { get; set; }

        public bool CanEdit { get; set; }

        public IEnumerable<IFormFile> Files { get; set; }

        public List<UploadedFile>? UploadedFiles { get; set; }

        public List<UploadedHomeworks>? UploadedHomeworks{ get; set; }

        public DateTime FromDateTime { get; set; }

        public DateTime ToDateTime { get; set; }

        public int TestId { get; set; }

        public string? UploadResult { get; set; }

        public Boolean IsTestSubmitted { get; set; }
        public double Percent { get; set; }
        public Boolean IsGradeSubmitted { get; set; }

    }
}
