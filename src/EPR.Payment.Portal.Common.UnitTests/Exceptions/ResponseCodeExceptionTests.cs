using EPR.Payment.Portal.Common.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Net;

namespace EPR.Payment.Portal.Common.UnitTests.Exceptions
{
    [TestClass]
    public class ResponseCodeExceptionTests
    {
        [TestMethod]
        public void Constructor_WithStatusCodeAndMessage_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var statusCode = HttpStatusCode.BadRequest;
            var message = "Bad request error";

            // Act
            var exception = new ResponseCodeException(statusCode, message);

            // Assert
            using(new AssertionScope())
            {
                exception.StatusCode.Should().Be(statusCode);
                exception.Message.Should().Be(message);
            }
        }

        [TestMethod]
        public void Constructor_WithStatusCode_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var statusCode = HttpStatusCode.NotFound;

            // Act
            var exception = new ResponseCodeException(statusCode);

            // Assert
            using(new AssertionScope())
            {
                exception.StatusCode.Should().Be(statusCode);
                exception.Should().BeOfType<ResponseCodeException>();
            }
        }
    }
}
