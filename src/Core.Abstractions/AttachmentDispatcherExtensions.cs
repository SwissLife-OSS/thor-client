using System;
using System.IO;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="AttachmentDispatcher"/> extensions.
    /// </summary>
    public static class AttachmentDispatcherExtensions
    {
        /// <summary>
        /// Dispatches an exception.
        /// </summary>
        /// <param name="dispatcher">An dispatcher.</param>
        /// <param name="id">An correlation id.</param>
        /// <param name="content">An exception.</param>
        /// <remarks>This method may not break because it is called from EventSources.</remarks>
        public static void Dispatch(this AttachmentDispatcher dispatcher, AttachmentId id,
            Exception content)
        {
            dispatcher.DispatchAs<ExceptionAttachment>(id, content);
        }

        /// <summary>
        /// Dispatches an object.
        /// </summary>
        /// <param name="dispatcher">An dispatcher.</param>
        /// <param name="id">An correlation id.</param>
        /// <param name="content">An object.</param>
        /// <remarks>This method may not break because it is called from EventSources.</remarks>
        public static void Dispatch(this AttachmentDispatcher dispatcher, AttachmentId id,
            object content)
        {
            dispatcher.DispatchAs<ObjectAttachment>(id, content);
        }

        /// <summary>
        /// Dispatches an object.
        /// </summary>
        /// <typeparam name="TAttachment">The expected attachment type.</typeparam>
        /// <param name="dispatcher">An dispatcher.</param>
        /// <param name="id">An correlation id.</param>
        /// <param name="content">An object.</param>
        /// <remarks>This method may not break because it is called from EventSources.</remarks>
        public static void DispatchAs<TAttachment>(this AttachmentDispatcher dispatcher,
            AttachmentId id, object content)
                where TAttachment : class, IAttachment, new()
        {
            if (dispatcher != null && id != AttachmentId.Empty && content != null)
            {
                TAttachment attachment = Activator.CreateInstance<TAttachment>();

                attachment.Id = id;
                attachment.Content = content.SerializeAttachmentContent();

                dispatcher.Dispatch(attachment);
            }
        }

        /// <summary>
        /// Dispatches an object.
        /// </summary>
        /// <typeparam name="TAttachment">The expected attachment type.</typeparam>
        /// <param name="dispatcher">An dispatcher.</param>
        /// <param name="id">An correlation id.</param>
        /// <param name="content">A content stream.</param>
        /// <remarks>This method may not break because it is called from EventSources.</remarks>
        public static void DispatchAs<TAttachment>(this AttachmentDispatcher dispatcher,
            AttachmentId id, Stream content)
                where TAttachment : class, IAttachment, new()
        {
            if (dispatcher != null && id != AttachmentId.Empty && content?.Length > 0)
            {
                using (StreamReader reader = new StreamReader(content.SetToStart()))
                {
                    TAttachment attachment = Activator.CreateInstance<TAttachment>();

                    attachment.Id = id;
                    attachment.Content = reader.ReadToEnd().SerializeAttachmentContent();

                    dispatcher.Dispatch(attachment);
                }
            }
        }
    }
}