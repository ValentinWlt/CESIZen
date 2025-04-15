using CesiZen.Data;
using CESIZen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CESIZen.Controllers
{
    [Route("[controller]/[action]")]
    public class InformationController : Controller
    {
        private readonly CesiZenDbContext _context;

        public InformationController(CesiZenDbContext context)
        {
            _context = context;
        }

        // GET: /Information
        public async Task<IActionResult> Index()
        {
            var informations = await _context.Informations
                .Where(i => i.EstPublie)
                .OrderBy(i => i.OrdreAffichage)
                .ToListAsync();

            return View("~/Views/Information/Index.cshtml", informations);
        }

        // GET: /Information/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var information = await _context.Informations
                .FirstOrDefaultAsync(i => i.Id == id && i.EstPublie);

            if (information == null) return NotFound();

            return View("~/Views/Information/Details.cshtml", information);
        }
    }
}
