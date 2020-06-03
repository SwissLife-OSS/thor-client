using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A buffer for telemetry data transmission which enqueue single data objects for batch processing.
    /// </summary>
    public interface ITransmissionBuffer<TData>
        where TData : class
    {
        /// <summary>
        /// Dequeue a telemetry data batch from the buffer.
        /// </summary>
        /// <returns>A telemetry data batch.</returns>
        IAsyncEnumerable<TData[]> Dequeue(CancellationToken cancellationToken);

        /// <summary>
        /// Enqueue a telemetry data object.
        /// </summary>
        Task Enqueue(TData data, CancellationToken cancellationToken);
    }
}
