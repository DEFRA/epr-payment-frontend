using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class FeesControllerTests
    {
        private readonly FeesController _controller;
        private readonly Mock<IFeesService> _feesServiceMock;

        public FeesControllerTests()
        {
            _feesServiceMock = new Mock<IFeesService>();
            _controller = new FeesController(_feesServiceMock.Object);
        }

        [TestMethod]
        public async Task GetFee_ReturnsViewWithViewModel()
        {
            var expectedViewModel = new GetFeesResponseViewModel { Large = true, Regulator = "regulator", Amount = 199, EffectiveFrom = DateTime.Now.AddDays(-1), EffectiveTo = DateTime.Now.AddDays(10) };
            _feesServiceMock.Setup(service => service.GetFee(It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(expectedViewModel);

            var result = await _controller.GetFee(true, "regulator");


            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult? viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewData.Model);

            // check model is expected type
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(GetFeesResponseViewModel));

            // check view name
            Assert.IsNull(viewResult.ViewName);

            _feesServiceMock.Verify(service => service.GetFee(true, "regulator"), Times.Once());
        }

        [TestMethod]
        public async Task GetFee_ReturnsBadRequest()
        {
            var result = await _controller.GetFee(false, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            _feesServiceMock.Verify(service => service.GetFee(false, null), Times.Never());
        }
    }
}
