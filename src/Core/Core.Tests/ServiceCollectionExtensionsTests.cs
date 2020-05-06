using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Thor.Core.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        #region AddTracingCore

        [Fact(DisplayName = "AddTracingCore: Resolve configuration")]
        public void AddTracingCore()
        {
            // arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationBuilder builder = new ConfigurationBuilder();

            builder.AddInMemoryCollection();

            IConfiguration configuration = builder.Build();

            // act
            services.AddTracingCore(configuration);

            // assert
            ServiceProvider provider = services.BuildServiceProvider();

            Assert.IsType<TracingConfiguration>(provider.GetService<TracingConfiguration>());
        }

        #endregion
    }
}
