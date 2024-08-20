using EducationApp.Data;
using EducationApp.Data.Models;
using EducationApp.Data.Services.Lessons;
using EducationApp.Data.Services.Subjects;
using EducationApp.Data.Services.Tests;
using EducationApp.Data.Services.User;
using EducationApp.Utils.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<EduAppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<EduAppDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
.AddRazorRuntimeCompilation();

builder.Services.AddTransient<ICourseService, CourseService>();
builder.Services.AddTransient<ILessonService, LessonService>();
builder.Services.AddTransient<ITestService, TestService>();
builder.Services.AddTransient<IUserService, UserService>();


var app = builder.Build();

//Uncomment to seed admin and create the necessary db data
var appBuilderExtension = new AppBuilderExtension(app);
appBuilderExtension.PrepareDB();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages();
});


app.Run();
