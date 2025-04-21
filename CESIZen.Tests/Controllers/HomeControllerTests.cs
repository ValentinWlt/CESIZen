using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using CesiZen.Controllers;
using Microsoft.AspNetCore.Mvc;
using CESIZen.Models;
using Moq;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace CESIZen.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Privacy_ReturnsViewResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object);

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "TestTraceId";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Error() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ErrorViewModel));

            var model = result.Model as ErrorViewModel;
            Assert.AreEqual("TestTraceId", model.RequestId);
        }

    }
}
