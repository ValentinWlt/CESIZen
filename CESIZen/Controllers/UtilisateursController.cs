using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CESIZen.Models;
using CesiZen.Data;
using Microsoft.AspNetCore.Identity;

namespace CESIZen.Controllers
{
    public class UtilisateursController : Controller
    {
        private readonly CesiZenDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UtilisateursController(
            CesiZenDbContext context,
            UserManager<Utilisateur> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Utilisateurs
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();

            // Préparer un dictionnaire pour stocker les rôles de chaque utilisateur
            ViewBag.UserRoles = new Dictionary<int, IList<string>>();

            foreach (var user in users)
            {
                ViewBag.UserRoles[user.Id] = await _userManager.GetRolesAsync(user);
            }

            return View(users);
        }

        // GET: Utilisateurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            ViewBag.UserRoles = await _userManager.GetRolesAsync(utilisateur);
            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        public IActionResult Create()
        {
            ViewBag.AvailableRoles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();
            return View();
        }

        // POST: Utilisateurs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom,Prenom,Mail,Tel,Statut")] Utilisateur utilisateur, string password, string selectedRole)
        {
            if (ModelState.IsValid)
            {
                utilisateur.UserName = utilisateur.Mail; // Utiliser l'email comme nom d'utilisateur

                var result = await _userManager.CreateAsync(utilisateur, password);

                if (result.Succeeded)
                {
                    // Ajouter le rôle sélectionné ou "User" par défaut
                    await _userManager.AddToRoleAsync(utilisateur, !string.IsNullOrEmpty(selectedRole) ? selectedRole : "User");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.AvailableRoles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();
            return View(utilisateur);
        }

        // GET: Utilisateurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Users.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            ViewBag.AvailableRoles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();

            ViewBag.UserRoles = await _userManager.GetRolesAsync(utilisateur);

            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prenom,Mail,Tel,Statut,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Utilisateur utilisateur, string selectedRole)
        {
            if (id != utilisateur.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Récupérer l'utilisateur complet depuis la base de données
                    var userToUpdate = await _userManager.FindByIdAsync(id.ToString());

                    // Mettre à jour les propriétés modifiables
                    userToUpdate.Nom = utilisateur.Nom;
                    userToUpdate.Prenom = utilisateur.Prenom;
                    userToUpdate.Email = utilisateur.Mail;
                    userToUpdate.PhoneNumber = utilisateur.Tel;
                    userToUpdate.Statut = utilisateur.Statut;

                    // Mettre à jour l'utilisateur
                    await _userManager.UpdateAsync(userToUpdate);

                    // Gérer les rôles
                    var userRoles = await _userManager.GetRolesAsync(userToUpdate);

                    if (!string.IsNullOrEmpty(selectedRole) && !userRoles.Contains(selectedRole))
                    {
                        // Retirer tous les rôles existants
                        await _userManager.RemoveFromRolesAsync(userToUpdate, userRoles);

                        // Ajouter le nouveau rôle
                        await _userManager.AddToRoleAsync(userToUpdate, selectedRole);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilisateurExists(utilisateur.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.AvailableRoles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();

            ViewBag.UserRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(id.ToString()));

            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            ViewBag.UserRoles = await _userManager.GetRolesAsync(utilisateur);
            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilisateur = await _userManager.FindByIdAsync(id.ToString());
            if (utilisateur != null)
            {
                var result = await _userManager.DeleteAsync(utilisateur);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View("Delete", utilisateur);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UtilisateurExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
