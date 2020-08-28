using System;
using System.Threading;
using System.Threading.Tasks;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// Represents an abstratcion for the BLOB storage.
    /// </summary>
    public interface IBlobContainer
    {
        /// <summary>
        /// Uploads a attachment to the BLOB storage.
        /// </summary>
        /// <param name="descriptor">A attchment descriptor.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="descriptor"/> must not be <c>null</c>.
        /// </exception>
        Task UploadAsync(AttachmentDescriptor descriptor, CancellationToken cancellationToken);
    }
}
