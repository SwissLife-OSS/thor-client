using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Xunit;

namespace Thor.Extensions.Http.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        #region AddTracingHttpMessageHandler

        [Fact(DisplayName = "AddTracingHttpMessageHandler: Resolve configuration")]
        public void AddTracingHttpMessageHandler()
        {
            // arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationBuilder builder = new ConfigurationBuilder();

            builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Tracing:ApplicationId", "999")
            });

            IConfiguration configuration = builder.Build();

            // act
            services.AddTracingHttpMessageHandler(configuration);

            // assert
            ServiceProvider provider = services.BuildServiceProvider();
            IEnumerable<IHttpMessageHandlerBuilderFilter> filters = provider
                .GetServices<IHttpMessageHandlerBuilderFilter>();

            Assert.Collection(filters,
                f => Assert.IsType<TracingHttpMessageHandlerBuilderFilter>(f),
                f => Assert.IsNotType<TracingHttpMessageHandlerBuilderFilter>(f));
        }

        #endregion
    }
}
