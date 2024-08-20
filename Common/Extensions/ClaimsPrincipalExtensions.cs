namespace CarRentingSystem.Infrastructure.Extensions
{
    using EducationApp.Data;
    using EducationApp.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using System.Security.Claims;

    public static class ClaimsPrincipalExtensions
    {

        public static string getId(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.NameIdentifier).Value;

    }
}
