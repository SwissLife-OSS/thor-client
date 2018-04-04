namespace Thor.Core.Abstractions
{
    /// <summary>
    /// An attachment is an object that is connected to a single ETW event but stored in a different way.
    /// </summary>
    public interface IAttachment
    {
        /// <summary>
        /// Gets or sets the correlation id. This id is used to link the event with the attachment.
        /// The format should be "YYYYMMDD-GUID".
        /// </summary>
        AttachmentId Id { get; set; }

        /// <summary>
        /// Gets or sets the serialized content.
        /// </summary>
        byte[] Content { get; set; }
    }
}