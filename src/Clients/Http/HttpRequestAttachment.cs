using Thor.Core.Abstractions;

namespace Thor.Core.Http
{
    /// <summary>
    /// A <c>HTTP</c> request attachment (without body).
    /// </summary>
    public class HttpRequestAttachment
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