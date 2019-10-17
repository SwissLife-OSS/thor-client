using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Thor.Analyzer;
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

        [Fact]
        public async Task HotChocolateActivity_Exist()
        {
            // arrange
            HotChocolateActivity hotChocolateActivity = null;
            TestServer server = CreateTestServer(ctx =>
                hotChocolateActivity = ctx.Features.Get<HotChocolateActivity>());
            var request = new
            {
                query = @"{ customProperty }"
            };

            // act
            HttpResponseMessage message =
                await server.SendRequestAsync(request);

            // assert
            Assert.Equal(HttpStatusCode.OK, message.StatusCode);
            Assert.NotNull(hotChocolateActivity);
        }

        [Fact]
        public async Task HttpAndHotChocolateEvents_AreFired()
        {
            // arrange
            TestServer server = CreateTestServer();
            var request = new
            {
                query = @"{ customProperty }"
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

        [Fact]
        public void Inspect_HotChocolateActivityEventSource()
        {
            // arrange
            var analyzer = new EventSourceAnalyzer();

            // act
            Report report = analyzer.Inspect(
                HotChocolateActivityEventSource.Log);

            // assert
            Assert.False(report.HasErrors);
        }

        private TestServer CreateTestServer(
            Action<HttpContext> onRequestFinish = null,
            string path = null)
        {
            return TestServerFactory.Create(
                new QueryMiddlewareOptions
                {
                    Path = path ?? "/",
                },
                CreateConfiguration(),
                onRequestFinish);
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
