using System;
using System.Collections.Generic;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// An attachment transmission initializer.
    /// </summary>
    public class AttachmentTransmissionInitializer
        : IAttachmentTransmissionInitializer
    {
        private readonly IEnumerable<ITelemetryAttachmentTransmitter> _transmitters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentTransmissionInitializer"/> class.
        /// </summary>
        /// <param name="transmitters"></param>
        public AttachmentTransmissionInitializer(
            IEnumerable<ITelemetryAttachmentTransmitter> transmitters)
        {
            _transmitters = transmitters ??
                throw new ArgumentNullException(nameof(transmitters));
        }

        /// <inheritdoc />
        public void Initialize()
        {
            foreach (var transmitter in _transmitters)
            {
                AttachmentDispatcher.Instance.Attach(transmitter);
            }
        }
    }
}
