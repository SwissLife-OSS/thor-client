using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly ConcurrentQueue<EventData[]> _output = new ConcurrentQueue<EventData[]>();
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
        public Task<EventData[]> DequeueAsync(CancellationToken cancellationToken)
        {
            if (_output.TryDequeue(out EventData[] batch))
            {
                return Task.FromResult(batch);
            }

            return Task.FromResult(new EventData[0]);
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
            List<EventData> batch = new List<EventData>();
            // todo: find a better way of checking batch size
            // explaination: EventDataBatch.ToEnumerable() is internal so we cannot use it for
            // passing just the pure list to the TransmissionSender thats why we have the second
            // list. thats bad because we have two list instead of one, but this is just an interim
            // solution which we will retire as soon as possible.
            EventDataBatch batchToCheckSize = _client.CreateBatch();
            bool stopCollectingEvents = false;
            EventData data;

            if (_next != null && batchToCheckSize.TryAdd(_next))
            {
                _next = null;
            }

            while (!stopCollectingEvents)
            {
                if (_input.TryDequeue(out data))
                {
                    if (batchToCheckSize.TryAdd(data))
                    {
                        batch.Add(data);
                    }
                    else
                    {
                        batchToCheckSize = _client.CreateBatch();
                        _next = data;
                    }
                }

                stopCollectingEvents = (_next != null || data == null);
            }

            if (batch.Count > 0)
            {
                _output.Enqueue(batch.ToArray());
            }
        }
    }
}