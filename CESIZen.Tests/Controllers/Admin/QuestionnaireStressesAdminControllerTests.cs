using CESIZen.Controllers;
using CesiZen.Data;
using CESIZen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CESIZen.Controllers.Admin;

namespace CESIZen.Tests.Controllers.Admin
{
    [TestClass]
    public class QuestionnaireStressesAdminControllerTests
    {
        [TestMethod]
        public async Task Index_Returns_ViewResult_WithQuestionnaires()
        {
            
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: "Index_Test_Database")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                context.Questionnaires.RemoveRange(context.Questionnaires);
                context.SaveChanges();

                context.Questionnaires.Add(new QuestionnaireStress { Id = 1, Valeur = 10, Libelle = "Stress test" });
                context.SaveChanges();

                // Vérifier que vous utilisez le bon contrôleur ici
                var controller = new QuestionnaireStressesAdminController(context);

                
                var result = await controller.Index();

                
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult, "La vue retournée est nulle");
                var model = viewResult.Model as IEnumerable<QuestionnaireStress>;
                Assert.IsNotNull(model, "Le modèle retourné est nul");
                Assert.AreEqual(1, model.Count(), "Le nombre de questionnaires est incorrect");
            }
        }

        [TestMethod]
        public async Task Edit_Get_ValidId_ReturnsViewWithModel()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase("Edit_Get_Test")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                var existing = new QuestionnaireStress { Id = 1, Libelle = "Avant", Valeur = 5 };
                context.Questionnaires.Add(existing);
                context.SaveChanges();

                var controller = new QuestionnaireStressesAdminController(context);

                var result = await controller.Edit(1);

                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);

                var model = viewResult.Model as QuestionnaireStress;
                Assert.IsNotNull(model);
                Assert.AreEqual("Avant", model.Libelle);
            }
        }

        [TestMethod]
        public async Task Edit_Post_ValidModel_UpdatesEntityAndRedirects()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase("Edit_Post_Test")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                var original = new QuestionnaireStress { Id = 1, Libelle = "Avant", Valeur = 5 };
                context.Questionnaires.Add(original);
                context.SaveChanges();

                context.Entry(original).State = EntityState.Detached;

                var controller = new QuestionnaireStressesAdminController(context);

                
                var updated = new QuestionnaireStress { Id = 1, Libelle = "Après", Valeur = 10 };
                var result = await controller.Edit(1, updated);

                
                var redirectResult = result as RedirectToActionResult;
                Assert.IsNotNull(redirectResult);
                Assert.AreEqual("Index", redirectResult.ActionName);

                var updatedItem = context.Questionnaires.First();
                Assert.AreEqual("Après", updatedItem.Libelle);
                Assert.AreEqual(10, updatedItem.Valeur);
            }
        }

        [TestMethod]
        public async Task Delete_Get_ValidId_ReturnsViewWithModel()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase("Delete_Get_Test")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                
                var questionnaire = new QuestionnaireStress
                {
                    Id = 1,
                    Libelle = "À supprimer",
                    Valeur = 42
                };
                context.Questionnaires.Add(questionnaire);
                context.SaveChanges();

                var controller = new QuestionnaireStressesAdminController(context);

                
                var result = await controller.Delete(1);

                
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);

                var model = viewResult.Model as QuestionnaireStress;
                Assert.IsNotNull(model);
                Assert.AreEqual("À supprimer", model.Libelle);
                Assert.AreEqual(42, model.Valeur);
            }
        }

        [TestMethod]
        public async Task Delete_Post_ValidId_DeletesEntityAndRedirects()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase("Delete_Post_Test")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                
                var questionnaire = new QuestionnaireStress { Id = 1, Libelle = "À supprimer", Valeur = 999 };
                context.Questionnaires.Add(questionnaire);
                context.SaveChanges();

                var controller = new QuestionnaireStressesAdminController(context);

                
                var result = await controller.DeleteConfirmed(1);

                
                var redirectResult = result as RedirectToActionResult;
                Assert.IsNotNull(redirectResult);
                Assert.AreEqual("Index", redirectResult.ActionName);

                var entityInDb = await context.Questionnaires.FindAsync(1);
                Assert.IsNull(entityInDb);
            }
        }

        [TestMethod]
        public async Task Details_ValidId_ReturnsViewWithModel()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase("Details_Get_Test")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                
                var questionnaire = new QuestionnaireStress { Id = 1, Libelle = "Stress test", Valeur = 20 };
                context.Questionnaires.Add(questionnaire);
                context.SaveChanges();

                var controller = new QuestionnaireStressesAdminController(context);

                
                var result = await controller.Details(1);

                
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);

                var model = viewResult.Model as QuestionnaireStress;
                Assert.IsNotNull(model);
                Assert.AreEqual("Stress test", model.Libelle);
                Assert.AreEqual(20, model.Valeur);
            }
        }

        [TestMethod]
        public void Create_Get_ReturnsView()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase("Create_Get_Test")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                var controller = new QuestionnaireStressesAdminController(context);

                
                var result = controller.Create();

                
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);
            }
        }

        [TestMethod]
        public async Task Create_Post_ValidModel_AddsEntityAndRedirects()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase("Create_Post_Test")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                var controller = new QuestionnaireStressesAdminController(context);

                var newItem = new QuestionnaireStress
                {
                    Id = 1,
                    Libelle = "Nouveau stress",
                    Valeur = 42
                };

                
                var result = await controller.Create(newItem);

                
                var redirectResult = result as RedirectToActionResult;
                Assert.IsNotNull(redirectResult);
                Assert.AreEqual("Index", redirectResult.ActionName);

                var itemInDb = await context.Questionnaires.FindAsync(1);
                Assert.IsNotNull(itemInDb);
                Assert.AreEqual("Nouveau stress", itemInDb.Libelle);
                Assert.AreEqual(42, itemInDb.Valeur);
            }
        }
    }
}
