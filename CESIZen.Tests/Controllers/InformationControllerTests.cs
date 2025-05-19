using CESIZen.Controllers;
using CESIZen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CesiZen.Data;

namespace CESIZen.Tests.Controllers
{
    [TestClass]
    public class InformationControllerTests
    {
        private CesiZenDbContext _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CesiZenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new CesiZenDbContext(options);

            _context.Informations.AddRange(new List<Information>
            {
                new Information { Id = 1, Titre = "Info 1", Contenu = "Contenu 1", EstPublie = true, OrdreAffichage = 2 },
                new Information { Id = 2, Titre = "Info 2", Contenu = "Contenu 2", EstPublie = true, OrdreAffichage = 1 },
                new Information { Id = 3, Titre = "Non publiée", Contenu = "Contenu caché", EstPublie = false, OrdreAffichage = 3 }
            });

            _context.SaveChanges();
        }


        [TestMethod]
        public async Task Index_ReturnsOnlyPublishedOrderedInformations()
        {
            
            var controller = new InformationController(_context);

            
            var result = await controller.Index() as ViewResult;
            var model = result.Model as List<Information>;

            
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Info 2", model[0].Titre); 
            Assert.AreEqual("Info 1", model[1].Titre);
        }

        [TestMethod]
        public async Task Details_ValidId_ReturnsInformation()
        {
            
            var controller = new InformationController(_context);

            
            var result = await controller.Details(1) as ViewResult;
            var model = result.Model as Information;

            
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual("Info 1", model.Titre);
        }

        [TestMethod]
        public async Task Details_NullId_ReturnsNotFound()
        {
            
            var controller = new InformationController(_context);

            
            var result = await controller.Details(null);

            
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_NonExistentId_ReturnsNotFound()
        {
            
            var controller = new InformationController(_context);

            
            var result = await controller.Details(999);

            
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
