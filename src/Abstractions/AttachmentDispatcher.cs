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
        public static AttachmentDispatcher Instance { get; } = new AttachmentDispatcher();

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
        /// Dispatches one or more attachments.
        /// </summary>
        /// <param name="attachments">A collection of attachments.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="attachments"/> is null.
        /// </exception>
        public void Dispatch(params IAttachment[] attachments)
        {
            if (attachments == null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }

            if (attachments.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attachments));
            }

            AttachmentDescriptor[] descriptors = new AttachmentDescriptor[attachments.Length];

            for (int i = 0; i < attachments.Length; i++)
            {
                descriptors[i] = new AttachmentDescriptor
                {
                    Id = attachments[i].Id,
                    Name = attachments[i].Name,
                    TypeName = attachments[i].GetTypeName(),
                    Value = attachments[i].Value
                };
            }

            foreach (Action<AttachmentDescriptor> observer in _observers)
            {
                foreach (AttachmentDescriptor descriptor in descriptors)
                {
                    observer(descriptor);
                }
            }
        }
    }
}