using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(50);
        private static readonly int MaxBufferSize = 1000;
        private static readonly EventData[] EmptyBatch = new EventData[0];
        private readonly BlockingCollection<EventData> _input = new BlockingCollection<EventData>(MaxBufferSize);
        private readonly ConcurrentQueue<EventData[]> _output = new ConcurrentQueue<EventData[]>();
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
        public int Count { get { return _output.Count; } }

        /// <inheritdoc />
        public EventData[] Dequeue()
        {
            if (_output.TryDequeue(out EventData[] batch))
            {
                return batch;
            }

            return EmptyBatch;
        }

        /// <inheritdoc />
        public void Enqueue(EventData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _input.TryAdd(data, TimeSpan.FromMilliseconds(-1));
        }

        private async Task StartAsyncProcessing()
        {
            while (true)
            {
                if (_input.Count < 50)
                {
                    await Task.Delay(Delay).ConfigureAwait(false);
                }

                TransformToBatch();
            }
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
                if (_input.TryTake(out EventData data))
                {
                    if (!batch.TryAdd(data))
                    {
                        _next = data;
                    }
                }

                stopCollectingEvents = _next != null || data == null;
            }

            _output.Enqueue(batch.ToEnumerable().ToArray());
        }
    }
}
