using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Payments;
using EPR.Payment.Portal.Common.RESTServices.Payments.Interfaces;
using EPR.Payment.Portal.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Diagnostics;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;

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

            // Mock and add required services
            _services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _services.AddHttpClient(); // Register the default HttpClientFactory

            // Mock ITokenAcquisition and register it
            Mock<ITokenAcquisition> tokenAcquisitionMock = new Mock<Microsoft.Identity.Web.ITokenAcquisition>();
            _services.AddSingleton(tokenAcquisitionMock.Object);

            // Mock IFeatureManager and register it
            Mock<IFeatureManager> featureManagerMock = new Mock<Microsoft.FeatureManagement.IFeatureManager>();
            _services.AddSingleton(featureManagerMock.Object);

            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
        }

        [TestMethod]
        public void AddPortalDependencies_PaymentFacade_RegistersServicesCorrectly()
        {
            // Arrange
            Dictionary<string, string> configurationData = new Dictionary<string, string>
            {
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.Url)}", "https://payment.facade" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.EndPointName)}", "payment" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.HttpClientName)}", "HttpClient" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.DownstreamScope)}", "scope_value" }, // Add DownstreamScope here
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeServiceV2)}:{nameof(FacadeServiceV2.Url)}", "https://payment.facadev2" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeServiceV2)}:{nameof(FacadeServiceV2.EndPointName)}", "payment" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeServiceV2)}:{nameof(FacadeServiceV2.HttpClientName)}", "HttpClient" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeServiceV2)}:{nameof(FacadeServiceV2.DownstreamScope)}", "scope_value" } // Add DownstreamScope here
            };

            IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData!)
                .Build();

            // Act
            _services?.AddPortalDependencies(configurationBuilder);
            ServiceProvider? serviceProvider = _services?.BuildServiceProvider();
            IHttpPaymentFacade? httpPaymentFacade = serviceProvider?.GetService<IHttpPaymentFacade>();
            IHttpPaymentFacadeV2? httpPaymentFacadeV2 = serviceProvider?.GetService<IHttpPaymentFacadeV2>();

            // Assert
            using (new FluentAssertions.Execution.AssertionScope())
            {
                httpPaymentFacade.Should().NotBeNull();
                httpPaymentFacade.Should().BeOfType<HttpPaymentFacade>();
            }
            using (new FluentAssertions.Execution.AssertionScope())
            {
                httpPaymentFacadeV2.Should().NotBeNull();
                httpPaymentFacadeV2.Should().BeOfType<HttpPaymentFacadeV2>();
            }
        }

        [TestMethod]
        public void AddPortalDependencies_PaymentFacadeHealthCheckService_RegistersServicesCorrectly()
        {
            // Arrange
            Dictionary<string, string> configurationData = new Dictionary<string, string>
            {
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.Url)}", "https://healthcheck" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.EndPointName)}", "payment" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.DownstreamScope)}", "scope_value" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeServiceV2)}:{nameof(FacadeServiceV2.Url)}", "https://payment.facadev2" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeServiceV2)}:{nameof(FacadeServiceV2.EndPointName)}", "payment" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeServiceV2)}:{nameof(FacadeServiceV2.HttpClientName)}", "HttpClient" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeServiceV2)}:{nameof(FacadeServiceV2.DownstreamScope)}", "scope_value" }
            };

            IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData!)
                .Build();

            // Act
            _services?.AddPortalDependencies(configurationBuilder);
            ServiceProvider? serviceProvider = _services?.BuildServiceProvider();
            IHttpPaymentFacadeHealthCheckService? httpPaymentFacade = serviceProvider?.GetService<IHttpPaymentFacadeHealthCheckService>();

            // Assert
            using (new FluentAssertions.Execution.AssertionScope())
            {
                httpPaymentFacade.Should().NotBeNull();
                httpPaymentFacade.Should().BeOfType<HttpPaymentFacadeHealthCheckService>();
            }
        }

        [TestMethod]
        public void AddPortalDependencies_WithMissingUrlConfiguration_ThrowsInvalidOperationException()
        {
            // Arrange
            Dictionary<string, string> configurationData = new Dictionary<string, string>
            {
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.Url)}", null! },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.EndPointName)}", "payment" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.HttpClientName)}", "HttpClient" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.DownstreamScope)}", "scope_value" } // Add DownstreamScope here
            };

            IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
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
            Dictionary<string, string> configurationData = new Dictionary<string, string>
            {
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.Url)}", "https://payment.facade" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.EndPointName)}", null! },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.HttpClientName)}", "HttpClient" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.DownstreamScope)}", "scope_value" } // Add DownstreamScope here
            };

            IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData!)
                .Build();

            // Act
            Action act = () => _services?.AddPortalDependencies(configurationBuilder).BuildServiceProvider();

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("FacadeService EndPointName configuration is missing.");
        }

        [TestMethod]
        public void AddPortalDependencies_WithMissingDownstreamScopeConfiguration_ThrowsInvalidOperationException()
        {
            // Arrange
            Dictionary<string, string> configurationData = new Dictionary<string, string>
            {
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.Url)}", "https://payment.facade" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.EndPointName)}", "payment" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.HttpClientName)}", "HttpClient" },
                { $"{FacadeConfiguration.SectionName}:{nameof(FacadeConfiguration.FacadeService)}:{nameof(FacadeService.DownstreamScope)}", null! } // DownstreamScope missing
            };

            IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData!)
                .Build();

            // Act
            Action act = () => _services?.AddPortalDependencies(configurationBuilder).BuildServiceProvider();

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("FacadeService DownstreamScope configuration is missing.");
        }

    }
}
