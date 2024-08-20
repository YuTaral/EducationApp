using EducationApp.Data.Models;
using EducationApp.Data.Services.Lessons.Models;
using EducationApp.Data.Services.Subjects.Models;
using eUni.Data.Models;
using Humanizer.Localisation.TimeToClockNotation;

namespace EducationApp.Data.Services.Lessons
{
    public interface ILessonService
    {
        public CourseDetailsServiceModel getCourseDetails(int id, string myId);
        public int Create(string title, string description, int courseId, string lessonType, DateTime fromDateTime, DateTime toDateTime);
        public bool Edit(int id, string title, string description, int courseId, string lessonType, DateTime fromDateTime, DateTime toDateTime, string editorId);
        public LessonDetailsServiceModel Details(int id, string myId, bool asTeacher);
        public string UploadFiles(IEnumerable<IFormFile> files, string myId, int lessonId, bool isHomework);
        public int RemoveFile(int fileId);
        public UploadedFile GetFile(int fileId);
        public int Delete(int id);
        public List<LessonType> GetLessonTypes();
        public bool IsOwnerOfLesson(int id, string userId);

    }
}
