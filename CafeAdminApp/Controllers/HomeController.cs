using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CafeAdminApp.Models;

namespace CafeAdminApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Home";

        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["ActivePage"] = "Privacy";

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
