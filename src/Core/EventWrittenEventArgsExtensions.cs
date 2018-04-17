using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace Thor.Core
{
    /// <summary>
    /// A bunch of convenient <see cref="EventWrittenEventArgs"/> extension methods.
    /// </summary>
    public static class EventWrittenEventArgsExtensions
    {
        private static HashSet<string> _wellKnownPayloadNames = new HashSet<string>
        {
            WellKnownPayloadNames.ActivityId,
            WellKnownPayloadNames.ApplicationId,
            WellKnownPayloadNames.AttachmentId,
            WellKnownPayloadNames.Message,
            WellKnownPayloadNames.UserId
        };

        /// <summary>
        /// Gets the activity id stored in the payload.
        /// </summary>
        /// <param name="source">An event to get the activity id from.</param>
        /// <returns>An activity id; otherwise empty.</returns>
        public static Guid GetActivityId(this EventWrittenEventArgs source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int index = source.PayloadNames.ToArray().IndexOfName(WellKnownPayloadNames.ActivityId);
            object payload = (index >= 0) ? source.Payload.ToArray()[index] : null;

            if (payload != null && payload is Guid)
            {
                return (Guid)payload;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Gets the application id stored in the payload.
        /// </summary>
        /// <param name="source">An event to get the application id from.</param>
        /// <returns>An application id; otherwise empty.</returns>
        public static int GetApplicationId(this EventWrittenEventArgs source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int index = source.PayloadNames.ToArray().IndexOfName(WellKnownPayloadNames.ApplicationId);
            object payload = (index >= 0) ? source.Payload.ToArray()[index] : null;

            return payload as int? ?? 0;
        }

        /// <summary>
        /// Gets the attachment id stored in the payload.
        /// </summary>
        /// <param name="source">An event to get the attachment id from.</param>
        /// <returns>An attachment id; otherwise empty.</returns>
        public static string GetAttachmentId(this EventWrittenEventArgs source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int index = source.PayloadNames.ToArray().IndexOfName(WellKnownPayloadNames.AttachmentId);
            object payload = (index >= 0) ? source.Payload.ToArray()[index] : null;

            return payload as string ?? default(string);
        }

        /// <summary>
        /// Gets the formatted message stored in the payload.
        /// </summary>
        /// <param name="source">An event to get the formatted message from.</param>
        /// <returns>A formatted message; otherwise empty.</returns>
        public static string GetFormattedMessage(this EventWrittenEventArgs source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Payload == null || source.Payload.Count == 0)
            {
                return source.Message;
            }

            return string.Format(source.Message, source.Payload.ToArray());
        }

        /// <summary>
        /// Gets the user id stored in the payload.
        /// </summary>
        /// <param name="source">An event to get the user id from.</param>
        /// <returns>An user id; otherwise empty.</returns>
        public static Guid? GetUserId(this EventWrittenEventArgs source)
        {
            int index = source.PayloadNames.ToArray().IndexOfName(WellKnownPayloadNames.UserId);
            object payload = (index >= 0) ? source.Payload.ToArray()[index] : null;

            if (payload != null && payload is Guid && (Guid)payload != Guid.Empty)
            {
                return (Guid)payload;
            }

            return null;
        }

        private static int IndexOfName(this string[] names, string name)
        {
            int index = 0;

            foreach (string current in names)
            {
                if (current == name)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }
}