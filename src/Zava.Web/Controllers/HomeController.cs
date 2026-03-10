using Microsoft.AspNetCore.Mvc;

namespace Zava.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult SourceCode()
    {
        return View();
    }
}
