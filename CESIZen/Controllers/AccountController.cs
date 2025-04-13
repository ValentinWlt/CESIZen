using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CESIZen.Models.ViewModels;
using CesiZen.Controllers;
using CESIZen.Models; // Ajoutez ceci pour accéder à la classe Utilisateur

namespace CESIZen.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AccountController(
            UserManager<Utilisateur> userManager,
            SignInManager<Utilisateur> signInManager,
            RoleManager<IdentityRole<int>> _roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = _roleManager;
        }

        // Action pour afficher la page de connexion
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Action pour traiter la connexion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tentative de connexion invalide.");
                    return View(model);
                }
            }
            return View(model);
        }

        // Action pour afficher la page d'inscription
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Action pour traiter l'inscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Utilisez la classe Utilisateur au lieu de IdentityUser
                var user = new Utilisateur
                {
                    UserName = model.Email,
                    Email = model.Email,
                    // Ajoutez les propriétés personnalisées
                    Nom = model.Nom, // Assurez-vous que RegisterViewModel a cette propriété
                    Prenom = model.Prenom, // Assurez-vous que RegisterViewModel a cette propriété
                    Statut = "Actif"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Attribuer le rôle "User" par défaut
                    await _userManager.AddToRoleAsync(user, "User");

                    // Connecter l'utilisateur immédiatement après l'inscription
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToLocal(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // Action pour la déconnexion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // Méthode pour rediriger vers une URL locale
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        // Page d'accès refusé
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
