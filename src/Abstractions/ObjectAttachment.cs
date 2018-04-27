namespace Thor.Core.Abstractions
{
    /// <summary>
    /// An object attachment.
    /// </summary>
    public class ObjectAttachment
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