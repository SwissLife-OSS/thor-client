namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A transmitter for telemetry attachments.
    /// </summary>
    public interface ITelemetryAttachmentTransmitter
        : ITelemetryTransmitter<AttachmentDescriptor>
    {
    }
}