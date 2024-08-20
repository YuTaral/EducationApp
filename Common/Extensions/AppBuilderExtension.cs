using EducationApp.Data;
using EducationApp.Data.Models;
using EducationApp.Data.Models.Questions;
using eUni.Data;
using Microsoft.AspNetCore.Identity;

namespace EducationApp.Utils.Extensions
{
    public class AppBuilderExtension
    {
        private readonly IApplicationBuilder app;

        public AppBuilderExtension(IApplicationBuilder app)
        {
            this.app = app;
        }


        public void PrepareDB() {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;

            CreateLessonTypes(services);
            CreateRoles(services);
            CreateQuestionTypes(services);
            SeedAdmin(services);
        }


        public void CreateRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task
                .Run(async () =>
                {
                    if (await roleManager.RoleExistsAsync(DataConstants.AdministratorRoleName))
                    {
                        return;
                    }
                    var admin = new IdentityRole { Name = DataConstants.AdministratorRoleName };
                    await roleManager.CreateAsync(admin);


                    if (await roleManager.RoleExistsAsync(DataConstants.TeacherRoleName))
                    {
                        return;
                    }
                    var teacher = new IdentityRole { Name = DataConstants.TeacherRoleName };
                    await roleManager.CreateAsync(teacher);

                    if (await roleManager.RoleExistsAsync(DataConstants.StudentRoleName))
                    {
                        return;
                    }
                    var student = new IdentityRole { Name = DataConstants.StudentRoleName };
                    await roleManager.CreateAsync(student);

                })
                .GetAwaiter()
                .GetResult();

        }

        public void CreateLessonTypes(IServiceProvider services) 
        {
            var DBAccess = services.GetRequiredService<EduAppDbContext>();

            if (DBAccess.LessonType.Any())
            {
                return;
            }

            DBAccess.LessonType.AddRange(new[]
            {
                new LessonType { Name = DataConstants.LTypeLecture },
                new LessonType { Name = DataConstants.LTypeExercise  },
                new LessonType { Name = DataConstants.LTypeTest  },
            });

            DBAccess.SaveChanges();
        }

        public void CreateQuestionTypes(IServiceProvider services)
        {
            var DBAccess = services.GetRequiredService<EduAppDbContext>();

            if (DBAccess.QuestionTypes.Any())
            {
                return;
            }

            DBAccess.QuestionTypes.AddRange(new[]
            {
                new QuestionType { Name = DataConstants.QTypeMulti },
                new QuestionType { Name = DataConstants.QTypeSingle  },
                new QuestionType { Name = DataConstants.QTypeText  },
                new QuestionType { Name = DataConstants.QTypeNumeric  },
            });

            DBAccess.SaveChanges();
        }



        public void SeedAdmin(IServiceProvider services) 
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task
                .Run(async () =>
                {
                   
                    const string adminEmail = "owner@edu.com";
                    const string adminPassword = "systemowner";

                    var user = new User
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        IsApproved = true
                    };

                    await userManager.CreateAsync(user, adminPassword);
                    await userManager.AddToRoleAsync(user, DataConstants.AdministratorRoleName);

                })
                .GetAwaiter()
                .GetResult();


        }
    }
}
