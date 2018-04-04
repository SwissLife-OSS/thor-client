namespace Thor.Core.Abstractions
{
    /// <summary>
    /// An descriptor for attachments.
    /// </summary>
    public class AttachmentDescriptor
    {
        /// <summary>
        /// Gets or sets an unique id which starts with a timestamp.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type name minus "Attachment".
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the content stream.
        /// </summary>
        public byte[] Content { get; set; }
    }
}