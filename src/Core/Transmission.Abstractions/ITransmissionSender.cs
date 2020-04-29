using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A transmission sender for telemetry data batches. The sender is responsible for sending
    /// the batches to the backend service/solution for further processing.
    /// </summary>
    public interface ITransmissionSender<in TData>
        where TData : class
    {
        /// <summary>
        /// Sends a telemetry data batch to a backend service/solution for further processing.
        /// </summary>
        /// <param name="batch">A telemetry data batch.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="batch"/> must not be <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="batch"/> must contain at least one item.
        /// </exception>
        Task SendAsync(IReadOnlyCollection<TData> batch, CancellationToken cancellationToken);
    }
}
