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
    public class DroitsController : Controller
    {
        private readonly CesiZenDbContext _context;

        public DroitsController(CesiZenDbContext context)
        {
            _context = context;
        }

        // GET: Droits
        public async Task<IActionResult> Index()
        {
            return View(await _context.Droits.ToListAsync());
        }

        // GET: Droits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var droit = await _context.Droits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (droit == null)
            {
                return NotFound();
            }

            return View(droit);
        }

        // GET: Droits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Droits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypeDroit")] Droit droit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(droit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(droit);
        }

        // GET: Droits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var droit = await _context.Droits.FindAsync(id);
            if (droit == null)
            {
                return NotFound();
            }
            return View(droit);
        }

        // POST: Droits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeDroit")] Droit droit)
        {
            if (id != droit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(droit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DroitExists(droit.Id))
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
            return View(droit);
        }

        // GET: Droits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var droit = await _context.Droits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (droit == null)
            {
                return NotFound();
            }

            return View(droit);
        }

        // POST: Droits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var droit = await _context.Droits.FindAsync(id);
            if (droit != null)
            {
                _context.Droits.Remove(droit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DroitExists(int id)
        {
            return _context.Droits.Any(e => e.Id == id);
        }
    }
}
