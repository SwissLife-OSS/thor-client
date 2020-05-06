using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Thor.Core.Transmission.Abstractions;
using Thor.Core.Transmission.EventHub;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A telemetry attachment transmitter for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public sealed class BlobStorageTransmitter
        : ITelemetryAttachmentTransmitter
        , IDisposable
    {
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim();
        private readonly ITransmissionStorage<AttachmentDescriptor> _storage;
        private readonly ITransmissionSender<AttachmentDescriptor> _sender;
        private readonly AttachmentsOptions _options;
        private readonly Job _sendJob;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmitter"/> class.
        /// </summary>
        public BlobStorageTransmitter(
            ITransmissionStorage<AttachmentDescriptor> storage,
            ITransmissionSender<AttachmentDescriptor> sender,
            AttachmentsOptions options)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _sendJob = Job.Start(
                async () => await SendBatchAsync().ConfigureAwait(false),
                () => !_storage.HasData,
                _disposeToken.Token);
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
                Task.Run(() => _storage.EnqueueAsync(data, _disposeToken.Token));
            }
        }

        private async Task SendBatchAsync()
        {
            // Add disposable dequeue and delete files after send
            IReadOnlyCollection<AttachmentDescriptor> batch = await _storage
                .DequeueAsync(_options.StorageDequeueBatchSize, _disposeToken.Token)
                .ConfigureAwait(false);

            if (batch.Count > 0)
            {
                await _sender
                    .SendAsync(batch, _disposeToken.Token)
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeToken.Cancel();

                if (!_sendJob.Stopped)
                {
                    WaitHandle.WaitAll(new[]
                    {
                        _sendJob.WaitHandle
                    }, TimeSpan.FromSeconds(5));
                }

                _disposeToken?.Dispose();
                _resetEvent?.Dispose();

                _disposed = true;
            }
        }
    }
}
