using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.AspNetCore.Hosting;
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

        public TestServer Create(
            QueryMiddlewareOptions options,
            IConfiguration configuration)
        {
            IWebHostBuilder builder = new WebHostBuilder()
                .Configure(app => app.UseGraphQL(options))
                .ConfigureServices(services =>
                {
                    services
                        .AddSingleton<IStartupFilter, TestStartupFilter>()
                        .AddGraphQL(
                            Schema.Create(c =>
                                c.RegisterQueryType<QueryType>())
                                .MakeExecutable(b =>
                                    b.UseDefaultPipeline(
                                        new QueryExecutionOptions
                                        {
                                            TracingPreference = TracingPreference.Always
                                        })
                                        .AddHotCocolateTracing()))
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
            descriptor.Field("customProperty")
                .Resolver(ctx => "foo");
        }
    }
}
