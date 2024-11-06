using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Sessions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.Payment.Portal.UnitTests.Sessions
{
    [TestClass]
    public class SessionRequestCultureProviderTests
    {
        private Mock<HttpContext> _httpContextMock = null!;
        private Dictionary<string, byte[]> _sessionStorage = null!;
        private SessionRequestCultureProvider _provider = null!;

        [TestInitialize]
        public void Setup()
        {
            _httpContextMock = new Mock<HttpContext>();
            _sessionStorage = new Dictionary<string, byte[]>();

            // Custom session mock that uses _sessionStorage to simulate session behavior
            var sessionMock = new Mock<ISession>();

            sessionMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]?>.IsAny))
                .Returns((string key, out byte[]? value) =>
                {
                    if (_sessionStorage.TryGetValue(key, out var result))
                    {
                        value = result;
                        return true;
                    }
                    value = null;
                    return false;
                });

            sessionMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => _sessionStorage[key] = value);

            _httpContextMock.Setup(x => x.Session).Returns(sessionMock.Object);
            _provider = new SessionRequestCultureProvider();
        }

        [TestMethod]
        public async Task DetermineProviderCultureResult_WhenSessionDoesNotContainCulture_ReturnsEnglish()
        {
            // Act
            var result = await _provider.DetermineProviderCultureResult(_httpContextMock.Object);

            // Assert
            using(new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.Cultures.First().Value.Should().Be(Language.English);
            }

        }

        [TestMethod]
        public async Task DetermineProviderCultureResult_WhenSessionContainsCulture_ReturnsCultureFromSession()
        {
            // Arrange
            const string expectedCulture = "cy";
            _sessionStorage[Language.SessionLanguageKey] = System.Text.Encoding.UTF8.GetBytes(expectedCulture);

            // Act
            var result = await _provider.DetermineProviderCultureResult(_httpContextMock.Object);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.Cultures.First().Value.Should().Be(expectedCulture);
            }
        }
    }
}
