using System;
using Thor.Core.Abstractions;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="IAttachment"/> extensions.
    /// </summary>
    public static class AttachmentExtensions
    {
        private const string _typeNameSuffix = "Attachment";

        /// <summary>
        /// Gets the type name of an attachment minus "Attachment".
        /// </summary>
        /// <param name="attachment">An attachment to get the type name for.</param>
        /// <returns>A type name.</returns>
        public static string GetTypeName(this IAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            return attachment.GetType().Name.Replace(_typeNameSuffix, string.Empty);
        }

        /// <summary>
        /// Gets the type name of an attachment minus "Attachment".
        /// </summary>
        /// <param name="type">An type to get the type name for.</param>
        /// <returns>A type name.</returns>
        public static string GetTypeName(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.Name.Replace(_typeNameSuffix, string.Empty);
        }
    }
}