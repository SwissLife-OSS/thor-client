using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thor.Core.Session.Abstractions;
using Xunit;

namespace Thor.Core.Session.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        #region AddInProcessTelemetrySession
        
        [Fact(DisplayName = "AddInProcessTelemetrySession: Resolve telemetry session")]
        public void AddInProcessTelemetrySession()
        {
            // arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationBuilder builder = new ConfigurationBuilder();
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                {"Tracing:ApplicationId", "5"},
                {"Tracing:Level", "Warning"}
            };

            builder.AddInMemoryCollection(data);

            IConfiguration configuration = builder.Build();

            // act
            services.AddInProcessTelemetrySession(configuration);

            // assert
            ServiceProvider provider = services.BuildServiceProvider();

            Assert.IsType<InProcessTelemetrySession>(provider.GetService<ITelemetrySession>());
        }

        #endregion
    }
}