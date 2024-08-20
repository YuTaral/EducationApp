using EducationApp.Data.Models;
using EducationApp.Data.Services.Lessons.Models;
using EducationApp.Data.Services.Subjects.Models;
using eUni.Data.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using FileSystem = System.IO.File;
using CustomUser = EducationApp.Data.Models.User;
using eUni.Data;

namespace EducationApp.Data.Services.Lessons
{
    public class LessonService : ILessonService
    {
        private readonly EduAppDbContext DBAccess;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LessonService(EduAppDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            DBAccess = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public CourseDetailsServiceModel getCourseDetails(int id, string myId)
        {
            var course = DBAccess.Courses
                           .Where(s => s.Id == id)
                           .Select(s => new CourseDetailsServiceModel
                           {
                               Id = s.Id,
                               Title = s.Title,
                               Description = s.Description,
                               ImageUrl = s.ImageUrl,
                               TeacherId = s.TeacherId,
                               CanEdit = s.TeacherId == myId
                           })
                           .FirstOrDefault();
            return course;
        }


        public int Create(string title, string description, int courseId, string lessonType, DateTime fromDateTime, DateTime toDateTime)
        {
            var lessonData = new Lesson
            {
                Title = title,
                Description = description,
                FromDateTime = fromDateTime,
                ToDateTime = toDateTime,
                CourseId = courseId,
                LessonType = lessonType,

            };

            DBAccess.Lessons.Add(lessonData);
            DBAccess.SaveChanges();

            // When lesson is created, create a parent folder for the lesson to upload materials and homeworks
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var lessonParentFolderPath = Path.Combine(wwwRootPath, "uploaded", lessonData.Id.ToString());

            Directory.CreateDirectory(lessonParentFolderPath);
            var materialsFolderPath = Path.Combine(lessonParentFolderPath, "materials");
            var homeworksFolderPath = Path.Combine(lessonParentFolderPath, "homeworks");
            Directory.CreateDirectory(materialsFolderPath);
            Directory.CreateDirectory(homeworksFolderPath);

            return lessonData.Id;
        }


        public LessonDetailsServiceModel Details(int id, string myId, bool asTeacher) {

            var lesson = DBAccess.Lessons
                        .Where(s => s.Id == id)
                        .Select(s => new LessonDetailsServiceModel
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Description = s.Description,
                            CourseId = s.CourseId,
                            LessonType = s.LessonType,
                            FromDateTime = s.FromDateTime,
                            ToDateTime = s.ToDateTime,
                            UploadedFiles = DBAccess.UploadedFiles
                                            .Where(f => f.LessonId == s.Id && f.IsHomework == false)
                                            .Select(f => new UploadedFile {
                                                Id = f.Id,
                                                Name = f.Name,
                                                OwnerId = f.OwnerId,
                                                LessonId = f.LessonId,
                                                IsHomework = f.IsHomework,
                                                DownloadPath = f.DownloadPath,
                                                Date = f.Date,
                                            }).ToList(),
                            TestId = DBAccess.Tests.Where(t => t.LessonId == id).Select(t => t.Id).FirstOrDefault()
                        }).FirstOrDefault();

            if (lesson != null)
            {
                var grade = DBAccess.Grades.Where(g => g.TestId == lesson.TestId && g.StudentId == myId).FirstOrDefault();

                if (grade != null) {
                    lesson.IsTestSubmitted = true;
                    lesson.IsGradeSubmitted = grade.IsSubmited;
                    lesson.Percent = grade.Result;
                }

                var course = DBAccess.Courses
                         .Where(c => c.Id == lesson.CourseId)
                         .Select(c => new Course
                         {
                             Title = c.Title,
                             Description = c.Description,
                             ImageUrl = c.ImageUrl,
                             TeacherId = c.TeacherId,

                         }).FirstOrDefault();

                if (course != null)
                {
                    if (asTeacher)
                    {
                        lesson.CanEdit = course.TeacherId == myId;
                        lesson.UploadedHomeworks = DBAccess.UploadedFiles
                                                   .Where(f => f.LessonId == lesson.Id && f.IsHomework)
                                                   .Select(f => new UploadedHomeworks
                                                   {
                                                       Id = f.Id,
                                                       Name = f.Name,
                                                       OwnerId = f.OwnerId,
                                                       LessonId = f.LessonId,
                                                       IsHomework = f.IsHomework,
                                                       DownloadPath = f.DownloadPath,
                                                       Date = f.Date,
                                                       Uploader = DBAccess.Users
                                                                  .Where(u => u.Id == f.OwnerId)
                                                                  .Select(u => new CustomUser
                                                                  {
                                                                      Id = u.Id,
                                                                      UserName = u.UserName,
                                                                      FirstName = u.FirstName,
                                                                      LastName = u.LastName,
                                                                      FacultyNumber = u.FacultyNumber
                                                                  }).FirstOrDefault()
                                                   }).ToList();
                        var A = 5;
                    }
                    else {
                        lesson.CanEdit = false;

                        lesson.UploadedHomeworks = DBAccess.UploadedFiles
                                                   .Where(f => f.LessonId == lesson.Id && f.IsHomework && f.OwnerId == myId)
                                                   .Select(f => new UploadedHomeworks
                                                   {
                                                       Id = f.Id,
                                                       Name = f.Name,
                                                       OwnerId = f.OwnerId,
                                                       LessonId = f.LessonId,
                                                       IsHomework = f.IsHomework,
                                                       DownloadPath = f.DownloadPath,
                                                       Date = f.Date,
                                                       Uploader = DBAccess.Users
                                                                  .Where(u => u.Id == f.OwnerId)
                                                                  .Select(u => new CustomUser
                                                                  {
                                                                      Id = u.Id,
                                                                      UserName = u.UserName,
                                                                      FirstName = u.FirstName,
                                                                      LastName = u.LastName,
                                                                      FacultyNumber = u.FacultyNumber
                                                                  }).FirstOrDefault()
                                                   }).ToList();
                    }

                    return lesson;
                }
            }

            // return not found
            return lesson;
        }

        public string UploadFiles(IEnumerable<IFormFile> files, string myId, int lessonId, bool isHomework)
        {
            var resultString = "";
            var unsucessfullMsg = " was not uploaded. File with the same name already exists for this lesson.\n";
            var sucessfullMsg = " successfully uploaded.\n";
            var addFile = false;
            var folderName = isHomework ? "homeworks\\" + myId : "materials";

            foreach (var file in files)
            {
                var wwwrootPath = _webHostEnvironment.WebRootPath;
                var studentFolderPath = wwwrootPath + "\\uploaded\\" + lessonId + "\\" + folderName;
                var filePath = wwwrootPath + "\\uploaded\\" + lessonId + "\\" + folderName + "\\" + file.FileName;

                if (!(isHomework && File.Exists(studentFolderPath))) {
                    // Create folder for this student
                    Directory.CreateDirectory(studentFolderPath);
                }

                // Part 1: save the db record
                var fileRecord = new UploadedFile
                {
                    Name = file.FileName,
                    OwnerId = myId,
                    LessonId = lessonId,
                    IsHomework = isHomework,
                    DownloadPath = filePath,
                    Date = DateTime.Now
                };
                DBAccess.UploadedFiles.Add(fileRecord);
                DBAccess.SaveChanges();


                // Part 2: save the file
                if (File.Exists(filePath))
                {
                    var foundedFiles = DBAccess.UploadedFiles
                                            .Where(f => f.LessonId == lessonId && f.Name == file.FileName)
                                            .Select(f => new UploadedFile
                                            {
                                                Name = f.Name,
                                                OwnerId = f.OwnerId,
                                                LessonId = f.LessonId,
                                                IsHomework = f.IsHomework,
                                                DownloadPath = f.DownloadPath,
                                                Date = f.Date,
                                            }).ToList();

                    if (foundedFiles.Count == 0)
                    {
                        addFile = true;
                    }
                    else {
                        foreach (var foundFile in foundedFiles) {
                            var recordId = -1;
                            using var readStream = new FileStream(filePath, FileMode.Open);
                            var readMetadataBytes = new byte[25];
                            var bytesRead = readStream.Read(readMetadataBytes, 0, 25);
                            var readMetadataJson = Encoding.UTF8.GetString(readMetadataBytes, 0, bytesRead);
                            var regex = new Regex(@"\d+");
                            var match = regex.Match(readMetadataJson);

                            if (match.Success)
                            {
                                recordId = int.Parse(match.Value);
                                var recordInfo = DBAccess.UploadedFiles
                                                    .Where(f => f.Id == recordId)
                                                    .Select(f => new UploadedFile
                                                    {
                                                        Name = f.Name,
                                                        OwnerId = f.OwnerId,
                                                        LessonId = f.LessonId,
                                                        IsHomework = f.IsHomework,
                                                        DownloadPath = f.DownloadPath,
                                                        Date = f.Date,
                                                    }).FirstOrDefault();

                                if (recordInfo.LessonId == lessonId && recordInfo.OwnerId == myId)
                                {
                                    addFile = false;
                                }
                                else
                                {
                                    addFile = true;
                                }
                            }
                            else
                            {
                                addFile = true;
                            }
                        }
                    }
                    

                } else {
                    addFile = true;
                }

                if (addFile)
                {
                    resultString = appendText(resultString, file.FileName + sucessfullMsg);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    file.CopyTo(stream);

                    var metadata = new
                    {
                        RecordId = fileRecord.Id,
                    };

                    var metadataJson = JsonConvert.SerializeObject(metadata);
                    var metadataBytes = Encoding.UTF8.GetBytes(metadataJson);

                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Write(metadataBytes, 0, metadataBytes.Length);
                }
                else
                {
                    resultString = appendText(resultString, file.FileName + unsucessfullMsg);
                    DBAccess.UploadedFiles.Remove(fileRecord);
                    DBAccess.SaveChanges();
                }
            }

            return resultString;
        }


        public int RemoveFile(int fileId) {

            var file = DBAccess.UploadedFiles.Find(fileId);

            if (file != null) {
                var folderName = file.IsHomework ? "homeworks" : "materials";
                var wwwrootPath = _webHostEnvironment.WebRootPath;
                var filePath = Path.Combine(wwwrootPath, "uploaded", file.LessonId.ToString(), folderName, file.Name);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    DBAccess.UploadedFiles.Remove(file);
                    DBAccess.SaveChanges();

                    return file.LessonId;

                }

                return -1;
            }
            return -1;
        }

      
        public UploadedFile GetFile(int fileId) {
            var file = DBAccess.UploadedFiles
                            .Where(f => f.Id == fileId)
                            .Select(f => new UploadedFile
                            {
                                Name = f.Name,
                                OwnerId = f.OwnerId,
                                LessonId = f.LessonId,
                                IsHomework = f.IsHomework,
                                DownloadPath = f.DownloadPath,
                                Date = f.Date,
                            }).FirstOrDefault();

            return file;
        }


        private string appendText(string initialText, string toAppend) {
           return initialText + toAppend; 
        }


        public bool Edit(int id, string title, string description, int courseId, string lessonType, DateTime fromDateTime, DateTime toDateTime, string editorId)
        {
            var lessonData = DBAccess.Lessons.Find(id);
            if (lessonData == null)
            {
                return false;
            }

            var courseData = DBAccess.Courses.Find(courseId);
            if (courseData == null || !(courseData.TeacherId == editorId.ToString()))
            {
                return false;
            }

            lessonData.Title = title;
            lessonData.Description = description;
            lessonData.FromDateTime = fromDateTime;
            lessonData.ToDateTime = toDateTime;

            DBAccess.SaveChanges();

            return true;
        }


        public int Delete(int id)
        {
            var lesson = DBAccess.Lessons
                .FirstOrDefault(c => c.Id == id);

            if (lesson != null)
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
                if (lesson.LessonType == DataConstants.LTypeTest)
                {
                    var test = DBAccess.Tests.Where(t => t.LessonId == lesson.Id).FirstOrDefault();

                    if (test != null)
                    {
                        DBAccess.Tests.Remove(test);

                        var questions = DBAccess.Questions.Where(q => q.TestId == test.Id).ToList();

                        if (questions != null)
                        {
                            foreach (var question in questions)
                            {
                                DBAccess.Questions.Remove(question);

                                if (question.Type == DataConstants.QTypeMulti || question.Type == DataConstants.QTypeSingle)
                                {
                                    var selectableQuestionAnswers = DBAccess.Answers.Where(a => a.QuestionId == question.Id).ToList();

                                    if (selectableQuestionAnswers != null)
                                    {
                                        foreach (var a in selectableQuestionAnswers)
                                        {
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


                DBAccess.SaveChanges();
                return lesson.CourseId;
            }

            return -1;
        }

        public bool IsOwnerOfLesson(int id, string userId) {
            var lesson = DBAccess.Lessons.Where(l => l.Id == id).FirstOrDefault();

            if (lesson != null) {
                var course = DBAccess.Courses.Where(c => c.Id == lesson.CourseId).FirstOrDefault();

                if (course != null) {
                    return course.TeacherId == userId;
                }
            }
            return false;
        }


        public List<LessonType> GetLessonTypes() {
            return DBAccess.LessonType.Select(t => new LessonType
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }
    }
}
