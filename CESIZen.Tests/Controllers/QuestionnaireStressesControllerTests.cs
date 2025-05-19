using Microsoft.VisualStudio.TestTools.UnitTesting;
using CESIZen.Controllers;
using CesiZen.Data;
using CESIZen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace CESIZen.Tests.Controllers
{
    [TestClass]
    public class QuestionnaireStressesControllerTests
    {
        [TestMethod]
        public async Task Index_Returns_ViewResult_WithQuestionnaires()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new CesiZenDbContext(options))
            {
                context.Questionnaires.Add(new QuestionnaireStress { Id = 1, Valeur = 10, Libelle = "Stress test" });
                context.SaveChanges();

                var controller = new QuestionnaireStressesController(context);

                
                var result = await controller.Index();

                
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult, "La vue retournée est nulle");
                var model = viewResult.Model as IEnumerable<QuestionnaireStress>;
                Assert.IsNotNull(model, "Le modèle retourné est nul");
                Assert.AreEqual(1, model.Count(), "Le nombre de questionnaires est incorrect");
            }
        }
    }
}
