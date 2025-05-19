using CesiZen.Data;
using CESIZen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CESIZen.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class InformationAdminController : Controller
    {
        private readonly CesiZenDbContext _context;

        public InformationAdminController(CesiZenDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Information
        public async Task<IActionResult> Index()
        {
            var informations = await _context.Informations
            .OrderBy(i => i.OrdreAffichage)
             .ToListAsync();

            return View("~/Views/Admin/Information/Index.cshtml", informations);
        }

        // GET: Admin/Information/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var information = await _context.Informations
                .FirstOrDefaultAsync(m => m.Id == id);

            if (information == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Information/Details.cshtml", information);
        }

        // GET: Admin/Information/Create
        public IActionResult Create()
        {
            return View("~/Views/Admin/Information/Create.cshtml");
        }

        // POST: Admin/Information/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titre,Contenu,Categorie,EstPublie,OrdreAffichage")] Information information)
        {
            if (ModelState.IsValid)
            {
                information.DateCreation = DateTime.Now;

                _context.Add(information);
                await _context.SaveChangesAsync();
                TempData["Success"] = "L'information a été créée avec succès.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Information/Create.cshtml", information);
        }

        // GET: Admin/Information/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var information = await _context.Informations.FindAsync(id);
            if (information == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Information/Edit.cshtml", information);
        }

        // POST: Admin/Information/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Contenu,Categorie,EstPublie,OrdreAffichage,DateCreation")] Information information)
        {
            if (id != information.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(information);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "L'information a été modifiée avec succès.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InformationExists(information.Id))
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
            return View("~/Views/Admin/Information/Edit.cshtml", information);
        }

        private bool InformationExists(int id)
        {
            return _context.Informations.Any(e => e.Id == id);
        }

        // GET: Admin/Information/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var information = await _context.Informations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (information == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Information/Delete.cshtml", information);
        }

        // POST: Admin/Information/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var information = await _context.Informations.FindAsync(id);
            if (information != null)
            {
                _context.Informations.Remove(information);
                await _context.SaveChangesAsync();
                TempData["Success"] = "L'information a été supprimée avec succès.";
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
