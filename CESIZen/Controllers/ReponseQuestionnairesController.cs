using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CESIZen.Models;
using CesiZen.Data;

namespace CESIZen.Controllers
{
    public class ReponseQuestionnairesController : Controller
    {
        private readonly CesiZenDbContext _context;

        public ReponseQuestionnairesController(CesiZenDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(List<int> evenementsSelectionnesIds)
        {
            if (evenementsSelectionnesIds == null || !evenementsSelectionnesIds.Any())
            {
                ModelState.AddModelError("", "Aucun événement sélectionné.");
                return View("Formulaire", await _context.Questionnaires.ToListAsync());
            }

            int? utilisateurId = null;

            if (User.Identity.IsAuthenticated)
            {
                utilisateurId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            }

            var reponseQuestionnaire = new ReponseQuestionnaire
            {
                UtilisateurId = utilisateurId, 
                DateReponse = DateTime.Now
            };

            _context.ReponsesQuestionnaire.Add(reponseQuestionnaire);
            await _context.SaveChangesAsync();

            int total = 0;

            foreach (var evenementId in evenementsSelectionnesIds)
            {
                var evenement = await _context.Questionnaires.FindAsync(evenementId);
                if (evenement != null)
                {
                    total += evenement.Valeur;

                    var reponseEvenement = new ReponseEvenement
                    {
                        ReponseQuestionnaireId = reponseQuestionnaire.Id,
                        QuestionnaireStressId = evenementId
                    };
                    _context.ReponsesEvenement.Add(reponseEvenement);
                }
            }

            await _context.SaveChangesAsync();

            var message = GetStressMessage(total);

            TempData["StressScore"] = total;
            TempData["StressMessage"] = message;

            return RedirectToAction("Resultat");
        }


        public IActionResult Resultat()
        {
            ViewBag.Score = TempData["StressScore"];
            ViewBag.Message = TempData["StressMessage"];
            return View();
        }

        private string GetStressMessage(int totalPoints)
        {
            if (totalPoints > 300)
                return "Votre score dépasse 300 points. Vous avez environ 80% de risques de tomber malade. Prenez soin de vous.";
            if (totalPoints > 200)
                return "Votre score est compris entre 200 et 300 points. Vous avez environ 50% de risques de tomber malade.";
            if (totalPoints >= 150)
                return "Votre score est compris entre 150 et 200 points. Vous avez environ 37% de risques de tomber malade.";
            return "Votre score est inférieur à 150 points. Le risque est faible, continuez à prendre soin de vous.";
        }

        public async Task<IActionResult> Formulaire()
        {
            var questions = await _context.Questionnaires.ToListAsync();
            return View(questions);
        }


    }
}
