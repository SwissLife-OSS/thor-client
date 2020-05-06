using System;
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
        /// Gets information if storage has any data.
        /// </summary>
        bool HasData { get; }

        /// <summary>
        /// Dequeue a data batch from the storage.
        /// </summary>
        /// <param name="count">Number of items to be dequeued.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A telemetry data batch.</returns>
        Task<IReadOnlyCollection<TData>> DequeueAsync(int count, CancellationToken cancellationToken);

        /// <summary>
        /// Enqueue a telemetry data batch to a <c>short-term</c> storage.
        /// </summary>
        /// <param name="batch">A data batch to be stored.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task EnqueueAsync(IReadOnlyCollection<TData> batch, CancellationToken cancellationToken);

        /// <summary>
        /// Enqueue a telemetry data object to a <c>short-term</c> storage.
        /// </summary>
        /// <param name="batch">A data object to be stored.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task EnqueueAsync(TData batch, CancellationToken cancellationToken);
    }
}
