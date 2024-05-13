using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using EPR.Payment.Portal.Common.Dtos;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
using EPR.Payment.Portal.Services;
using Moq;
using Xunit;

namespace EPR.Payment.Portal.UnitTests.Services
{
    public class FeesServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHttpFeesService> _httpFeeServiceMock;
        private readonly FeesService _feesService;

        public FeesServiceTests()
        {
            _fixture = new Fixture();
            _mockMapper = new Mock<IMapper>();
            _httpFeeServiceMock = new Mock<IHttpFeesService>();

            _feesService = new FeesService(
                _mockMapper.Object, 
                _httpFeeServiceMock.Object);
        }


        [Theory, AutoData]
        public async Task GetFee_ReturnsCorrectViewModel(bool isLarge, string regulator)
        {
            // Arrange
            var expectedResponse = _fixture.Build<GetFeesResponseDto>().Create();
            _httpFeeServiceMock.Setup(s => s.GetFee(isLarge, regulator)).ReturnsAsync(expectedResponse);

            // Act
            var response = await _feesService.GetFee(isLarge, regulator);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<GetFeesResponseDto>(response);
        }
    }
}
