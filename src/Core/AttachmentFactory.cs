using System;
using System.IO;

namespace Thor.Core
{
    /// <summary>
    /// An attachment factory.
    /// </summary>
    public static class AttachmentFactory
    {
        /// <summary>
        /// Creates a new attachment for an exception.
        /// </summary>
        /// <param name="id">A correlation id.</param>
        /// <param name="payloadName">A payload name.</param>
        /// <param name="payloadValue">A payload value.</param>
        /// <returns>A new attachment instance.</returns>
        public static ExceptionAttachment Create(AttachmentId id, string payloadName, Exception payloadValue)
        {
            return Create<ExceptionAttachment, Exception>(id, payloadName, payloadValue);
        }

        /// <summary>
        /// Creates a new attachment for an object.
        /// </summary>
        /// <param name="id">A correlation id.</param>
        /// <param name="payloadName">A payload name.</param>
        /// <param name="payloadValue">A payload value.</param>
        /// <returns>A new attachment instance.</returns>
        public static ObjectAttachment Create(AttachmentId id, string payloadName, object payloadValue)
        {
            return Create<ObjectAttachment, object>(id, payloadName, payloadValue);
        }

        /// <summary>
        /// Creates a new attachment.
        /// </summary>
        /// <param name="id">A correlation id.</param>
        /// <param name="payloadName">A payload name.</param>
        /// <param name="payloadValue">A payload value.</param>
        /// <returns>A new attachment instance.</returns>
        public static TAttachment Create<TAttachment, TPayload>(AttachmentId id, string payloadName,
            TPayload payloadValue)
                where TAttachment : class, IAttachment, new()
        {
            TAttachment attachment = null;

            if (id != AttachmentId.Empty && !string.IsNullOrWhiteSpace(payloadName) &&
                payloadValue != null)
            {
                attachment = Activator.CreateInstance<TAttachment>();
                attachment.Id = id;
                attachment.Name = payloadName;
                attachment.Value = payloadValue.SerializeAttachmentContent();
            }

            return attachment;
        }

        /// <summary>
        /// Creates a new attachment for a stream.
        /// </summary>
        /// <param name="id">A correlation id.</param>
        /// <param name="payloadName">A payload name.</param>
        /// <param name="payloadValue">A payload value.</param>
        /// <returns>A new attachment instance.</returns>
        public static TAttachment Create<TAttachment>(AttachmentId id, string payloadName,
            Stream payloadValue)
                where TAttachment : class, IAttachment, new()
        {
            TAttachment attachment = null;

            if (payloadValue?.Length > 0)
            {
                using (StreamReader reader = new StreamReader(payloadValue.SetToStart()))
                {
                    attachment = Create<TAttachment, string>(id, payloadName, reader.ReadToEnd());
                }
            }

            return attachment;
        }
    }
}