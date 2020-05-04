using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A transmission sender for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public class BlobContainer
        : IBlobContainer
    {
        private readonly CloudBlobContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmissionSender"/> class.
        /// </summary>
        /// <param name="container">A <c>Azure</c> <c>BLOB</c> <c>Storage</c> container instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container"/> must not be <c>null</c>.
        /// </exception>
        public BlobContainer(CloudBlobContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <inheritdoc/>
        public async Task UploadAsync(AttachmentDescriptor descriptor, CancellationToken cancellationToken)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            try
            {
                await _container.GetBlockBlobReference(descriptor.GetFilepath())
                    .UploadFromByteArrayAsync(descriptor.Value, 0, descriptor.Value.Length, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                // todo: log via event provider
            }
        }
    }
}
