using EducationApp.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ReturnCustomErrorViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string errorTitle, string errorMessage)
    {
        var errorModel = new CustomErrorModel
        {
            ErrorTitle = errorTitle,
            ErrorMessage = errorMessage
        };

        return View("ReturnCustomError", errorModel);
    }
}
