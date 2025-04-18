using CesiZen.Data;
using CESIZen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CESIZen.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class QuestionnaireStressesAdminController : Controller
    {
        private readonly CesiZenDbContext _context;

        public QuestionnaireStressesAdminController(CesiZenDbContext context)
        {
            _context = context;
        }

        // GET: QuestionnaireStresses
        public async Task<IActionResult> Index()
        {
            var evenements = await _context.Questionnaires
                .OrderByDescending(q => q.Valeur)
                .ToListAsync();

            return View("~/Views/Admin/QuestionnaireStress/Index.cshtml", evenements);
        }

        // GET: QuestionnaireStresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionnaireStress = await _context.Questionnaires
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionnaireStress == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/QuestionnaireStress/Details.cshtml", questionnaireStress);
        }

        // GET: QuestionnaireStresses/Create
        public IActionResult Create()
        {
            return View("~/Views/Admin/QuestionnaireStress/Create.cshtml");
        }

        // POST: QuestionnaireStresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Libelle,Valeur")] QuestionnaireStress questionnaireStress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionnaireStress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/QuestionnaireStress/Create.cshtml", questionnaireStress);
        }

        // GET: QuestionnaireStresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionnaireStress = await _context.Questionnaires.FindAsync(id);
            if (questionnaireStress == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/QuestionnaireStress/Edit.cshtml", questionnaireStress);
        }

        // POST: QuestionnaireStresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Libelle,Valeur")] QuestionnaireStress questionnaireStress)
        {
            if (id != questionnaireStress.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionnaireStress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionnaireStressExists(questionnaireStress.Id))
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
            return View("~/Views/Admin/QuestionnaireStress/Edit.cshtml", questionnaireStress);
        }

        // GET: QuestionnaireStresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionnaireStress = await _context.Questionnaires
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionnaireStress == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/QuestionnaireStress/Delete.cshtml", questionnaireStress);
        }

        // POST: QuestionnaireStresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionnaireStress = await _context.Questionnaires.FindAsync(id);
            if (questionnaireStress != null)
            {
                _context.Questionnaires.Remove(questionnaireStress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionnaireStressExists(int id)
        {
            return _context.Questionnaires.Any(e => e.Id == id);
        }
    }
}
