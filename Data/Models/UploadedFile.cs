using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Models
{
    public class UploadedFile
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
        
        [Required]
        public required string OwnerId { get; set; }

        [Required]
        public required int LessonId { get; set; }

        [Required]
        public required bool IsHomework { get; set; }

        [Required]
        public required DateTime Date { get; set; }

        public string? DownloadPath { get; set; }

    }
}
