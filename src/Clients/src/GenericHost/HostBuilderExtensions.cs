using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Thor.Hosting.GenericHost
{
    /// <summary>
    /// Extension methods for Generic Host Builder
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configure tracing and build the host.
        /// </summary>
        public static void RunWithTracing(
            this IHostBuilder hostBuilder)
        {
            IHost host = hostBuilder
                .ConfigureServices((context, builder) =>
                    builder.AddTracing(context.Configuration))
                .Build();

            HostTelemetryInitializer hostTelemetryInitializer = host.Services
                .GetService<HostTelemetryInitializer>();
            
            hostTelemetryInitializer?.Initialize();

            host.RunSafe();
        }
    }
}
