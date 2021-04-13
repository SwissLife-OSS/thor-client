using System;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddEventHubTelemetryEventTransmission(
            this IServiceCollection services,
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

            EventHubConfiguration eventHubConfiguration = configuration
                .GetSection("Tracing")
                .GetSection("EventHub")
                .Get<EventHubConfiguration>();

            EventsOptions eventsOptions = configuration
                .GetSection("Tracing")
                .GetSection("Events")
                .Get<EventsOptions>() ?? new EventsOptions();

            return services
                .AddTracingCore(configuration)
                .AddSingleton(eventHubConfiguration)
                .AddSingleton(eventsOptions)
                .AddSingleton(eventsOptions.Buffer)
                .AddSingleton(p =>
                {
                    var connection = new EventHubConnection(eventHubConfiguration.ConnectionString);
                    EventHubProducerClientOptions clientOptions = CreateEventHubClientOptions(eventHubConfiguration);
                    return new EventHubProducerClient(connection, clientOptions);
                })
                .AddSingleton<IMemoryBuffer<EventData>>(sp => new MemoryBuffer<EventData>(eventsOptions.Buffer))
                .AddSingleton<ITransmissionBuffer<EventData, EventDataBatch>, EventHubTransmissionBuffer>()
                .AddSingleton<ITransmissionSender<EventDataBatch>, EventHubTransmissionSender>()
                .AddSingleton<ITransmissionStorage<EventData>>(p =>
                {
                    TracingConfiguration tracingConfiguration = p.GetRequiredService<TracingConfiguration>();
                    return new EventHubTransmissionStorage(tracingConfiguration.GetEventsStoragePath());
                })
                .AddSingleton<ITelemetryEventTransmitter, EventHubTransmitter>();
        }

        private static EventHubProducerClientOptions CreateEventHubClientOptions(
            EventHubConfiguration configuration)
        {
            EventHubsTransportType transportType = EventHubsTransportType.AmqpTcp;
            if (!string.IsNullOrEmpty(configuration.TransportType))
            {
                Enum.TryParse(configuration.TransportType, out transportType);
            }

            return new EventHubProducerClientOptions
            {
                ConnectionOptions = new EventHubConnectionOptions
                {
                    TransportType = transportType
                }
            };
        }
    }
}
