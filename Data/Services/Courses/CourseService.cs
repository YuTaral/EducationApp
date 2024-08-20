using EducationApp.Data.Models;
using EducationApp.Data.Services.Subjects.Models;
using eUni.Data;
using eUni.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using NuGet.Versioning;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using FileSystem = System.IO.File;
using CustomUser = EducationApp.Data.Models.User;
using EducationApp.Data.Services.Courses.Models;

namespace EducationApp.Data.Services.Subjects
{
    public class CourseService : ICourseService
    {

        private readonly EduAppDbContext DBAccess;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<CustomUser> _UserManager;


        public CourseService(EduAppDbContext dbContext, IWebHostEnvironment webHostEnvironment, UserManager<CustomUser> CustomUserManager)
        {
            DBAccess = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _UserManager = CustomUserManager;

        }


        public int Create(string title, string description, string imageurl, string teacherId)
        {

            var courseData = new Course
            {
                Title = title,
                Description = description,
                ImageUrl = imageurl,
                TeacherId = teacherId
            };

            DBAccess.Courses.Add(courseData);
            DBAccess.SaveChanges();

            return courseData.Id;
        }


        public bool Edit(int id, string title, string description, string imageurl, string teacherId)
        {
            var courseData = DBAccess.Courses.Find(id);

            if (courseData == null || !(courseData.TeacherId == teacherId)) {
                return false;
            }

            courseData.Title = title;
            courseData.Description = description;
            courseData.ImageUrl = imageurl;

            DBAccess.SaveChanges();

            return true;
        }


        public CourseDetailsServiceModel Details(int id, string myId, bool asTeacher)
        {
            if (asTeacher) {
                var course = DBAccess.Courses
                        .Where(s => s.Id == id)
                        .Select(s => new CourseDetailsServiceModel
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Description = s.Description,
                            ImageUrl = s.ImageUrl,
                            TeacherId = s.TeacherId,
                            CanEdit = s.TeacherId == myId,
                            CanViewMore = s.TeacherId == myId,
                            Lessons = DBAccess.Lessons
                                .Where(l => l.CourseId == s.Id)
                                .Select(l => new Lesson
                                {
                                    Id = l.Id,
                                    Title = l.Title,
                                    Description = l.Description,
                                    FromDateTime = l.FromDateTime,
                                    ToDateTime = l.ToDateTime,
                                    CourseId = l.CourseId,
                                    LessonType = l.LessonType,

                                }).ToList()

                        }).FirstOrDefault();

                return course;

            } else {
                var hasAccess = DBAccess.Access
                                .Where(a => a.CourseId == id && a.UserId == myId)
                                .Any();

                var course = DBAccess.Courses
                       .Where(s => s.Id == id)
                       .Select(s => new CourseDetailsServiceModel
                       {
                           Id = s.Id,
                           Title = s.Title,
                           Description = s.Description,
                           ImageUrl = s.ImageUrl,
                           TeacherId = s.TeacherId,
                           CanEdit = false,
                           CanViewMore = hasAccess,
                           Lessons = DBAccess.Lessons
                               .Where(l => l.CourseId == s.Id)
                               .Select(l => new Lesson
                               {
                                   Id = l.Id,
                                   Title = l.Title,
                                   Description = l.Description,
                                   FromDateTime = l.FromDateTime,
                                   ToDateTime = l.ToDateTime,
                                   CourseId = l.CourseId,
                                   LessonType = l.LessonType,

                               }).ToList()

                       }).FirstOrDefault();

                return course;
            }
        }


        public CoursesQueryModel FetchMine(CoursesQueryModel query, string myId, bool asTeacher)
        {

            if (asTeacher) {
                return FetchMineAsTeacher(query, myId);
            }
            else {
                return FetchMineAsStudent(query, myId);
            }
        }


        private CoursesQueryModel FetchMineAsTeacher(CoursesQueryModel query, string myId)
        {

            var coursesQuery = applyFilters(query);

            var courses = coursesQuery
                .Where(c => c.TeacherId == myId)
                .Skip((query.CurrentPage - 1) * query.CoursesPerPage)
                .Take(query.CoursesPerPage)
                .Select(c => new CourseListingModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    ImageUrl = c.ImageUrl
                })
                .ToList();

            return new CoursesQueryModel
            {
                SearchTerm = query.SearchTerm,
                Sorting = query.Sorting,
                CurrentPage = query.CurrentPage,
                Courses = courses,
                TotalCourses = coursesQuery.Count()
            };
        }

        private CoursesQueryModel FetchMineAsStudent(CoursesQueryModel query, string myId)
        {
            var coursesQuery = applyFilters(query);
            var accessedCourseIds = DBAccess.Access
                                    .Where(a => a.UserId == myId)
                                    .Select(a => a.CourseId)
                                    .ToList();

            
            var courses = coursesQuery
                .Where(c => accessedCourseIds.Contains(c.Id))
                .Skip((query.CurrentPage - 1) * query.CoursesPerPage)
                .Take(query.CoursesPerPage)
                .Select(c => new CourseListingModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    ImageUrl = c.ImageUrl
                })
                .ToList();

            return new CoursesQueryModel
            {
                SearchTerm = query.SearchTerm,
                Sorting = query.Sorting,
                CurrentPage = query.CurrentPage,
                Courses = courses,
                TotalCourses = coursesQuery.Count()
            };
        }


        public CoursesQueryModel FetchCourses(CoursesQueryModel query)
        {

            var coursesQuery = applyFilters(query);

            var courses = coursesQuery
                .Skip((query.CurrentPage - 1) * query.CoursesPerPage)
                .Take(query.CoursesPerPage)
                .Select(s => new CourseListingModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    ImageUrl = s.ImageUrl   
                })
                .ToList();

            return new CoursesQueryModel
            {
                SearchTerm = query.SearchTerm,
                Sorting = query.Sorting,
                CurrentPage = query.CurrentPage,
                Courses = courses,
                TotalCourses = coursesQuery.Count()
            };
        }


        private IQueryable<Course> applyFilters(CoursesQueryModel query)
        {

            var coursesQuery = DBAccess.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                coursesQuery = coursesQuery.Where(c => c.Title.ToLower().Contains(query.SearchTerm.ToLower())
                                                      || c.Description.ToLower().Contains(query.SearchTerm.ToLower()));
            }

            if (query.Sorting == "Asc")
            {
                coursesQuery = coursesQuery.OrderBy(s => s.Id);
            }
            else
            {
                coursesQuery = coursesQuery.OrderByDescending(s => s.Id);
            }

            return coursesQuery;
        }


        public bool Delete(int id)
        {
            var course = DBAccess.Courses
                .FirstOrDefault(c => c.Id == id);

            if (course != null)
            {
                var lessons = DBAccess.Lessons
                    .Where(l => l.CourseId == course.Id)
                    .ToList();

                foreach (var lesson in lessons)
                {
                    // Delete all file records
                    var files = DBAccess.UploadedFiles
                        .Where(f => f.LessonId == lesson.Id)
                        .ToList();

                    foreach (var file in files)
                    {
                        DBAccess.UploadedFiles.Remove(file);
                    }

                    // Delete the lesson's folder
                    var wwwrootPath = _webHostEnvironment.WebRootPath;
                    var folderPath = Path.Combine(wwwrootPath, "uploaded", lesson.Id.ToString());
                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, recursive: true);
                    }


                    // Delete the lesson
                    DBAccess.Lessons.Remove(lesson);

                    // Delete the test, questions and answers related to this lesson
                    if (lesson.LessonType == DataConstants.LTypeTest) {
                        var test = DBAccess.Tests.Where(t => t.LessonId == lesson.Id).FirstOrDefault();

                        if (test != null) {
                            DBAccess.Tests.Remove(test);

                            var questions = DBAccess.Questions.Where(q => q.TestId == test.Id).ToList();

                            if (questions != null) {
                                foreach (var question in questions)
                                {
                                    DBAccess.Questions.Remove(question);

                                    if (question.Type ==  DataConstants.QTypeMulti || question.Type == DataConstants.QTypeSingle) {
                                        var selectableQuestionAnswers = DBAccess.Answers.Where(a => a.QuestionId == question.Id).ToList();

                                        if (selectableQuestionAnswers != null)
                                        {
                                            foreach (var a in selectableQuestionAnswers) {
                                                DBAccess.Answers.Remove(a);
                                            }
                                        }

                                        var questionAnswers = DBAccess.QuestionAnswers.Where(a => a.QuestionId == question.Id).ToList();

                                        if (questionAnswers != null)
                                        {
                                            foreach (var a in questionAnswers)
                                            {
                                                DBAccess.QuestionAnswers.Remove(a);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Delete the course
                DBAccess.Courses.Remove(course);
                DBAccess.SaveChanges();
                return true;
            }

            return false;
        }


        public GrantAccessFormModel FetchAccessForCourse(CourseDetailsServiceModel courseDetails) {

            var studentRecords = _UserManager.GetUsersInRoleAsync(DataConstants.StudentRoleName).Result.Where(s => s.IsApproved);
            var studentIds = studentRecords.Select(u => u.Id);
            var test = courseDetails.Id;
            var accessRecords = DBAccess.Access
                                .Where(a => a.CourseId == courseDetails.Id)
                                .Select(a => new Access { 
                                    CourseId = a.CourseId,
                                    UserId = a.UserId
                                }).ToList();

            IEnumerable<CustomUser> CustomUsersWithAccess = new List<CustomUser>();
            IEnumerable<CustomUser> CustomUsersWithNoAccess = new List<CustomUser>();

            foreach (var id in studentIds) {

                var hasAccess = accessRecords.Find(a => a.UserId == id);
                var CustomUser = studentRecords.Where(u => u.Id == id).FirstOrDefault();

                if (hasAccess != null) {
                    CustomUsersWithAccess = CustomUsersWithAccess.Concat(new List<CustomUser> { CustomUser });

                } else {
                    CustomUsersWithNoAccess = CustomUsersWithNoAccess.Concat(new List<CustomUser> { CustomUser });
                }

            }

            return new GrantAccessFormModel
            {
                Course = courseDetails,
                UsersWithAccess = CustomUsersWithAccess,
                UsersWithNoAccess = CustomUsersWithNoAccess
            };
        }


        public bool GrantAccess(string[] grantAccessToIds, int courseId)
        {

            try
            {
                for (int i = 0; i < grantAccessToIds.Length; i++)
                {
                    var accessRecord = DBAccess.Access
                                       .Where(a => a.CourseId == courseId && a.UserId == grantAccessToIds[i])
                                       .FirstOrDefault();

                    if (accessRecord == null) {
                        var grantAccess = new Access
                        {
                            CourseId = courseId,
                            UserId = grantAccessToIds[i]
                        };

                        DBAccess.Access.Add(grantAccess);
                    } 
                }

                DBAccess.SaveChanges();
                return true;
            }

            catch {
                return false;
            }
        }

        public bool UnGrantAccess(string[] unGrantAccessToIds, int courseId)
        {
            try
            {
                for (int i = 0; i < unGrantAccessToIds.Length; i++)
                {
                    var accessRecord = DBAccess.Access
                                       .Where(a => a.CourseId == courseId && a.UserId == unGrantAccessToIds[i])
                                       .FirstOrDefault();

                    if (accessRecord != null)
                    {
                        DBAccess.Access.Remove(accessRecord);
                    }
                }

                DBAccess.SaveChanges();
                return true;
            }

            catch {
                return false;
            }
        }

        public bool IsOwnerOfCourse(int id, string CustomUserId) {

            var course = DBAccess.Courses.Where(c => c.Id == id).FirstOrDefault();

            if (course != null) {
                return course.TeacherId == CustomUserId;
            }

            return false;

        }
    }
}
