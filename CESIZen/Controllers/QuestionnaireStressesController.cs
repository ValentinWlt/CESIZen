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
            var evenements = await _context.Questionnaires
                .OrderByDescending(q => q.Valeur)
                .ToListAsync();

            return View(evenements);
        }
    }
}
