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

        // GET: ReponseQuestionnaires
        public async Task<IActionResult> Index()
        {
            var cesiZenDbContext = _context.ReponsesQuestionnaire.Include(r => r.Utilisateur);
            return View(await cesiZenDbContext.ToListAsync());
        }

        // GET: ReponseQuestionnaires/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reponseQuestionnaire = await _context.ReponsesQuestionnaire
                .Include(r => r.Utilisateur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reponseQuestionnaire == null)
            {
                return NotFound();
            }

            return View(reponseQuestionnaire);
        }

        // GET: ReponseQuestionnaires/Create
        public IActionResult Create()
        {
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Nom");
            return View();
        }

        // POST: ReponseQuestionnaires/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UtilisateurId,DateReponse")] ReponseQuestionnaire reponseQuestionnaire)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reponseQuestionnaire);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Nom", reponseQuestionnaire.UtilisateurId);
            return View(reponseQuestionnaire);
        }

        // GET: ReponseQuestionnaires/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reponseQuestionnaire = await _context.ReponsesQuestionnaire.FindAsync(id);
            if (reponseQuestionnaire == null)
            {
                return NotFound();
            }
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Nom", reponseQuestionnaire.UtilisateurId);
            return View(reponseQuestionnaire);
        }

        // POST: ReponseQuestionnaires/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UtilisateurId,DateReponse")] ReponseQuestionnaire reponseQuestionnaire)
        {
            if (id != reponseQuestionnaire.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reponseQuestionnaire);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReponseQuestionnaireExists(reponseQuestionnaire.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UtilisateurId"] = new SelectList(_context.Users, "Id", "Nom", reponseQuestionnaire.UtilisateurId);
            return View(reponseQuestionnaire);
        }

        // GET: ReponseQuestionnaires/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reponseQuestionnaire = await _context.ReponsesQuestionnaire
                .Include(r => r.Utilisateur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reponseQuestionnaire == null)
            {
                return NotFound();
            }

            return View(reponseQuestionnaire);
        }

        // POST: ReponseQuestionnaires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reponseQuestionnaire = await _context.ReponsesQuestionnaire.FindAsync(id);
            if (reponseQuestionnaire != null)
            {
                _context.ReponsesQuestionnaire.Remove(reponseQuestionnaire);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReponseQuestionnaireExists(int id)
        {
            return _context.ReponsesQuestionnaire.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(List<int> evenementsSelectionnesIds)
        {
            if (evenementsSelectionnesIds == null || !evenementsSelectionnesIds.Any())
            {
                ModelState.AddModelError("", "Aucun événement sélectionné.");
                return View(); 
            }

            var utilisateurId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var utilisateur = await _context.Utilisateurs.FindAsync(utilisateurId);
            if (utilisateur == null)
            {
                return Unauthorized();
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
