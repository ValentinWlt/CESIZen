using System.Security.Claims;
using CesiZen.Data;
using CESIZen.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[TestClass]
public class QuestionnaireUtilisateurTests
{
    private CesiZenDbContext _context;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CesiZenDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        _context = new CesiZenDbContext(options);

        _context.Questionnaires.AddRange(new List<QuestionnaireStress>
        {
            new QuestionnaireStress { Id = 1, Libelle = "Q1" },
            new QuestionnaireStress { Id = 2, Libelle = "Q2" }
        });

        _context.SaveChanges();
    }

    [TestMethod]
    public async Task Index_ReturnsViewWithQuestionnaires()
    {
        
        var controller = new QuestionnaireUtilisateurController(_context);

        
        var result = await controller.Index();

        
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as List<QuestionnaireStress>;
        Assert.IsNotNull(model);
        Assert.AreEqual(2, model.Count);
        Assert.AreEqual("Q1", model[0].Libelle);
        Assert.AreEqual("Q2", model[1].Libelle);
    }

    [TestMethod]
    public void Merci_ReturnsView()
    {
        
        var controller = new QuestionnaireUtilisateurController(_context);

        
        var result = controller.Merci();

        
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
    }

    [TestMethod]
    public async Task Soumettre_ValidEvenementIds_SavesReponseAndRedirects()
    {
        
        var userId = 42;

        if (!_context.Questionnaires.Any(q => q.Id == 1))
        {
            _context.Questionnaires.Add(new QuestionnaireStress { Id = 1, Libelle = "Stress 1" });
            _context.SaveChanges();
        }

        var controller = new QuestionnaireUtilisateurController(_context);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var evenementIds = new List<int> { 1 };

        
        var result = await controller.Soumettre(evenementIds);

        
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual("Merci", redirectResult.ActionName);

        var reponse = _context.ReponsesQuestionnaire.Include(r => r.ReponsesEvenement).FirstOrDefault(r => r.UtilisateurId == userId);
        Assert.IsNotNull(reponse);
        Assert.AreEqual(1, reponse.ReponsesEvenement.Count);
        Assert.AreEqual(1, reponse.ReponsesEvenement.First().QuestionnaireStressId);
    }



}
