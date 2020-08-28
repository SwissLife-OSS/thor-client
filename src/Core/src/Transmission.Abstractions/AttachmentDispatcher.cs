using System;
using System.Collections.Generic;
using Thor.Core.Abstractions;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A dispatcher for attachments.
    /// </summary>
    public class AttachmentDispatcher
    {
        private static readonly object _sync = new object();
        private HashSet<ITelemetryAttachmentTransmitter> _transmitters;

        private AttachmentDispatcher()
            : this(new HashSet<ITelemetryAttachmentTransmitter>())
        { }

        internal AttachmentDispatcher(
            HashSet<ITelemetryAttachmentTransmitter> transmitters)
        {
            _transmitters = transmitters ?? 
                throw new ArgumentNullException(nameof(transmitters));
        }

        /// <summary>
        /// Gets a singelton instance of <see cref="AttachmentDispatcher"/>.
        /// </summary>
        public static AttachmentDispatcher Instance { get; } =
            new AttachmentDispatcher();

        /// <summary>
        /// Attaches a transmitter for telemetry attachment transmission.
        /// </summary>
        /// <param name="transmitter">A attachment transmitter.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="transmitter"/> is null.
        /// </exception>
        public void Attach(ITelemetryAttachmentTransmitter transmitter)
        {
            if (transmitter == null)
            {
                throw new ArgumentNullException(nameof(transmitter));
            }

            lock (_sync)
            {
                if (!_transmitters.Contains(transmitter))
                {
                    var newTransmitters =
                        new HashSet<ITelemetryAttachmentTransmitter>(
                            _transmitters);

                    newTransmitters.Add(transmitter);
                    _transmitters = newTransmitters;
                }
            }
        }

        /// <summary>
        /// Detaches a transmitter for telemetry attachment transmission.
        /// </summary>
        /// <param name="transmitter">A attachment transmitter.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="transmitter"/> is null.
        /// </exception>
        public void Detach(ITelemetryAttachmentTransmitter transmitter)
        {
            if (transmitter == null)
            {
                throw new ArgumentNullException(nameof(transmitter));
            }

            lock (_sync)
            {
                if (_transmitters.Contains(transmitter))
                {
                    var newTransmitters =
                        new HashSet<ITelemetryAttachmentTransmitter>(
                            _transmitters);

                    newTransmitters.Remove(transmitter);
                    _transmitters = newTransmitters;
                }
            }
        }

        /// <summary>
        /// Dispatches one or more attachments.
        /// </summary>
        /// <param name="attachments">A collection of attachments.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="attachments"/> is null.
        /// </exception>
        public void Dispatch(params IAttachment[] attachments)
        {
            if (attachments == null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }

            if (attachments.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attachments));
            }

            var descriptors = new AttachmentDescriptor[attachments.Length];

            for (int i = 0; i < attachments.Length; i++)
            {
                descriptors[i] = new AttachmentDescriptor
                {
                    Id = attachments[i].Id,
                    Name = attachments[i].Name,
                    TypeName = attachments[i].GetTypeName(),
                    Value = attachments[i].Value
                };
            }

            foreach (var transmitter in _transmitters)
            {
                foreach (AttachmentDescriptor descriptor in descriptors)
                {
                    transmitter.Enqueue(descriptor);
                }
            }
        }
    }
}