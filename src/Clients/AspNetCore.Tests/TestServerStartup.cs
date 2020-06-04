using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thor.Core.Session;
using Thor.Core.Transmission.Abstractions;
using Thor.Core.Transmission.BlobStorage;

namespace Thor.Hosting.AspNetCore.Tests
{
    public class TestServerStartup
    {
        public TestServerStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddEmptyTelemetrySession(Configuration)
                .AddSingleton<IAttachmentTransmissionInitializer, AttachmentTransmissionInitializer>()
                .AddTracingMinimum(Configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                if (context.Request.Path == "/_health/liveness")
                {
                    throw new InvalidOperationException();
                }

                await context.Response.WriteAsync("foo");
            });
        }
    }
}
