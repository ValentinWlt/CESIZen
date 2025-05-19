
using System.Security.Claims;
using CESIZen.Controllers;
using CESIZen.Models;
using CESIZen.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using CESIZen.Tests.Helpers;

namespace CESIZen.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<RoleManager<IdentityRole<int>>> GetMockRoleManager()
        {
            var rolesList = new List<IdentityRole<int>>
            {
                new IdentityRole<int> { Name = "Admin" },
                new IdentityRole<int> { Name = "User" }
            };

            var roles = rolesList.AsQueryable().BuildMock();

            var store = new Mock<IRoleStore<IdentityRole<int>>>();
            var mock = new Mock<RoleManager<IdentityRole<int>>>(
                store.Object,
                null, null, null, null
            );

            mock.Setup(r => r.Roles).Returns(roles);
            mock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            return mock;
        }


        [TestMethod]
        public void Login_Get_ReturnsViewWithReturnUrl()
        {
            var controller = new AccountController(null, null, null);
            var returnUrl = "/Home/Index";

            var result = controller.Login(returnUrl) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(returnUrl, controller.ViewData["ReturnUrl"]);
        }

        [TestMethod]
        public async Task Login_Post_ValidCredentials_RedirectsToLocal()
        {
            var mockSignInManager = TestHelpers.GetMockSignInManager();
            mockSignInManager.Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = new AccountController(null, mockSignInManager.Object, null);
            controller.ModelState.Clear(); 

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(true);
            controller.Url = mockUrlHelper.Object;

            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = true
            };

            var returnUrl = "/Home/Index";

            var result = await controller.Login(model, returnUrl);

            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(returnUrl, redirectResult.Url);
        }

        [TestMethod]
        public void Register_Get_ReturnsViewWithReturnUrl()
        {
            var controller = new AccountController(null, null, null);
            var returnUrl = "/home/index";

            var result = controller.Register(returnUrl) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(returnUrl, controller.ViewData["ReturnUrl"]);
        }

        [TestMethod]
        public async Task Register_Post_ValidModel_CreatesUserAndRedirects()
        {
            var mockUserManager = TestHelpers.GetMockUserManager();
            var mockSignInManager = TestHelpers.GetMockSignInManager();
            var mockRoleManager = GetMockRoleManager();

            var registerViewModel = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Nom = "Doe",
                Prenom = "John"
            };

            mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<Utilisateur>(), registerViewModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager
                .Setup(m => m.AddToRoleAsync(It.IsAny<Utilisateur>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            mockSignInManager
                .Setup(s => s.SignInAsync(It.IsAny<Utilisateur>(), false, null))
                .Returns(Task.CompletedTask);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockRoleManager.Object);
            controller.ModelState.Clear(); 

            var returnUrl = "/Home/Index";

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.IsLocalUrl(returnUrl)).Returns(true);
            controller.Url = mockUrlHelper.Object;

            var result = await controller.Register(registerViewModel, returnUrl);

            var redirect = result as RedirectResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual(returnUrl, redirect.Url);
        }

        [TestMethod]
        public async Task Logout_CallsSignOutAndRedirectsToHomeIndex()
        {
            var mockSignInManager = TestHelpers.GetMockSignInManager();
            mockSignInManager
                .Setup(s => s.SignOutAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            var controller = new AccountController(null, mockSignInManager.Object, null);

            var result = await controller.Logout();

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Home", redirectResult.ControllerName);

            mockSignInManager.Verify(s => s.SignOutAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Manage_Get_UserExists_ReturnsViewWithModel()
        {
            var utilisateur = new Utilisateur
            {
                Nom = "Doe",
                Prenom = "John",
                Email = "john@example.com",
                PhoneNumber = "0123456789"
            };

            var mockUserManager = TestHelpers.GetMockUserManager();
            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(utilisateur);

            var controller = new AccountController(mockUserManager.Object, null, null);

            var result = await controller.Manage();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as ManageAccountViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(utilisateur.Nom, model.Nom);
            Assert.AreEqual(utilisateur.Email, model.Email);
        }

        [TestMethod]
        public async Task Manage_Post_ValidModel_UpdatesUserAndRedirects()
        {
            var utilisateur = new Utilisateur
            {
                Id = 1,
                Email = "ancien@example.com"
            };

            var mockUserManager = TestHelpers.GetMockUserManager();
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(utilisateur);
            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<Utilisateur>()))
                           .ReturnsAsync(IdentityResult.Success);

            var controller = new AccountController(mockUserManager.Object, null, null);
            controller.ModelState.Clear(); 

            var model = new ManageAccountViewModel
            {
                Nom = "NouveauNom",
                Prenom = "NouveauPrenom",
                Email = "nouveau@example.com",
                PhoneNumber = "0123456789"
            };

            
            var result = await controller.Manage(model);

            
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual("Home", redirect.ControllerName);

            mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<Utilisateur>()), Times.Once);
        }

        [TestMethod]
        public void ChangePassword_Get_ReturnsView()
        {
            
            var controller = new AccountController(null, null, null);

            
            var result = controller.ChangePassword();

            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task ChangePassword_Post_ValidModel_ChangesPasswordAndRedirects()
        {
            
            var utilisateur = new Utilisateur { Email = "test@example.com" };

            var mockUserManager = TestHelpers.GetMockUserManager();
            var mockSignInManager = TestHelpers.GetMockSignInManager();

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(utilisateur);

            mockUserManager.Setup(u => u.ChangePasswordAsync(utilisateur, "Old123!", "New123!"))
                           .ReturnsAsync(IdentityResult.Success);

            mockSignInManager.Setup(s => s.RefreshSignInAsync(utilisateur))
                             .Returns(Task.CompletedTask);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null);
            controller.ModelState.Clear(); 

            var model = new ChangePasswordViewModel
            {
                OldPassword = "Old123!",
                NewPassword = "New123!",
                ConfirmPassword = "New123!"
            };

            
            var result = await controller.ChangePassword(model);

            
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual("Home", redirect.ControllerName);
        }

    }
}
