namespace Thor.Core.Abstractions
{
    /// <summary>
    /// An exception attachment.
    /// </summary>
    public class ExceptionAttachment
        : IAttachment
    {
        /// <inheritdoc/>
        public AttachmentId Id { get; set; }

        /// <inheritdoc/>
        public byte[] Content { get; set; }
    }
}