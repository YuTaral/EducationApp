using eUni.Data;
using System.ComponentModel.DataAnnotations;

namespace EducationApp.Data.Services.Subjects.Models
{
    public class CourseServiceModel
    {
       
        public required string Title { get; init; }
        public required string Description { get; init; }      
        public required string ImageUrl { get; init; }
        public required string TeacherId { get; init; }
        
    }
}
