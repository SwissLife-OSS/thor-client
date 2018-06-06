using System;
using System.Threading;
using System.Threading.Tasks;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IBlobContainer"/>.
    /// </summary>
    public static class BlobContainerExtensions
    {
        /// <summary>
        /// Adds <c>Azure BLOB Storage</c> telemetry attachment transmission services to the service collection.
        /// </summary>
        /// <param name="container">A <see cref="IBlobContainer"/> instance.</param>
        /// <param name="descriptor">An attachment descriptor instance.</param>
        public static Task UploadAsync(this IBlobContainer container, AttachmentDescriptor descriptor)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return container.UploadAsync(descriptor, CancellationToken.None);
        }
    }
}