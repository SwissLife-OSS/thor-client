using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A transmission storage to cache telemetry data batches in <c>memory</c> or on the
    /// <c>local file system</c> to prevent flooding the backend service.
    /// </summary>
    public interface ITransmissionStorage<TData>
        where TData : class
    {
        /// <summary>
        /// Dequeue a data batch from the storage.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A telemetry data batch.</returns>
        IAsyncEnumerable<TData> DequeueAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Enqueue a telemetry data batch to a <c>short-term</c> storage.
        /// </summary>
        /// <param name="batch">A data batch to be stored.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task EnqueueAsync(IAsyncEnumerable<TData> batch, CancellationToken cancellationToken);
    }
}
