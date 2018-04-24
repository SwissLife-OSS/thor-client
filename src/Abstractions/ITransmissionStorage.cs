using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A transmission storage to cache telemetry data batches in <c>memory</c> or on the
    /// <c>local file system</c> to prevent flooding the backend service.
    /// </summary>
    public interface ITransmissionStorage<TData>
        where TData : class
    {
        /// <summary>
        /// Enqueues a telemetry data batch to a <c>short-term</c> storage.
        /// </summary>
        /// <param name="batch">A telemetry data batch.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="batch"/> must not be <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="batch"/> must contain at least one item.
        /// </exception>
        Task EnqueueAsync(TData[] batch, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to dequeue a telemetry data batch from the storage.
        /// </summary>
        /// <param name="batch">A telemetry data batch.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A value indicating whether a telemetry data batch could be returned.</returns>
        Task<bool> TryDequeueAsync(out TData[] batch, CancellationToken cancellationToken);
    }
}