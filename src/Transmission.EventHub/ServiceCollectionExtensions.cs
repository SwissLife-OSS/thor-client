using System;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <c>EventHub</c> telemetry event transmission services to the service collection.
        /// </summary>
        /// <param name="services">A <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configuration">A <see cref="IConfiguration"/> instance.</param>
        /// <returns>The provided <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddEventHubTelemetryEventTransmission(this IServiceCollection services,
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
                .Configure<EventHubConfiguration>(configuration.GetSection("Tracing").GetSection("EventHub"))
                .AddSingleton(p =>
                {
                    IOptions<EventHubConfiguration> configAccessor = p.GetRequiredService<IOptions<EventHubConfiguration>>();

                    return EventHubClient.CreateFromConnectionString(configAccessor.Value.ConnectionString);
                })
                .AddSingleton<ITransmissionBuffer<EventData>, EventHubTransmissionBuffer>()
                .AddSingleton<ITransmissionSender<EventData>, EventHubTransmissionSender>()
                .AddSingleton<ITelemetryTransmitter, EventHubTransmitter>();
        }
    }
}