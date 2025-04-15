using CesiZen.Data;
using CESIZen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[Route("Utilisateur/[controller]/[action]")]
public class QuestionnaireUtilisateurController : Controller
{
    private readonly CesiZenDbContext _context;

    public QuestionnaireUtilisateurController(CesiZenDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var evenements = await _context.Questionnaires.ToListAsync();
        return View(evenements);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Soumettre(List<int> evenementIds)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        var reponse = new ReponseQuestionnaire
        {
            DateReponse = DateTime.Now,
            UtilisateurId = userId,
            ReponsesEvenement = evenementIds.Select(id => new ReponseEvenement { QuestionnaireStressId = id }).ToList()
        };

        _context.Add(reponse);
        await _context.SaveChangesAsync();

        return RedirectToAction("Merci");
    }

    public IActionResult Merci()
    {
        return View();
    }
}
