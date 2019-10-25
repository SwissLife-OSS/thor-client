using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="ITransmissionBuffer{TData}"/> extension methods.
    /// </summary>
    public static class TransmissionBufferExtensions
    {
        /// <summary>
        /// Dequeues a telemetry data batch from the buffer.
        /// </summary>
        /// <param name="buffer">A transmission buffer instance.</param>
        /// <returns>A telemetry data batch.</returns>
        public static Task<IEnumerable<TData>> DequeueAsync<TData>(this ITransmissionBuffer<TData> buffer)
            where TData : class
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return buffer.DequeueAsync(CancellationToken.None);
        }

        /// <summary>
        /// Enqueues a single telemetry data object.
        /// </summary>
        /// <param name="buffer">A transmission buffer instance.</param>
        /// <param name="data">A telemetry data object.</param>
        public static Task EnqueueAsync<TData>(this ITransmissionBuffer<TData> buffer, TData data)
            where TData : class
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return buffer.EnqueueAsync(data, CancellationToken.None);
        }
    }
}