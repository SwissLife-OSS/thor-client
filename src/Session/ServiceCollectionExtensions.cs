using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Thor.Core.Session.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Session
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <c>ETW</c> in-process telemetry session services to the service collection.
        /// </summary>
        /// <param name="services">A <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configuration">A <see cref="IConfiguration"/> instance.</param>
        /// <returns>The provided <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddInProcessTelemetrySession(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services
                .AddOptions()
                .Configure<SessionConfiguration>(configuration.GetSection("Tracing"))
                .AddSingleton(p =>
                {
                    IOptions<SessionConfiguration> configAccessor = p.GetRequiredService<IOptions<SessionConfiguration>>();
                    IEnumerable<ITelemetryEventTransmitter> transmitters = p.GetServices<ITelemetryEventTransmitter>();
                    ITelemetrySession session = InProcessTelemetrySession.Create(configAccessor.Value);

                    foreach(ITelemetryEventTransmitter transmitter in transmitters)
                    {
                        session.Attach(transmitter);
                    }

                    return session;
                });
        }
    }
}