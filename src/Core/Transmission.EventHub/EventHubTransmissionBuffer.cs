using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A transmission buffer for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public class EventHubTransmissionBuffer
        : ITransmissionBuffer<EventData>
    {
        private static readonly int MaxBufferSize = 1000;
        private readonly Channel<EventData> _input = Channel.CreateBounded<EventData>(MaxBufferSize);
        private readonly Channel<EventData[]> _output = Channel.CreateUnbounded<EventData[]>();
        private readonly EventHubClient _client;
        private EventData _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmissionBuffer"/> class.
        /// </summary>
        /// <param name="client">A <c>Azure</c> <c>EventHub</c> client instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="client"/> must not be <c>null</c>.
        /// </exception>
        public EventHubTransmissionBuffer(EventHubClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            Task.Factory.StartNew(
                StartAsyncProcessing,
                default,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<EventData[]> Dequeue(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (await _output.Reader.WaitToReadAsync(cancellationToken))
            {
                yield return await _output.Reader.ReadAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task Enqueue(
            IAsyncEnumerable<EventData> batch,
            CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            await foreach (EventData data in batch.WithCancellation(cancellationToken))
            {
                await Enqueue(data, cancellationToken);
            }
        }

        private async Task Enqueue(
            EventData data,
            CancellationToken cancellationToken)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            await _input.Writer.WaitToWriteAsync(cancellationToken);
            await _input.Writer.WriteAsync(data, cancellationToken);
        }

        private async Task StartAsyncProcessing()
        {
            while (true)
            {
                EventDataBatch batch = _client.CreateBatch();
                bool stopCollectingEvents = false;

                if (_next != null && batch.TryAdd(_next))
                {
                    _next = null;
                }

                while (!stopCollectingEvents)
                {
                    EventData data = await _input.Reader.ReadAsync();
                    if (!batch.TryAdd(data))
                    {
                        _next = data;
                    }

                    stopCollectingEvents = _next != null || data == null;
                }

                await _output.Writer.WriteAsync(batch.ToEnumerable().ToArray());
            }
        }
    }
}
