using System;
using System.Collections.Generic;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A dispatcher for attachments.
    /// </summary>
    public class AttachmentDispatcher
    {
        private static readonly object _sync = new object();
        private readonly HashSet<Action<AttachmentDescriptor>> _observers;

        private AttachmentDispatcher()
            : this(new HashSet<Action<AttachmentDescriptor>>())
        { }

        internal AttachmentDispatcher(HashSet<Action<AttachmentDescriptor>> observers)
        {
            _observers = observers ?? throw new ArgumentNullException(nameof(observers));
        }

        /// <summary>
        /// Gets a singelton instance of <see cref="AttachmentDispatcher"/>.
        /// </summary>
        public static readonly AttachmentDispatcher Instance = new AttachmentDispatcher();

        /// <summary>
        /// Attaches an observer for <see cref="AttachmentDescriptor"/> changes.
        /// </summary>
        /// <param name="observer">An observer.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="observer"/> is null.
        /// </exception>
        public void Attach(Action<AttachmentDescriptor> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (_sync)
            {
                _observers.Add(observer);
            }
        }

        /// <summary>
        /// Detaches an observer for <see cref="AttachmentDescriptor"/> changes.
        /// </summary>
        /// <param name="observer">An observer.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="observer"/> is null.
        /// </exception>
        public void Detach(Action<AttachmentDescriptor> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (_sync)
            {
                _observers.Remove(observer);
            }
        }

        /// <summary>
        /// Dispatches an attachment.
        /// </summary>
        /// <param name="attachment">An attachment.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="attachment"/> is null.
        /// </exception>
        public void Dispatch(IAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            AttachmentDescriptor descriptor = new AttachmentDescriptor
            {
                Id = attachment.Id.ToString(),
                TypeName = attachment.GetTypeName(),
                Content = attachment.Content
            };

            foreach (Action<AttachmentDescriptor> observer in _observers)
            {
                observer(descriptor);
            }
        }
    }
}