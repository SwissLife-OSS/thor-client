using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Thor.HotChocolate.Tests
{
    public class HotChocolateDiagnosticListenerTests
        : IClassFixture<TestServerFactory>
    {
        public HotChocolateDiagnosticListenerTests(TestServerFactory testServerFactory)
        {
            TestServerFactory = testServerFactory;
        }

        private TestServerFactory TestServerFactory { get; set; }
        private HttpContext Context { get; set; }

        [Fact]
        public async Task HotChocolateActivity_Exist()
        {
            // arrange
            TestServer server = CreateTestServer();
            var request = new
            {
                Query = @"{ customProperty }"
            };

            // act
            HttpResponseMessage message =
                await server.SendRequestAsync(request);

            Assert.Equal(HttpStatusCode.OK, message.StatusCode);

            // TODO: HttpContext check activity
        }

        private TestServer CreateTestServer(string path = null)
        {
            return TestServerFactory.Create(
                new QueryMiddlewareOptions
                {
                    Path = path ?? "/",
                    OnCreateRequest = (context, request, properties, ct) =>
                    {
                        Context = context;
                        properties["foo"] = "bar";
                        return Task.CompletedTask;
                    }
                });
        }

    }
}