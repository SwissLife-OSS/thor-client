using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thor.Core.Session;
using Thor.Core.Transmission.Abstractions;
using Thor.Core.Transmission.BlobStorage;
using Thor.Extensions.Http;
using Thor.Hosting.AspNetCore;

namespace Thor.Extensions.HotChocolate.Tests
{
    public class TestServerFactory
        : IDisposable
    {
        private readonly List<TestServer> _instances = new List<TestServer>();

        internal TestServer Create(
            IConfiguration configuration,
            Action<IRequestContext> onRequestFinish = null,
            string path = null)
        {
            IWebHostBuilder builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGraphQL(path ?? "/");
                    });
                })
                .ConfigureServices(services =>
                {
                    services
                        .AddSingleton<IStartupFilter, TestStartupFilter>()
                        .AddRouting()
                        .AddGraphQLServer()
                        .AddQueryType<QueryType>()
                        .AddThorLogging()
                        .UseRequest(next => async context =>
                        {
                            await next(context);
                            onRequestFinish?.Invoke(context);
                        })
                        .UseDefaultPipeline()
                        .Services
                        .AddSingleton<IAttachmentTransmissionInitializer>(
                            provider =>
                                new AttachmentTransmissionInitializer(
                                    Enumerable.Empty<ITelemetryAttachmentTransmitter>()))
                        .AddTracingHttpMessageHandler(configuration)
                        .AddInProcessTelemetrySession(configuration)
                        .AddTracingMinimum(configuration);
                });

            var server = new TestServer(builder);

            _instances.Add(server);
            return server;
        }

        public void Dispose()
        {
            foreach (TestServer testServer in _instances)
            {
                testServer.Dispose();
            }
        }
    }

    internal class QueryType
        : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name("Query");
            descriptor.Field("customProperty")
                .Resolver(ctx => "foo");
        }
    }
}
