using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="ITransmissionSender{TData}"/> extension methods.
    /// </summary>
    public static class TransmissionSenderExtensions
    {
        /// <summary>
        /// Sends a telemetry data batch to a backend service/solution for further processing.
        /// </summary>
        /// <param name="sender">A transmission sender instance.</param>
        /// <param name="batch">A telemetry event batch.</param>
        public static Task SendAsync<TData>(this ITransmissionSender<TData> sender, IEnumerable<TData> batch)
                where TData : class
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            return sender.SendAsync(batch, CancellationToken.None);
        }
    }
}