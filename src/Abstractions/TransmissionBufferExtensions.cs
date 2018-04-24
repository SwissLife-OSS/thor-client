using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="ITransmissionBuffer{TData}"/> extension methods.
    /// </summary>
    public static class TransmissionBufferExtensions
    {
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

        /// <summary>
        /// Tries to dequeue an telemetry data batch from the buffer.
        /// </summary>
        /// <param name="buffer">A transmission buffer instance.</param>
        /// <param name="batch">A telemetry data batch.</param>
        /// <returns>A value indicating whether a telemetry data batch could be returned.</returns>
        public static Task<bool> TryDequeueAsync<TData>(this ITransmissionBuffer<TData> buffer,
            out TData[] batch)
                where TData : class
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return buffer.TryDequeueAsync(out batch, CancellationToken.None);
        }
    }
}