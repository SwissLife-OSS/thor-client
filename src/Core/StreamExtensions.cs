using System;
using System.IO;

namespace Thor.Core
{
    /// <summary>
    /// An bunch of convenient <see cref="Stream"/> extensions.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Sets a stream to its start position if possible.
        /// </summary>
        /// <param name="stream">A stream.</param>
        /// <returns>The provided stream.</returns>
        public static Stream SetToStart(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return stream;
        }
    }
}