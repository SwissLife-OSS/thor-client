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
        private readonly IMemoryBuffer<AttachmentDescriptor> _buffer;
        private readonly ITransmissionStorage<AttachmentDescriptor> _storage;
        private readonly ITransmissionSender<AttachmentDescriptor> _sender;
        private readonly AttachmentsOptions _options;
        private readonly Job _sendJob;
        private readonly Job _storeJob;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmitter"/> class.
        /// </summary>
        public BlobStorageTransmitter(
            IMemoryBuffer<AttachmentDescriptor> buffer,
            ITransmissionStorage<AttachmentDescriptor> storage,
            ITransmissionSender<AttachmentDescriptor> sender,
            AttachmentsOptions options)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _storeJob = Job.Start(
                async () => await StoreBatchAsync().ConfigureAwait(false),
                () => _buffer.Count == 0,
                _disposeToken.Token);

            _sendJob = Job.Start(
                async () => await SendBatchAsync().ConfigureAwait(false),
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
                _buffer.Enqueue(data);
            }
        }

        private async Task StoreBatchAsync()
        {
            IReadOnlyCollection<AttachmentDescriptor> batch = _buffer.Dequeue(
                _options.Buffer.DequeueBatchSize);

            if (batch.Count > 0)
            {
                await _storage
                    .EnqueueAsync(batch, _disposeToken.Token)
                    .ConfigureAwait(false);
            }
        }

        private async Task SendBatchAsync()
        {
            await _sender
                .SendAsync(_storage.DequeueAsync(_disposeToken.Token), _disposeToken.Token)
                .ConfigureAwait(false);
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
                _resetEvent?.Dispose();
                _sendJob?.Dispose();
                _storeJob?.Dispose();

                _disposed = true;
            }
        }
    }
}
