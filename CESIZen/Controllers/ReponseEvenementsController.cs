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
    public class ReponseEvenementsController : Controller
    {
        private readonly CesiZenDbContext _context;

        public ReponseEvenementsController(CesiZenDbContext context)
        {
            _context = context;
        }

        // GET: ReponseEvenements
        public async Task<IActionResult> Index()
        {
            var cesiZenDbContext = _context.ReponsesEvenement.Include(r => r.QuestionnaireStress).Include(r => r.ReponseQuestionnaire);
            return View(await cesiZenDbContext.ToListAsync());
        }

        // GET: ReponseEvenements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reponseEvenement = await _context.ReponsesEvenement
                .Include(r => r.QuestionnaireStress)
                .Include(r => r.ReponseQuestionnaire)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reponseEvenement == null)
            {
                return NotFound();
            }

            return View(reponseEvenement);
        }

        // GET: ReponseEvenements/Create
        public IActionResult Create()
        {
            ViewData["QuestionnaireStressId"] = new SelectList(_context.Questionnaires, "Id", "Libelle");
            ViewData["ReponseQuestionnaireId"] = new SelectList(_context.ReponsesQuestionnaire, "Id", "Id");
            return View();
        }

        // POST: ReponseEvenements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReponseQuestionnaireId,QuestionnaireStressId,EstSurvenu,ValeurPoints")] ReponseEvenement reponseEvenement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reponseEvenement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionnaireStressId"] = new SelectList(_context.Questionnaires, "Id", "Libelle", reponseEvenement.QuestionnaireStressId);
            ViewData["ReponseQuestionnaireId"] = new SelectList(_context.ReponsesQuestionnaire, "Id", "Id", reponseEvenement.ReponseQuestionnaireId);
            return View(reponseEvenement);
        }

        // GET: ReponseEvenements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reponseEvenement = await _context.ReponsesEvenement.FindAsync(id);
            if (reponseEvenement == null)
            {
                return NotFound();
            }
            ViewData["QuestionnaireStressId"] = new SelectList(_context.Questionnaires, "Id", "Libelle", reponseEvenement.QuestionnaireStressId);
            ViewData["ReponseQuestionnaireId"] = new SelectList(_context.ReponsesQuestionnaire, "Id", "Id", reponseEvenement.ReponseQuestionnaireId);
            return View(reponseEvenement);
        }

        // POST: ReponseEvenements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReponseQuestionnaireId,QuestionnaireStressId,EstSurvenu,ValeurPoints")] ReponseEvenement reponseEvenement)
        {
            if (id != reponseEvenement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reponseEvenement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReponseEvenementExists(reponseEvenement.Id))
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
            ViewData["QuestionnaireStressId"] = new SelectList(_context.Questionnaires, "Id", "Libelle", reponseEvenement.QuestionnaireStressId);
            ViewData["ReponseQuestionnaireId"] = new SelectList(_context.ReponsesQuestionnaire, "Id", "Id", reponseEvenement.ReponseQuestionnaireId);
            return View(reponseEvenement);
        }

        // GET: ReponseEvenements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reponseEvenement = await _context.ReponsesEvenement
                .Include(r => r.QuestionnaireStress)
                .Include(r => r.ReponseQuestionnaire)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reponseEvenement == null)
            {
                return NotFound();
            }

            return View(reponseEvenement);
        }

        // POST: ReponseEvenements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reponseEvenement = await _context.ReponsesEvenement.FindAsync(id);
            if (reponseEvenement != null)
            {
                _context.ReponsesEvenement.Remove(reponseEvenement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReponseEvenementExists(int id)
        {
            return _context.ReponsesEvenement.Any(e => e.Id == id);
        }
    }
}
