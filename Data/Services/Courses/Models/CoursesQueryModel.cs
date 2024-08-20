using eUni.Data;

namespace EducationApp.Data.Services.Courses.Models
{
    public class CoursesQueryModel
    {

        public int CoursesPerPage { get; } = DataConstants.CoursesPerPage;
        public string? SearchTerm { get; init; }
        public string? Sorting { get; init; }
        public int CurrentPage { get; init; } = 1;
        public IEnumerable<CourseListingModel>? Courses { get; init; }
        public int TotalCourses { get; init; }

    }
}
