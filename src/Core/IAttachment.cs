namespace Thor.Core
{
    /// <summary>
    /// An attachment is a complex type which is part of a ETW event in form of a payload, but
    /// stored in a different way because of the limits of ETW itself. ETW is not able to handle
    /// complex types or even reference types and has a limit in size (64KB with headers).
    /// </summary>
    public interface IAttachment
    {
        /// <summary>
        /// Gets or sets the correlation id. This id is used to link the event with the attachment.
        /// The format should be "YYYYMMDD-GUID".
        /// </summary>
        AttachmentId Id { get; set; }

        /// <summary>
        /// Gets or sets the payload name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the serialized payload value.
        /// </summary>
        byte[] Value { get; set; }
    }
}