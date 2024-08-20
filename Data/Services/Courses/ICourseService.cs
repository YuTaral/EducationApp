using EducationApp.Data.Services.Courses.Models;
using EducationApp.Data.Services.Subjects.Models;

namespace EducationApp.Data.Services.Subjects
{
    public interface ICourseService
    {
        public int Create(string title, string description, string imageurl, string teacherId);
        public bool Edit(int id, string title, string description, string imageurl, string teacherId);
        public CourseDetailsServiceModel Details(int id, string myId, bool asTeacher);
        public CoursesQueryModel FetchMine(CoursesQueryModel query, string myId, bool asTeacher);
        public CoursesQueryModel FetchCourses(CoursesQueryModel query);
        public GrantAccessFormModel FetchAccessForCourse(CourseDetailsServiceModel courseDetails);
        public bool GrantAccess(string[] grantAccessToIds, int courseId);
        public bool UnGrantAccess(string[] unGrantAccessToIds, int courseId);
        public bool Delete(int id);

        public bool IsOwnerOfCourse(int id, string userId);
    }
}
