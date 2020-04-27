using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A telemetry attachment transmitter for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public sealed class BlobStorageTransmitter
        : ITelemetryAttachmentTransmitter
        , IDisposable
    {
        private static readonly TimeSpan _delay = TimeSpan.FromMilliseconds(50);
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim();
        private readonly ITransmissionStorage<AttachmentDescriptor> _storage;
        private readonly ITransmissionSender<AttachmentDescriptor> _sender;
        private bool _disposed;
        private bool _transmissionStopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmitter"/> class.
        /// </summary>
        /// <param name="storage">A transmission storage instance.</param>
        /// <param name="sender">A transmission sender instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="storage"/> must not be <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> must not be <c>null</c>.
        /// </exception>
        public BlobStorageTransmitter(ITransmissionStorage<AttachmentDescriptor> storage,
            ITransmissionSender<AttachmentDescriptor> sender)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));

            StartAsyncSending();
        }

        /// <inheritdoc/>
        public void Enqueue(AttachmentDescriptor data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!_disposeToken.IsCancellationRequested)
            {
                Task.Run(() => _storage.EnqueueAsync(new[] { data }, _disposeToken.Token));
            }
        }

        private async Task SendBatchAsync()
        {
            IReadOnlyCollection<AttachmentDescriptor> batch = await _storage
                .DequeueAsync(_disposeToken.Token)
                .ConfigureAwait(false);

            if (batch.Count > 0)
            {
                await _sender.SendAsync(batch).ConfigureAwait(false);
            }
        }

        private void StartAsyncSending()
        {
            Task.Run(async () =>
            {
                _disposeToken.Token.ThrowIfCancellationRequested();

                while (!_disposeToken.IsCancellationRequested || _storage.HasData)
                {
                    await SendBatchAsync().ConfigureAwait(false);

                    if (!_disposeToken.IsCancellationRequested && _storage.HasData)
                    {
                        await Task.Delay(_delay).ConfigureAwait(false);
                    }
                }

                _transmissionStopped = true;
                _resetEvent.Set();
                _disposeToken.Token.ThrowIfCancellationRequested();
            }, _disposeToken.Token);
        }

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeToken.Cancel();

                if (!_transmissionStopped)
                {
                    _resetEvent.Wait(TimeSpan.FromSeconds(5));
                }

                _disposeToken?.Dispose();
                _resetEvent?.Dispose();

                _disposed = true;
            }
        }

        #endregion
    }
}
