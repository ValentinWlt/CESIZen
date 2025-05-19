using CesiZen.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CESIZen.Models;

namespace CESIZen.Controllers
{
    public class QuestionnaireStressesController : Controller
    {
        private readonly CesiZenDbContext _context;

        public QuestionnaireStressesController(CesiZenDbContext context)
        {
            _context = context;
        }

        // GET: QuestionnaireStresses
        public async Task<IActionResult> Index()
        {
            var questionnaireStresses = await _context.Questionnaires
                .OrderByDescending(q => q.Valeur)
                .ToListAsync();

            return View(questionnaireStresses);
        }
    }
}
