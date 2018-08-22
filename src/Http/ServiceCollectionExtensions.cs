using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace Thor.Core.Http
{
    /// <summary>
    /// A bunch of convenient extensions methods for
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="TracingHttpMessageHandler"/> services to the
        /// service collection.
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
        public static IServiceCollection AddTracingHttpMessageHandler(
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

            services
                .TryAddEnumerable(ServiceDescriptor
                    .Singleton<IHttpMessageHandlerBuilderFilter,
                        TracingHttpMessageHandlerBuilderFilter>());

            return services
                .AddTracingCore(configuration)
                .AddHttpClient();
        }
    }
}