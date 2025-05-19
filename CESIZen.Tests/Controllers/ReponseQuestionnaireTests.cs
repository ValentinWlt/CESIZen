using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CESIZen.Controllers;
using CESIZen.Models;
using CesiZen.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace CESIZen.Tests.Controllers
{
    [TestClass]
    public class ReponseQuestionnaireTests
    {
        private CesiZenDbContext _context;
        private ReponseQuestionnairesController _controller;
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CesiZenDbContext(options);
            _controller = new ReponseQuestionnairesController(_context);
        }

        [TestMethod]
        public async Task Submit_ReturnsFormulaireView_WhenNoEventsSelected()
        {
            
            var controller = new ReponseQuestionnairesController(_context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            
            var result = await controller.Submit(new List<int>()) as ViewResult;

            
            Assert.IsNotNull(result);
            Assert.AreEqual("Formulaire", result.ViewName);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Submit_RedirectsToResultat_WhenEvenementsSelectionnesIdsProvided()
        {
            var questionnaire = new QuestionnaireStress { Id = 1, Libelle = "Stress", Valeur = 100 };
            _context.Questionnaires.Add(questionnaire);
            await _context.SaveChangesAsync();

            var evenementsSelectionnes = new List<int> { 1 };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            var user = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = user };
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _controller.TempData = tempData;

            
            var result = await _controller.Submit(evenementsSelectionnes);

            
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Resultat", redirectResult.ActionName);

            Assert.IsNotNull(_controller.TempData["StressScore"]);
            Assert.IsNotNull(_controller.TempData["StressMessage"]);
        }

        [TestMethod]
        public async Task Formulaire_ReturnsViewWithQuestionnaires()
        {
            
            _context.Questionnaires.Add(new QuestionnaireStress { Id = 1, Libelle = "Test Q", Valeur = 50 });
            await _context.SaveChangesAsync();

            
            var result = await _controller.Formulaire() as ViewResult;

            
            Assert.IsNotNull(result);
            var model = result.Model as List<QuestionnaireStress>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Test Q", model[0].Libelle);
        }

        [TestMethod]
        public void Resultat_ReturnsViewWithTempData()
        {
            
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["StressScore"] = 250,
                ["StressMessage"] = "Prenez soin de vous."
            };

            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = tempData;

            
            var result = _controller.Resultat() as ViewResult;

            
            Assert.IsNotNull(result);
            Assert.AreEqual(250, _controller.ViewBag.Score);
            Assert.AreEqual("Prenez soin de vous.", _controller.ViewBag.Message);
        }




    }
}
