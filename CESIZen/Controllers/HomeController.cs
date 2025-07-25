using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CESIZen.Models;

namespace CesiZen.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

     public IActionResult Index()
        {
            try
            {
                var testNumber = 42;
                var result = MathHelper.IsEven(testNumber); 
                ViewBag.MathTestResult = $"Le nombre {testNumber} est {(result ? "pair" : "impair")}";
            }
            catch (Exception ex)
            {
                ViewBag.MathTestResult = $"❌ Erreur détectée: {ex.Message}";
                _logger.LogError(ex, "Erreur lors du test de la méthode IsEven pour SonarCloud");
            }

            return View();
        }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}