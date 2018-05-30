using System;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="AttachmentDescriptor"/> extension methods.
    /// </summary>
    public static class AttachmentDescriptorExtensions
    {
        /// <summary>
        /// Enqueues a single telemetry data object.
        /// </summary>
        /// <param name="descriptor">An attachment descriptor instance.</param>
        /// <returns>A file path for the specified <paramref name="descriptor"/>.</returns>
        public static string GetFilepath(this AttachmentDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            return $"{descriptor.Id}\\{descriptor.Name}";
        }
    }
}