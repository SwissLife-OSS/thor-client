using System;
using Microsoft.Azure.EventHubs;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A transmission storage for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public class EventHubTransmissionStorage
        : FileStorage<EventData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmissionBuffer"/> class.
        /// </summary>
        /// <param name="storagePath">A storage path to save temporarily.</param>
        public EventHubTransmissionStorage(string storagePath)
            : base(storagePath)
        {
        }

        /// <inheritdoc/>
        protected override EventData Deserialize(byte[] payload, string fileName)
        {
            return new EventData(payload);
        }

        /// <inheritdoc/>
        protected override byte[] Serialize(EventData data)
        {
            return data.Body.Array;
        }

        /// <inheritdoc/>
        protected override string EncodeFileName(EventData data)
        {
            return DateTime.UtcNow.Ticks.ToString();
        }
    }
}
