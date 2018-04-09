using System;
using System.Runtime.InteropServices;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// An identifier for attachments.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct AttachmentId
        : IEquatable<AttachmentId>
    {
        private readonly DateTime _timestamp;
        private readonly Guid _uniqueId;

        private AttachmentId(DateTime timestamp, Guid uniqueId)
        {
            _timestamp = timestamp;
            _uniqueId = uniqueId;
        }

        /// <summary>
        /// Gets an empty attachment id.
        /// </summary>
        public static AttachmentId Empty
        {
            get
            {
                return new AttachmentId();
            }
        }

        /// <summary>
        /// Creates a new id.
        /// </summary>
        /// <returns>An attachment id.</returns>
        public static AttachmentId NewId()
        {
            return new AttachmentId(DateTime.UtcNow, Guid.NewGuid());
        }

        /// <inheritdoc/>
        public static bool operator ==(AttachmentId left, AttachmentId right)
        {
            return left._timestamp == right._timestamp &&
                left._uniqueId.Equals(right._uniqueId);
        }

        /// <inheritdoc/>
        public static bool operator !=(AttachmentId left, AttachmentId right)
        {
            return left._timestamp != right._timestamp ||
                !left._uniqueId.Equals(right._uniqueId);
        }

        /// <inheritdoc/>
        public bool Equals(AttachmentId id)
        {
            return _timestamp == id._timestamp && _uniqueId.Equals(id._uniqueId);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is AttachmentId))
            {
                return false;
            }

            return Equals((AttachmentId)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = _timestamp.GetHashCode() ^ _uniqueId.GetHashCode();

            return hashCode.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{_timestamp:yyyyMMdd}-{_uniqueId:N}";
        }
    }
}