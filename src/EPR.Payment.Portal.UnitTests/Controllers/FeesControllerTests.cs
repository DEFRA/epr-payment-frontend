using AutoFixture;
using AutoFixture.Xunit2;
using EPR.Payment.Portal.Common.Dtos;
using EPR.Payment.Portal.Controllers;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    public class FeesControllerTests
    {
        private readonly IFixture _fixture;
        private readonly FeesController _controller;
        private readonly Mock<IFeesService> _feesServiceMock;

        public FeesControllerTests()
        {
            _fixture = new Fixture();
            _feesServiceMock = new Mock<IFeesService>();
            _controller = new FeesController(_feesServiceMock.Object);
        }

        [Theory, AutoData]
        public async Task GetFee_ReturnsViewWithViewModel(bool isLarge, string regulator)
        {
            var expectedViewModel = _fixture.Build<GetFeesResponseDto>().Create(); 
            _feesServiceMock.Setup(service => service.GetFee(isLarge, regulator)).ReturnsAsync(expectedViewModel);

            var result = await _controller.GetFee(isLarge, regulator);


            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            ViewResult? viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.NotNull(viewResult.ViewData.Model);

            // check model is expected type
            Assert.IsType<GetFeesResponseDto>(viewResult.ViewData.Model);

            // check view name
            Assert.Null(viewResult.ViewName);

            _feesServiceMock.Verify(service => service.GetFee(isLarge, regulator), Times.Once());
        }

        [Fact]
        public async Task GetFee_ReturnsBadRequest()
        {
            var result = await _controller.GetFee(false, null );

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            _feesServiceMock.Verify(service => service.GetFee(false, null ), Times.Never());
        }
    }
}
