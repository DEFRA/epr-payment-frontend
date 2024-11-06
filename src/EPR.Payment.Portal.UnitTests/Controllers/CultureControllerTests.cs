﻿using AutoFixture;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Controllers.Culture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.Payment.Portal.UnitTests.Controllers
{
    [TestClass]
    public class CultureControllerTests
    {
        private const string ReturnUrl = "returnUrl";
        private const string CultureEn = "en";

        private Mock<IResponseCookies> _responseCookiesMock = null!;
        private Mock<ISession> _sessionMock = null!;
        private Mock<HttpContext> _httpContextMock = null!;
        private CultureController _systemUnderTest = null!;

        [TestInitialize]
        public void Setup()
        {
            _responseCookiesMock = new Mock<IResponseCookies>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();

            // Set up the controller with mock HttpContext
            _systemUnderTest = new CultureController
            {
                ControllerContext = { HttpContext = _httpContextMock.Object }
            };

            _httpContextMock.Setup(x => x.Session).Returns(_sessionMock.Object);
        }

        [TestMethod]
        public void CultureController_UpdateCulture_RedirectsToReturnUrlWithCulture()
        {
            // Arrange
            _httpContextMock
                .Setup(x => x.Response.Cookies)
                .Returns(_responseCookiesMock.Object);

            var cultureBytes = Encoding.UTF8.GetBytes(CultureEn);

            // Act
            var result = _systemUnderTest.UpdateCulture(CultureEn, ReturnUrl);

            // Assert
            result.Url.Should().Be(ReturnUrl);
            _sessionMock.Verify(x => x.Set(Language.SessionLanguageKey, cultureBytes), Times.Once);
        }
    }
}
