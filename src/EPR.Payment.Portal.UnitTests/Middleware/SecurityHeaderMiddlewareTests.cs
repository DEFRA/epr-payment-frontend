namespace EPR.Payment.Portal.UnitTests.Middleware;

using AutoFixture;
using Castle.Components.DictionaryAdapter.Xml;
using Constants;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Exceptions;
using EPR.Payment.Portal.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Configuration;

[TestClass]
public class SecurityHeaderMiddlewareTests
{
    private Mock<RequestDelegate> _mockRequestDelegate = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;
    private SecurityHeaderMiddleware _middleware = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void SetUp()
    {
        _mockRequestDelegate = new Mock<RequestDelegate>();
        _mockConfiguration = new Mock<IConfiguration>();
        _middleware = new SecurityHeaderMiddleware(_mockRequestDelegate.Object);
        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task Invoke_WithMissingConfiguration_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        Func<Task> action = () => Task.Run(() => _middleware.Invoke(context, new Mock<IConfiguration>().Object));

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("AzureAdB2C:Instance is not configured.");
    }

    [TestMethod]
    public async Task Invoke_ShouldAddSecurityHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _mockConfiguration.Setup(config => config["AzureAdB2C:Instance"])
                 .Returns("https://mocked-instance.b2clogin.com/");

        // Act
        await _middleware.Invoke(context, _mockConfiguration.Object);
        
        // Assert
        context.Response.Headers.Should().ContainKey("Content-Security-Policy");
        context.Response.Headers.Should().ContainKey("Cross-Origin-Embedder-Policy");
        context.Response.Headers.Should().ContainKey("Cross-Origin-Opener-Policy");
        context.Response.Headers.Should().ContainKey("Cross-Origin-Resource-Policy");
        context.Response.Headers.Should().ContainKey("Permissions-Policy");
        context.Response.Headers.Should().ContainKey("Referrer-Policy");
        context.Response.Headers.Should().ContainKey("X-Content-Type-Options");
        context.Response.Headers.Should().ContainKey("X-Frame-Options");
        context.Response.Headers.Should().ContainKey("X-Permitted-Cross-Domain-Policies");
        context.Response.Headers.Should().ContainKey("X-Robots-Tag");
    }

    [TestMethod]
    public async Task Invoke_ShouldAddScriptNonceToItems()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _mockConfiguration.Setup(config => config["AzureAdB2C:Instance"])
                 .Returns("https://mocked-instance.b2clogin.com/");

        // Act
        await _middleware.Invoke(context, _mockConfiguration.Object);

        // Assert
        context.Items.Should().ContainKey(ContextKeys.ScriptNonceKey);
    }

    [TestMethod]
    public async Task Invoke_ShouldCallNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _mockConfiguration.Setup(config => config["AzureAdB2C:Instance"])
                 .Returns("https://mocked-instance.b2clogin.com/");

        // Act
        await _middleware.Invoke(context, _mockConfiguration.Object);

        // Assert
        _mockRequestDelegate.Verify(d => d(It.IsAny<HttpContext>()), Times.Once);
    }
}