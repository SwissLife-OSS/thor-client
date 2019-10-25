using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A buffer for telemetry data transmission which enqueues single data objects for batch processing.
    /// </summary>
    public interface ITransmissionBuffer<TData>
        where TData : class
    {
        /// <summary>
        /// Gets the count of batches.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Dequeues a telemetry data batch from the buffer.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A telemetry data batch.</returns>
        Task<IEnumerable<TData>> DequeueAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Enqueues a single telemetry data object.
        /// </summary>
        /// <param name="data">A telemetry data object.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> must not be <c>null</c>.
        /// </exception>
        Task EnqueueAsync(TData data, CancellationToken cancellationToken);
    }
}