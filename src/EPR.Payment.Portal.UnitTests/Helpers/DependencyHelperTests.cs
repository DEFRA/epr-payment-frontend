using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Payments;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EPR.Payment.Portal.UnitTests.Helpers
{
    [TestClass]
    public class DependencyHelperTests
    {
        private IServiceCollection? _services = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _services = new ServiceCollection();

            // Add required services
            _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _services.AddHttpClient(); // Register the default HttpClientFactory

            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
        }

        [TestMethod]
        public void AddPortalDependencies_RegistersServicesCorrectly()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.Url)}", "https://payment.facade" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.EndPointName)}", "payment" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.HttpClientName)}", "HttpClient" }
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData!)
                .Build();

            // Act
            _services?.AddPortalDependencies(configurationBuilder);
            var serviceProvider = _services?.BuildServiceProvider();

            // Assert
            using (new FluentAssertions.Execution.AssertionScope())
            {
                var httpPaymentFacade = serviceProvider?.GetService<IHttpPaymentFacade>();
                httpPaymentFacade.Should().NotBeNull();
                httpPaymentFacade.Should().BeOfType<HttpPaymentFacade>();
            }
        }

        [TestMethod]
        public void AddPortalDependencies_WithMissingUrlConfiguration_ThrowsInvalidOperationException()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.Url)}", null! },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.EndPointName)}", "payment" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.HttpClientName)}", "HttpClient" }
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData!)
                .Build();

            // Act
            Action act = () => _services?.AddPortalDependencies(configurationBuilder).BuildServiceProvider();

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("FacadeService Url configuration is missing.");
        }

        [TestMethod]
        public void AddPortalDependencies_WithMissingEndPointNameConfiguration_ThrowsInvalidOperationException()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.Url)}", "https://payment.facade" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.EndPointName)}", null! },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.HttpClientName)}", "HttpClient" }
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData!)
                .Build();

            // Act
            Action act = () => _services?.AddPortalDependencies(configurationBuilder).BuildServiceProvider();

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("FacadeService EndPointName configuration is missing.");
        }
    }
}
