using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly TimeSpan _delay = TimeSpan.FromMilliseconds(50);
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly ManualResetEventSlim _syncTransmission = new ManualResetEventSlim();
        private readonly ManualResetEventSlim _syncStore = new ManualResetEventSlim();
        private readonly WaitHandle[] _sync;
        private readonly ITransmissionBuffer<EventData> _buffer;
        private readonly ITransmissionSender<EventData> _sender;
        private readonly ITransmissionStorage<EventData> _storage;
        private bool _disposed;
        private bool _transmissionStopped;
        private bool _storeStopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmitter"/> class.
        /// </summary>
        /// <param name="buffer">A transmission buffer instance.</param>
        /// <param name="sender">A transmission sender instance.</param>
        /// <param name="storage">A transmission storage instance.</param>
        public EventHubTransmitter(
            ITransmissionBuffer<EventData> buffer,
            ITransmissionSender<EventData> sender,
            ITransmissionStorage<EventData> storage)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _sync = new[] {_syncTransmission.WaitHandle, _syncStore.WaitHandle};

            StartAsyncStore();
            StartAsyncSending();
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
                Task.Run(() => _buffer.EnqueueAsync(data.Map(), _disposeToken.Token));
            }
        }

        private void StartAsyncSending()
        {
            Task.Run(async () =>
            {
                while (!_disposeToken.IsCancellationRequested)
                {
                    await SendBatchAsync().ConfigureAwait(false);

                    if (!_disposeToken.IsCancellationRequested && _buffer.Count == 0)
                    {
                        await Task.Delay(_delay).ConfigureAwait(false);
                    }
                }

                _transmissionStopped = true;
                _syncTransmission.Set();
                _disposeToken.Token.ThrowIfCancellationRequested();
            });
        }

        private async Task SendBatchAsync()
        {
            IEnumerable<EventData> batch = await _storage
                .DequeueAsync(_disposeToken.Token)
                .ConfigureAwait(false);

            if (batch.Any())
            {
                await _sender
                    .SendAsync(batch, _disposeToken.Token)
                    .ConfigureAwait(false);
            }
        }

        private void StartAsyncStore()
        {
            Task.Run(async () =>
            {
                _disposeToken.Token.ThrowIfCancellationRequested();

                while (!_disposeToken.IsCancellationRequested || _buffer.Count > 0)
                {
                    await StoreBatchAsync().ConfigureAwait(false);

                    if (!_disposeToken.IsCancellationRequested && _buffer.Count == 0)
                    {
                        await Task.Delay(_delay).ConfigureAwait(false);
                    }
                }

                _storeStopped = true;
                _syncStore.Set();
                _disposeToken.Token.ThrowIfCancellationRequested();
            });
        }

        private async Task StoreBatchAsync()
        {
            EventData[] batch = await _buffer
                .DequeueAsync(_disposeToken.Token)
                .ConfigureAwait(false);

            if (batch.Length > 0)
            {
                await _storage
                    .EnqueueAsync(batch.ToArray(), _disposeToken.Token)
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeToken.Cancel();

                if (!_transmissionStopped && !_storeStopped)
                {
                    WaitHandle.WaitAll(_sync, TimeSpan.FromSeconds(5));
                }

                _disposeToken?.Dispose();
                _syncTransmission?.Dispose();
                _syncStore?.Dispose();

                _disposed = true;
            }
        }
    }
}
