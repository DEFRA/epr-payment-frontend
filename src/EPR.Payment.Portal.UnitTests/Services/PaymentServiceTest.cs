using AutoFixture.AutoMoq;
using AutoFixture;
using AutoMapper;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
using EPR.Payment.Portal.Services;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Logging;

namespace EPR.Payment.Portal.UnitTests.Services
{
    [TestClass]
    public class PaymentServiceTest
    {
        private IFixture? _fixture;
        private Mock<IHttpPaymentsService>? _httpPaymentsServiceMock;
        private Mock<ILogger<PaymentsService>>? _loggerMock;
        private PaymentsService? _service;
        private IMapper? _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            var throwingRecursionBehaviors = _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList();
            foreach (var behavior in throwingRecursionBehaviors)
            {
                _fixture.Behaviors.Remove(behavior);
            }
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _httpPaymentsServiceMock = _fixture.Freeze<Mock<IHttpPaymentsService>>();
            _loggerMock = _fixture.Freeze<Mock<ILogger<PaymentsService>>>();

            _service = new PaymentsService(
                _mapper, 
                _httpPaymentsServiceMock.Object
            );
        }
    }
}