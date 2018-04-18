namespace Thor.Core
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
        public string Name { get; set; }

        /// <inheritdoc/>
        public byte[] Value { get; set; }
    }
}