using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.EventHub.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        #region AddEventHubTelemetryEventTransmission

        [Fact(DisplayName = "AddEventHubTelemetryEventTransmission: Should throw an argument null exception for services")]
        public void AddEventHubTelemetryEventTransmission_ServicesNull()
        {
            // arrange
            IServiceCollection services = null;
            IConfiguration configuration = new Mock<IConfiguration>().Object;

            // act
            Action verify = () => services.AddEventHubTelemetryEventTransmission(configuration);

            // arrange
            Assert.Throws<ArgumentNullException>("services", verify);
        }

        [Fact(DisplayName = "AddEventHubTelemetryEventTransmission: Should throw an argument null exception for configuration")]
        public void AddEventHubTelemetryEventTransmission_ConfigurationNull()
        {
            // arrange
            IServiceCollection services = new Mock<IServiceCollection>().Object;
            IConfiguration configuration = null;

            // act
            Action verify = () => services.AddEventHubTelemetryEventTransmission(configuration);

            // arrange
            Assert.Throws<ArgumentNullException>("configuration", verify);
        }

        [Fact(DisplayName = "AddEventHubTelemetryEventTransmission: Resolve telemetry transmitter")]
        public void AddEventHubTelemetryEventTransmission()
        {
            // arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationBuilder builder = new ConfigurationBuilder();
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                {"Tracing:EventHub:ConnectionString", Constants.FakeConnectionString},
                {"Tracing:EventHub:TransportType", Constants.TransportType},
                {"Tracing:Events:Buffer:Size", "500"},
                {"Tracing:Events:BufferDequeueBatchSize", "50"}
            };

            builder.AddInMemoryCollection(data);

            IConfiguration configuration = builder.Build();

            // act
            services.AddEventHubTelemetryEventTransmission(configuration);

            // assert
            ServiceProvider provider = services.BuildServiceProvider();

            Assert.IsType<EventHubTransmitter>(provider.GetService<ITelemetryEventTransmitter>());
        }

        #endregion
    }
}
