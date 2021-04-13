using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A transmission sender for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public class EventHubTransmissionSender
        : ITransmissionSender<EventDataBatch>
    {
        private readonly EventHubProducerClient _client;
        private readonly ErrorLogger<EventHubTransmissionSender> _errorLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmissionSender"/> class.
        /// </summary>
        /// <param name="client">A <c>Azure</c> <c>EventHub</c> client instance.</param>
        /// <param name="logger">A logger instance</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="client"/> must not be <c>null</c>.
        /// </exception>
        public EventHubTransmissionSender(
            EventHubProducerClient client,
            ILogger<EventHubTransmissionSender> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _errorLogger = new ErrorLogger<EventHubTransmissionSender>(logger);
        }

        /// <inheritdoc />
        public async Task SendAsync(IAsyncEnumerable<EventDataBatch> batches, CancellationToken cancellationToken)
        {
            if (batches == null)
            {
                throw new ArgumentNullException(nameof(batches));
            }

            await foreach(EventDataBatch batch in batches.WithCancellation(cancellationToken))
            {
                try
                {
                    await _client.SendAsync(batch, cancellationToken);
                }
                catch (Exception ex)
                {
                    _errorLogger.Log(ex);
                }
            }
        }
    }
}
