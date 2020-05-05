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
        /// <returns>A telemetry data batch.</returns>
        TData[] Dequeue();

        /// <summary>
        /// Enqueues a single telemetry data object.
        /// </summary>
        /// <param name="data">A telemetry data object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> must not be <c>null</c>.
        /// </exception>
        void Enqueue(TData data);
    }
}
