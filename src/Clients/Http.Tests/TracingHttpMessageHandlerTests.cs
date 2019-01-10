using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thor.Core.Abstractions;
using Xunit;

namespace Thor.Core.Http.Tests
{
    public class TracingHttpMessageHandlerTests
    {
        [Fact(DisplayName = "GetAsync: Should set a HTTP header with name 'Thor-ActivityId'")]
        public async Task GetAsync()
        {
            // arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationBuilder builder = new ConfigurationBuilder();

            builder.AddInMemoryCollection();

            IConfiguration configuration = builder.Build();

            services.AddTracingHttpMessageHandler(configuration);

            ServiceProvider provider = services.BuildServiceProvider();
            IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
            HttpClient client = factory.CreateClient();

            // act
            HttpResponseMessage response = await client.GetAsync("http://test.ch").ConfigureAwait(false);

            // assert
            Assert.NotNull(response);
            Assert.True(response.RequestMessage.Headers.Contains(MessageHeaderKeys.ActivityId));
        }
    }
}