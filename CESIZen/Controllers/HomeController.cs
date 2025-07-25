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
            // ❌ ERREURS VOLONTAIRES POUR SONARCLOUD (à ajouter dans votre méthode existante)
            
            // Erreur 1: Variable non utilisée (Code Smell)
            var unusedVariable = "Cette variable ne sert à rien";
            
            // Erreur 2: Division par zéro (Bug Critique)
            var testNumber = 42;
            var zero = 0;
            
            try 
            {
                var result = testNumber / zero; // Division par zéro - SonarCloud va détecter ça !
                ViewBag.TestResult = $"Résultat: {result}";
            }
            catch (Exception ex)
            {
                ViewBag.TestResult = $"Erreur capturée: {ex.Message}";
            }
            
            // Erreur 3: Code jamais atteint (Code Smell)
            if (false)
            {
                Console.WriteLine("Ce code ne s'exécutera jamais");
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