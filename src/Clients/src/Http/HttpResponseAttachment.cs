﻿using Thor.Core.Abstractions;

namespace Thor.Extensions.Http
{
    /// <summary>
    /// A <c>HTTP</c> request attachment (without body).
    /// </summary>
    public class HttpResponseAttachment
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