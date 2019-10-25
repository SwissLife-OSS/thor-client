using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly ConcurrentQueue<EventData> _input = new ConcurrentQueue<EventData>();
        private readonly ConcurrentQueue<IEnumerable<EventData>> _output = new ConcurrentQueue<IEnumerable<EventData>>();
        private static readonly TimeSpan _delay = TimeSpan.FromMilliseconds(50);
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

            StartAsyncProcessing();
        }

        /// <inheritdoc />
        public int Count { get { return _output.Count; } }

        /// <inheritdoc />
        public Task<IEnumerable<EventData>> DequeueAsync(CancellationToken cancellationToken)
        {
            if (_output.TryDequeue(out IEnumerable<EventData> batch))
            {
                return Task.FromResult(batch);
            }

            return Task.FromResult(Enumerable.Empty<EventData>());
        }

        /// <inheritdoc />
        public Task EnqueueAsync(EventData data, CancellationToken cancellationToken)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _input.Enqueue(data);

            return Task.FromResult(0);
        }

        private void StartAsyncProcessing()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (_input.Count < 50)
                    {
                        await Task.Delay(_delay).ConfigureAwait(false);
                    }

                    TransformToBatch();
                }
            });
        }

        private void TransformToBatch()
        {
            EventDataBatch batch = _client.CreateBatch();
            bool stopCollectingEvents = false;

            if (_next != null && batch.TryAdd(_next))
            {
                _next = null;
            }

            while (!stopCollectingEvents)
            {
                if (_input.TryDequeue(out EventData data))
                {
                    if (!batch.TryAdd(data))
                    {
                        batch = _client.CreateBatch();
                        _next = data;
                    }
                }

                stopCollectingEvents = _next != null || data == null;
            }

            _output.Enqueue(batch.ToEnumerable());
        }
    }
}