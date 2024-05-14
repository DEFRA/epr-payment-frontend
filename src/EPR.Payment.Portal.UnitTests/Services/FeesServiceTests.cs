using AutoMapper;
using EPR.Payment.Portal.Common.Dtos;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
using EPR.Payment.Portal.Services;
using Moq;

namespace EPR.Payment.Portal.UnitTests.Services
{
    [TestClass]
    public class FeesServiceTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHttpFeesService> _httpFeeServiceMock;
        private readonly FeesService _feesService;

        public FeesServiceTests()
        {
            _mockMapper = new Mock<IMapper>();
            _httpFeeServiceMock = new Mock<IHttpFeesService>();

            _feesService = new FeesService(
                _mockMapper.Object,
                _httpFeeServiceMock.Object);
        }


        [TestMethod]
        public async Task GetFee_ReturnsCorrectViewModel()
        {
            // Arrange
            var expectedResponse = new GetFeesResponseDto { Large = true, Regulator = "regulator", Amount = 199, EffectiveFrom = DateTime.Now.AddDays(-1), EffectiveTo = DateTime.Now.AddDays(10) };
            _httpFeeServiceMock.Setup(s => s.GetFee(It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

            // Act
            var response = await _feesService.GetFee(true, "regulator");

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(GetFeesResponseDto));
        }
    }
}
