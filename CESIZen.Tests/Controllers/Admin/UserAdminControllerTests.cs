using CESIZen.Controllers.Admin;
using CESIZen.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

namespace CESIZen.Tests.Controllers.Admin
{
    [TestClass]
    public class UserAdminControllerTests
    {
        private Mock<UserManager<Utilisateur>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<Utilisateur>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<Utilisateur>>();
            var userValidatorsMock = new List<Mock<IUserValidator<Utilisateur>>>();
            var passwordValidatorsMock = new List<Mock<IPasswordValidator<Utilisateur>>>();
            var keyNormalizerMock = new Mock<ILookupNormalizer>();
            var errorsMock = new Mock<IdentityErrorDescriber>();
            var servicesMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<Utilisateur>>>();

            return new Mock<UserManager<Utilisateur>>(
                userStoreMock.Object, options.Object, passwordHasherMock.Object,
                userValidatorsMock.Select(m => m.Object),
                passwordValidatorsMock.Select(m => m.Object),
                keyNormalizerMock.Object, errorsMock.Object, servicesMock.Object,
                loggerMock.Object);
        }

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
        public async Task UsersList_ReturnsViewWithUsersAndRoles()
        {
            
            var users = new List<Utilisateur>
    {
        new Utilisateur { Id = 1, Nom = "Test1", Prenom = "User1", Email = "test1@example.com" },
        new Utilisateur { Id = 2, Nom = "Test2", Prenom = "User2", Email = "test2@example.com" }
    };

            var mockUserManager = GetMockUserManager();

            // Créer un mock qui prend en charge ToListAsync
            var mockDbSet = users.AsQueryable().BuildMockDbSet();

            mockUserManager.Setup(m => m.Users).Returns(mockDbSet.Object);

            mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<Utilisateur>()))
                           .ReturnsAsync(new List<string> { "User" });

            var mockRoleManager = new Mock<RoleManager<IdentityRole<int>>>(
                Mock.Of<IRoleStore<IdentityRole<int>>>(), null, null, null, null);

            var controller = new UserAdminController(mockUserManager.Object, mockRoleManager.Object);

            
            var result = await controller.UsersList();

            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(List<Utilisateur>));
            var model = viewResult.Model as List<Utilisateur>;
            Assert.AreEqual(2, model.Count);
        }

        [TestMethod]
        public async Task Details_UserExists_ReturnsViewWithUserAndRoles()
        {
            
            var user = new Utilisateur { Id = 1, Nom = "Test", Prenom = "User", Email = "test@example.com" };

            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
            mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            var mockRoleManager = new Mock<RoleManager<IdentityRole<int>>>(
                Mock.Of<IRoleStore<IdentityRole<int>>>(), null, null, null, null);

            var controller = new UserAdminController(mockUserManager.Object, mockRoleManager.Object);

            
            var result = await controller.Details(1);

            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(user, viewResult.Model);
        }

        [TestMethod]
        public void Create_Get_ReturnsViewWithRoles()
        {
            
            var mockUserManager = GetMockUserManager();

            var roles = new List<IdentityRole<int>>
    {
        new IdentityRole<int> { Name = "Admin" },
        new IdentityRole<int> { Name = "User" }
    };

            var mockRoleStore = new Mock<IRoleStore<IdentityRole<int>>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole<int>>>(
                mockRoleStore.Object, null, null, null, null);

            mockRoleManager.Setup(r => r.Roles).Returns(roles.AsQueryable());

            var controller = new UserAdminController(mockUserManager.Object, mockRoleManager.Object);

            
            var result = controller.Create();

            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var viewBagRoles = controller.ViewBag.AvailableRoles as SelectList;
            Assert.IsNotNull(viewBagRoles);
            CollectionAssert.AreEquivalent(roles.Select(r => r.Name).ToList(), viewBagRoles.Items.Cast<string>().ToList());
        }

        [TestMethod]
        public async Task Create_Post_ValidModel_CreatesUserAndRedirects()
        {
            
            var mockUserManager = GetMockUserManager();
            var mockRoleManager = new Mock<RoleManager<IdentityRole<int>>>(
                Mock.Of<IRoleStore<IdentityRole<int>>>(), null, null, null, null);

            var utilisateur = new Utilisateur { Id = 1, Mail = "test@example.com" };

            mockUserManager.Setup(um => um.CreateAsync(utilisateur, "Password123"))
                           .ReturnsAsync(IdentityResult.Success);

            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Admin"))
                           .ReturnsAsync(true);

            mockUserManager.Setup(um => um.AddToRoleAsync(utilisateur, "Admin"))
                           .ReturnsAsync(IdentityResult.Success);

            mockRoleManager.Setup(rm => rm.Roles)
                           .Returns(new List<IdentityRole<int>> {
                       new IdentityRole<int> { Name = "Admin" }
                           }.AsQueryable());

            var controller = new UserAdminController(mockUserManager.Object, mockRoleManager.Object);

            
            var result = await controller.Create(utilisateur, "Password123", "Admin");

            
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("UsersList", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Edit_Get_ReturnsViewWithUserAndRoles()
        {
            
            var utilisateur = new Utilisateur
            {
                Id = 1,
                Nom = "NomTest",
                Prenom = "PrenomTest",
                Mail = "test@example.com",
                Tel = "0123456789",
                Statut = "Actif"
            };

            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(utilisateur);
            mockUserManager.Setup(m => m.GetRolesAsync(utilisateur)).ReturnsAsync(new List<string> { "User" });

            var roles = new List<IdentityRole<int>>
    {
        new IdentityRole<int> { Name = "Admin" },
        new IdentityRole<int> { Name = "User" }
    };
            var mockRoleManager = new Mock<RoleManager<IdentityRole<int>>>(
                Mock.Of<IRoleStore<IdentityRole<int>>>(), null, null, null, null);
            var mockRolesDbSet = roles.AsQueryable().BuildMockDbSet();
            mockRoleManager.Setup(r => r.Roles).Returns(mockRolesDbSet.Object);

            var controller = new UserAdminController(mockUserManager.Object, mockRoleManager.Object);

            
            var result = await controller.Edit(1);

            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(Utilisateur));
            var model = viewResult.Model as Utilisateur;
            Assert.AreEqual(utilisateur.Nom, model.Nom);
            Assert.AreEqual(utilisateur.Mail, model.Mail);
        }


        [TestMethod]
        public async Task Edit_Post_ValidUser_UpdatesSuccessfully()
        {
            
            var utilisateur = new Utilisateur
            {
                Id = 1,
                Nom = "AncienNom",
                Prenom = "AncienPrenom",
                Mail = "ancien@mail.com",
                UserName = "ancien@mail.com",
                Email = "ancien@mail.com",
                Tel = "0123456789",
                Statut = "Inactif"
            };

            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(utilisateur);
            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<Utilisateur>())).ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<Utilisateur>()))
                           .ReturnsAsync(new List<string> { "User" });

            mockUserManager.Setup(m => m.RemoveFromRolesAsync(It.IsAny<Utilisateur>(), It.IsAny<IEnumerable<string>>()))
                           .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<Utilisateur>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);

            var controller = new UserAdminController(mockUserManager.Object, GetMockRoleManager().Object);
            controller.ModelState.Clear(); 

            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var utilisateurModifie = new Utilisateur
            {
                Id = 1,
                Nom = "NouveauNom",
                Prenom = "NouveauPrenom",
                Mail = "nouveau@mail.com",
                UserName = "nouveau@mail.com",
                Email = "nouveau@mail.com",
                Tel = "0987654321",
                Statut = "Actif"
            };

             
            var result = await controller.Edit(1, utilisateurModifie, selectedRole: null);

            
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("UsersList", redirectResult.ActionName);

            mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<Utilisateur>()), Times.Once());
        }

        [TestMethod]
        public async Task Delete_Get_ValidId_ReturnsViewWithUserAndRoles()
        {
            
            var utilisateur = new Utilisateur
            {
                Id = 1,
                Nom = "test",
                Prenom = "test",
                Email = "test@example.com"
            };

            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(utilisateur);
            mockUserManager.Setup(m => m.GetRolesAsync(utilisateur)).ReturnsAsync(new List<string> { "User" });

            var controller = new UserAdminController(mockUserManager.Object, GetMockRoleManager().Object);

            
            var result = await controller.Delete(1);

            
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult, typeof(ViewResult));
            Assert.AreEqual(utilisateur, viewResult.Model);
            Assert.AreEqual("User", ((List<string>)controller.ViewBag.UserRoles).First());
        }

        [TestMethod]
        public async Task DeleteConfirmed_ValidId_DeletesUserAndRedirects()
        {
            
            var utilisateur = new Utilisateur
            {
                Id = 1,
                Nom = "Test",
                Prenom = "User",
                Email = "test@example.com"
            };

            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(utilisateur);
            mockUserManager.Setup(m => m.GetRolesAsync(utilisateur)).ReturnsAsync(new List<string> { "User" });
            mockUserManager.Setup(m => m.RemoveFromRolesAsync(utilisateur, It.IsAny<IEnumerable<string>>()))
                           .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(m => m.DeleteAsync(utilisateur)).ReturnsAsync(IdentityResult.Success);

            var controller = new UserAdminController(mockUserManager.Object, GetMockRoleManager().Object);
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            
            var result = await controller.DeleteConfirmed(1);

            
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = result as RedirectToActionResult;
            Assert.AreEqual("UsersList", redirect.ActionName);

            Assert.AreEqual("L'utilisateur a été supprimé avec succès.", controller.TempData["SuccessMessage"]);

            mockUserManager.Verify(m => m.DeleteAsync(utilisateur), Times.Once);
        }
    }
}