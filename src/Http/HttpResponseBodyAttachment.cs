using Thor.Core.Abstractions;

namespace Thor.Core.Http
{
    /// <summary>
    /// A <c>HTTP</c> request body attachment.
    /// </summary>
    public class HttpResponseBodyAttachment
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