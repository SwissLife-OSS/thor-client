using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Thor.Core.Session.Abstractions;
using Xunit;
using CoreEventSources = Thor.Core.Abstractions.EventSourceNames;

namespace Thor.Extensions.HotChocolate.Tests
{
    public class HotChocolateDiagnosticListenerTests
        : IClassFixture<TestServerFactory>
    {
        public HotChocolateDiagnosticListenerTests(
            TestServerFactory testServerFactory)
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

            // assert
            Assert.Equal(HttpStatusCode.OK, message.StatusCode);
            var hotChocolateActivity = Context
                .Features.Get<HotChocolateActivity>();
            Assert.NotNull(hotChocolateActivity);
        }

        [Fact]
        public async Task HttpAndHotChocolateEvents_AreFired()
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
            await Task.Delay(100);

            // assert
            Assert.Equal(HttpStatusCode.OK, message.StatusCode);
            var transmitter = ProbeTransmitter.Instance;
            Assert.True(transmitter.Contains(CoreEventSources.RequestActivity, "Start"));
            Assert.True(transmitter.Contains(EventSourceNames.HotChocolate, "Start"));
            Assert.True(transmitter.Contains(EventSourceNames.HotChocolate, "Stop"));
            Assert.True(transmitter.Contains(CoreEventSources.RequestActivity, "Stop"));
        }

        private TestServer CreateTestServer(string path = null)
        {
            return TestServerFactory.Create(
                new QueryMiddlewareOptions
                {
                    Path = path ?? "/",
                    OnCreateRequest = (context, builder, ct) =>
                    {
                        Context = context;
                        return Task.CompletedTask;
                    }
                }, CreateConfiguration());
        }

        private IConfiguration CreateConfiguration()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                {"Tracing:ApplicationId", "5"},
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(data)
                .Build();
        }

    }
}