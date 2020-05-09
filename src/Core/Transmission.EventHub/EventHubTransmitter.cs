using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A telemetry event transmitter for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public sealed class EventHubTransmitter
        : ITelemetryEventTransmitter
        , IDisposable
    {
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly IMemoryBuffer<EventData> _buffer;
        private readonly ITransmissionBuffer<EventData> _aggregator;
        private readonly ITransmissionSender<EventData> _sender;
        private readonly ITransmissionStorage<EventData> _storage;
        private readonly EventsOptions _options;
        private readonly Job _sendJob;
        private readonly Job _storeJob;
        private readonly Job _aggregateJob;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmitter"/> class.
        /// </summary>
        /// <param name="buffer">A transmission buffer instance.</param>
        /// <param name="aggregator">A transmission aggregator instance.</param>
        /// <param name="sender">A transmission sender instance.</param>
        /// <param name="storage">A transmission storage instance.</param>
        /// <param name="options">EventHub transmission options.</param>
        public EventHubTransmitter(
            IMemoryBuffer<EventData> buffer,
            ITransmissionBuffer<EventData> aggregator,
            ITransmissionSender<EventData> sender,
            ITransmissionStorage<EventData> storage,
            EventsOptions options)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _storeJob = Job.Start(
                async () => await StoreBatchAsync().ConfigureAwait(false),
                () => _buffer.Count == 0,
                _disposeToken.Token);

            _aggregateJob = Job.Start(
                async () => await AggregateBatchAsync().ConfigureAwait(false),
                () => _aggregator.Count == 0,
                _disposeToken.Token);

            _sendJob = Job.Start(
                async () => await SendBatchAsync().ConfigureAwait(false),
                () => !_storage.HasData,
                _disposeToken.Token);
        }

        /// <inheritdoc />
        public void Enqueue(TelemetryEvent data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!_disposeToken.IsCancellationRequested)
            {
                _buffer.Enqueue(data.Map());
            }
        }

        private async Task SendBatchAsync()
        {
            IReadOnlyCollection<EventData> batch = _aggregator.Dequeue();

            if (batch.Count > 0)
            {
                await _sender
                    .SendAsync(batch, _disposeToken.Token)
                    .ConfigureAwait(false);
            }
        }

        private async Task StoreBatchAsync()
        {
            IReadOnlyCollection<EventData> batch = _buffer.Dequeue(
                _options.Buffer.DequeueBatchSize);

            if (batch.Count > 0)
            {
                await _storage
                    .EnqueueAsync(batch, _disposeToken.Token)
                    .ConfigureAwait(false);
            }
        }

        private async Task AggregateBatchAsync()
        {
            IReadOnlyCollection<EventData> batch = await _storage
                .DequeueAsync(_options.Storage.DequeueBatchSize, _disposeToken.Token)
                .ConfigureAwait(false);

            foreach (EventData data in batch)
            {
                _aggregator.Enqueue(data);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeToken.Cancel();

                if (!_sendJob.Stopped && !_storeJob.Stopped)
                {
                    WaitHandle.WaitAll(new[]
                    {
                        _sendJob.WaitHandle,
                        _storeJob.WaitHandle
                    }, TimeSpan.FromSeconds(5));
                }

                _disposeToken?.Dispose();
                _aggregateJob?.Dispose();
                _sendJob?.Dispose();
                _storeJob?.Dispose();

                _disposed = true;
            }
        }
    }
}
