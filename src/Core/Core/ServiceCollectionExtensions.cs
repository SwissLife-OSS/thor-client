using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thor.Core.Abstractions;

namespace Thor.Core
{
    /// <summary>
    /// A bunch of convenient extensions methods for
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds tracing core services to the service collection.
        /// </summary>
        /// <param name="services">
        /// A <see cref="IServiceCollection"/> instance.
        /// </param>
        /// <param name="configuration">
        /// A <see cref="IConfiguration"/> instance.
        /// </param>
        /// <returns>
        /// The provided <see cref="IServiceCollection"/> instance.
        /// </returns>
        public static IServiceCollection AddTracingCore(
            this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            TracingConfiguration tracingConfiguration = configuration
                .GetSection("Tracing")
                .Get<TracingConfiguration>();

            return services
                .AddSingleton<IProvidersDescriptor, CoreProvidersDescriptor>()
                .AddSingleton<IJobHealthCheck, JobHealthCheck>()
                .AddSingleton(tracingConfiguration);
        }
    }
}
