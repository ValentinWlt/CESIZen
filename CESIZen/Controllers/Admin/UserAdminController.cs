using CESIZen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CESIZen.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserAdminController : Controller
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public UserAdminController(UserManager<Utilisateur> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> UsersList()
        {
            var users = await _userManager.Users.ToListAsync();

            var userRolesDict = new Dictionary<int, List<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesDict[user.Id] = roles.ToList();
            }

            ViewBag.UserRoles = userRolesDict;

            return View("~/Views/Admin/Users/Index.cshtml", users);
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var utilisateur = await _userManager.FindByIdAsync(id.ToString());
                if (utilisateur == null)
                {
                    return NotFound();
                }

                // Récupérer les rôles de l'utilisateur
                var userRoles = await _userManager.GetRolesAsync(utilisateur);
                ViewBag.UserRoles = userRoles;

                return View("~/Views/Admin/Users/Details.cshtml", utilisateur);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(UsersList), new { error = "Une erreur est survenue lors de la récupération des détails de l'utilisateur." });
            }
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.AvailableRoles = new SelectList(roles);

            return View("~/Views/Admin/Users/Create.cshtml");
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Utilisateur utilisateur, string Password, string selectedRole)
        {
            if (ModelState.IsValid)
            {
                utilisateur.UserName = utilisateur.Mail;
                utilisateur.Email = utilisateur.Mail;

                var result = await _userManager.CreateAsync(utilisateur, Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(selectedRole) && await _roleManager.RoleExistsAsync(selectedRole))
                    {
                        await _userManager.AddToRoleAsync(utilisateur, selectedRole);
                    }
                    else
                    {
                        if (await _roleManager.RoleExistsAsync("User"))
                        {
                            await _userManager.AddToRoleAsync(utilisateur, "User");
                        }
                    }

                    return RedirectToAction(nameof(UsersList)); 
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.AvailableRoles = new SelectList(roles);

            return View("~/Views/Admin/Users/Create.cshtml", utilisateur);
        }



        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var utilisateur = await _userManager.FindByIdAsync(id.ToString());
            if (utilisateur == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(utilisateur);
            ViewBag.UserRoles = userRoles;

            return View("~/Views/Admin/Users/Delete.cshtml", utilisateur);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilisateur = await _userManager.FindByIdAsync(id.ToString());
            if (utilisateur == null)
            {
                return NotFound();
            }

            try
            {
                var userRoles = await _userManager.GetRolesAsync(utilisateur);
                if (userRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(utilisateur, userRoles);
                }

                var result = await _userManager.DeleteAsync(utilisateur);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "L'utilisateur a été supprimé avec succès.";
                    return RedirectToAction(nameof(UsersList));
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = $"Échec de la suppression : {errors}";

                    ViewBag.UserRoles = userRoles;
                    return View("~/Views/Admin/Users/Delete.cshtml", utilisateur);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Une erreur est survenue lors de la suppression : {ex.Message}";

                var userRoles = await _userManager.GetRolesAsync(utilisateur);
                ViewBag.UserRoles = userRoles;
                return View("~/Views/Admin/Users/Delete.cshtml", utilisateur);
            }
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var utilisateur = await _userManager.FindByIdAsync(id.ToString());
            if (utilisateur == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.AvailableRoles = new SelectList(roles, "Name", "Name");

            var userRoles = await _userManager.GetRolesAsync(utilisateur);
            ViewBag.UserRoles = userRoles;

            return View("~/Views/Admin/Users/Edit.cshtml", utilisateur);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Utilisateur utilisateur, string selectedRole, bool changePassword = false, string newPassword = null, string confirmPassword = null)
        {
            if (id != utilisateur.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _userManager.FindByIdAsync(id.ToString());
                    if (userToUpdate == null)
                    {
                        return NotFound();
                    }

                    userToUpdate.Nom = utilisateur.Nom;
                    userToUpdate.Prenom = utilisateur.Prenom;
                    userToUpdate.Mail = utilisateur.Mail;
                    userToUpdate.Email = utilisateur.Mail;
                    userToUpdate.Tel = utilisateur.Tel;
                    userToUpdate.PhoneNumber = utilisateur.Tel;
                    userToUpdate.Statut = utilisateur.Statut;

                    var updateResult = await _userManager.UpdateAsync(userToUpdate);
                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        var roles = await _roleManager.Roles.ToListAsync();
                        ViewBag.AvailableRoles = new SelectList(roles, "Name", "Name");
                        var userRoles = await _userManager.GetRolesAsync(userToUpdate);
                        ViewBag.UserRoles = userRoles;
                        return View("~/Views/Admin/Users/Edit.cshtml", utilisateur);
                    }

                    if (changePassword && !string.IsNullOrEmpty(newPassword))
                    {
                        if (newPassword != confirmPassword)
                        {
                            ModelState.AddModelError("confirmPassword", "Le mot de passe et sa confirmation ne correspondent pas.");
                            var roles = await _roleManager.Roles.ToListAsync();
                            ViewBag.AvailableRoles = new SelectList(roles, "Name", "Name");
                            var userRoles = await _userManager.GetRolesAsync(userToUpdate);
                            ViewBag.UserRoles = userRoles;
                            return View("~/Views/Admin/Users/Edit.cshtml", utilisateur);
                        }

                        var token = await _userManager.GeneratePasswordResetTokenAsync(userToUpdate);
                        var passwordResult = await _userManager.ResetPasswordAsync(userToUpdate, token, newPassword);

                        if (!passwordResult.Succeeded)
                        {
                            foreach (var error in passwordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            var roles = await _roleManager.Roles.ToListAsync();
                            ViewBag.AvailableRoles = new SelectList(roles, "Name", "Name");
                            var userRoles = await _userManager.GetRolesAsync(userToUpdate);
                            ViewBag.UserRoles = userRoles;
                            return View("~/Views/Admin/Users/Edit.cshtml", utilisateur);
                        }
                    }

                    if (!string.IsNullOrEmpty(selectedRole))
                    {
                        var userRoles = await _userManager.GetRolesAsync(userToUpdate);

                        if (userRoles.Any())
                        {
                            await _userManager.RemoveFromRolesAsync(userToUpdate, userRoles);
                        }

                        await _userManager.AddToRoleAsync(userToUpdate, selectedRole);
                    }

                    TempData["SuccessMessage"] = "L'utilisateur a été mis à jour avec succès.";
                    return RedirectToAction(nameof(UsersList));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Une erreur de concurrence est survenue. L'utilisateur a peut-être été modifié par quelqu'un d'autre.";
                        return RedirectToAction(nameof(UsersList));
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Une erreur est survenue lors de la mise à jour de l'utilisateur: {ex.Message}";
                    return RedirectToAction(nameof(UsersList));
                }
            }

            var availableRoles = await _roleManager.Roles.ToListAsync();
            ViewBag.AvailableRoles = new SelectList(availableRoles, "Name", "Name");
            var currentUserRoles = await _userManager.GetRolesAsync(utilisateur);
            ViewBag.UserRoles = currentUserRoles;
            return View("~/Views/Admin/Users/Edit.cshtml", utilisateur);
        }


        private async Task PrepareViewBagForEdit(Utilisateur user)
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.AvailableRoles = new SelectList(roles);
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
        }


        private async Task<bool> UserExists(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user != null;
        }
    }
}
