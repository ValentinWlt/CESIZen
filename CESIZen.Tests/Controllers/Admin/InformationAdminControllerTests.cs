using CesiZen.Data;
using CESIZen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CESIZen.Controllers.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;


namespace CESIZen.Tests.Controllers.Admin
{
    [TestClass]
    public class InformationAdminControllerTests
    {
        private CesiZenDbContext _context;
        private InformationAdminController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_InformationDb")
                .Options;
            _context = new CesiZenDbContext(options);

            _controller = new InformationAdminController(_context);
        }

        [TestMethod]
        public async Task Index_Returns_ViewResult_With_Informations()
        {
            
            _context.Informations.Add(new Information
            {
                Titre = "Titre test",
                Contenu = "Ceci est un contenu de test.",
                DateCreation = DateTime.Now,
                OrdreAffichage = 1
            });
            await _context.SaveChangesAsync();

            
            var result = await _controller.Index();

            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(List<Information>));

        }

        [TestMethod]
        public async Task Details_ValidId_ReturnsViewWithModel()
        {
            
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new CesiZenDbContext(options);
            var controller = new InformationAdminController(context);

            var info = new Information
            {
                Id = 1,
                Titre = "Titre",
                Contenu = "Contenu",
                DateCreation = DateTime.Now
            };
            context.Informations.Add(info);
            await context.SaveChangesAsync();

            
            var result = await controller.Details(1);

            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(Information));
            var model = viewResult.Model as Information;
            Assert.AreEqual("Titre", model.Titre);
        }
        [TestMethod]
        public void Create_Get_ReturnsView()
        {
            
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CesiZenDbContext(options);
            var controller = new InformationAdminController(context);

            
            var result = controller.Create();

            
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CesiZenDbContext(options);
            var controller = new InformationAdminController(context);

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var info = new Information
            {
                Titre = "Nouveau",
                Contenu = "Contenu important",
                Categorie = "Actu",
                EstPublie = true,
                OrdreAffichage = 1
            };

            
            var result = await controller.Create(info);

            
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual(1, context.Informations.Count());
        }

        [TestMethod]

        public async Task Edit_Get_ValidId_ReturnsViewResultWithInformation()
        {
            
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                var info = new Information
                {
                    Id = 1,
                    Titre = "Test",
                    Contenu = "Contenu test",
                    DateCreation = DateTime.Now
                };
                context.Informations.Add(info);
                context.SaveChanges();

                var controller = new InformationAdminController(context);

                
                var result = await controller.Edit(1);

                
                Assert.IsInstanceOfType(result, typeof(ViewResult));
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);
                Assert.IsInstanceOfType(viewResult.Model, typeof(Information));
                var model = viewResult.Model as Information;
                Assert.AreEqual(1, model.Id);
            }
        }

        [TestMethod]
        public async Task Edit_Post_ValidModel_UpdatesEntityAndRedirects()
        {
            
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                var info = new Information
                {
                    Id = 1,
                    Titre = "Ancien titre",
                    Contenu = "Ancien contenu",
                    Categorie = "Catégorie A",
                    EstPublie = true,
                    OrdreAffichage = 1,
                    DateCreation = DateTime.Now
                };
                context.Informations.Add(info);
                context.SaveChanges();
            }

            using (var context = new CesiZenDbContext(options))
            {
                var controller = new InformationAdminController(context);
                controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

                var updatedInfo = new Information
                {
                    Id = 1,
                    Titre = "Nouveau titre",
                    Contenu = "Nouveau contenu",
                    Categorie = "Catégorie B",
                    EstPublie = false,
                    OrdreAffichage = 2,
                    DateCreation = DateTime.Now
                };

                
                var result = await controller.Edit(1, updatedInfo);

                
                Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
                var redirect = result as RedirectToActionResult;
                Assert.AreEqual("Index", redirect.ActionName);
            }

            using (var context = new CesiZenDbContext(options))
            {
                var infoFromDb = context.Informations.First(i => i.Id == 1);
                Assert.AreEqual("Nouveau titre", infoFromDb.Titre);
                Assert.AreEqual("Nouveau contenu", infoFromDb.Contenu);
                Assert.AreEqual("Catégorie B", infoFromDb.Categorie);
                Assert.IsFalse(infoFromDb.EstPublie);
                Assert.AreEqual(2, infoFromDb.OrdreAffichage);
            }
        }

        [TestMethod]
        public async Task Delete_Get_ValidId_ReturnsViewResultWithInformation()
        {
            
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                var info = new Information
                {
                    Id = 1,
                    Titre = "Test",
                    Contenu = "Contenu test",
                    DateCreation = DateTime.Now
                };
                context.Informations.Add(info);
                context.SaveChanges();

                var controller = new InformationAdminController(context);

                
                var result = await controller.Delete(1);

                
                Assert.IsInstanceOfType(result, typeof(ViewResult));
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);
                Assert.IsInstanceOfType(viewResult.Model, typeof(Information));
                var model = viewResult.Model as Information;
                Assert.AreEqual(1, model.Id);
            }
        }

        [TestMethod]
        public async Task DeleteConfirmed_RemovesEntityAndRedirects()
        {
            
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                var info = new Information
                {
                    Id = 1,
                    Titre = "Test",
                    Contenu = "Contenu test",
                    DateCreation = DateTime.Now
                };
                context.Informations.Add(info);
                context.SaveChanges();
            }

            using (var context = new CesiZenDbContext(options))
            {
                var controller = new InformationAdminController(context);
                controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

                
                var result = await controller.DeleteConfirmed(1);

                
                Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
                var redirect = result as RedirectToActionResult;
                Assert.AreEqual("Index", redirect.ActionName);
            }

            using (var context = new CesiZenDbContext(options))
            {
                var infoInDb = context.Informations.FirstOrDefault(i => i.Id == 1);
                Assert.IsNull(infoInDb); 
            }
        }

    }

}
