using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
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
        /// Maps a <see cref="EventWrittenEventArgs"/> to a <see cref="TelemetryEvent"/>.
        /// </summary>
        /// <param name="source">An event to map.</param>
        /// <param name="sessionName">A session name to add to the destination object.</param>
        /// <returns>A instance of <see cref="TelemetryEvent"/>.</returns>
        public static TelemetryEvent Map(this EventWrittenEventArgs source, string sessionName)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrWhiteSpace(sessionName))
            {
                throw new ArgumentNullException(nameof(sessionName));
            }

            TelemetryEvent destination = new TelemetryEvent
            {
                ApplicationId = source.GetApplicationId(),
                AttachmentId = source.GetAttachmentId(),
                Channel = (int)source.Channel,
                Id = source.EventId,
                Level = source.Level,
                Message = source.GetFormattedMessage(),
                Name = source.EventName,
                Payload = source.GetPayloads(),
                ProcessId = 0,
                ProcessName = null,
                ProviderId = source.EventSource.Guid,
                ProviderName = source.EventSource.Name,
                OpcodeId = (int)source.Opcode,
                OpcodeName = source.Opcode.ToString(),
                ActivityId = source.GetActivityId(),
                RelatedActivityId = source.RelatedActivityId,
                Keywords = source.Keywords,
                SessionName = sessionName,
                TaskId = (int)source.Task,
                TaskName = source.Task.ToString(),
                ThreadId = 0,
                Timestamp = DateTime.UtcNow.Ticks,
                UserId = source.GetUserId(),
                Version = source.Version
            };

            return destination;
        }

        private static Guid GetActivityId(this EventWrittenEventArgs source)
        {
            int index = source.PayloadNames.ToArray().IndexOfName(WellKnownPayloadNames.ActivityId);
            object payload = (index >= 0) ? source.Payload.ToArray()[index] : null;

            if (payload != null && payload is Guid)
            {
                return (Guid)payload;
            }

            return Guid.Empty;
        }

        private static int GetApplicationId(this EventWrittenEventArgs source)
        {
            int index = source.PayloadNames.ToArray().IndexOfName(WellKnownPayloadNames.ApplicationId);
            object payload = (index >= 0) ? source.Payload.ToArray()[index] : null;

            return payload as int? ?? 0;
        }

        private static string GetAttachmentId(this EventWrittenEventArgs source)
        {
            int index = source.PayloadNames.ToArray().IndexOfName(WellKnownPayloadNames.AttachmentId);
            object payload = (index >= 0) ? source.Payload.ToArray()[index] : null;

            return payload as string ?? default(string);
        }

        private static string GetFormattedMessage(this EventWrittenEventArgs source)
        {
            object[] payload = source.Payload?.ToArray();

            if (payload == null || payload.Length == 0)
            {
                return source.Message;
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, source.Message, payload);
            }
        }

        private static Guid? GetUserId(this EventWrittenEventArgs source)
        {
            int index = source.PayloadNames.ToArray().IndexOfName(WellKnownPayloadNames.UserId);
            object payload = (index >= 0) ? source.Payload.ToArray()[index] : null;

            if (payload != null && payload is Guid && (Guid)payload != Guid.Empty)
            {
                return (Guid)payload;
            }

            return null;
        }

        private static Dictionary<string, object> GetPayloads(this EventWrittenEventArgs source)
        {
            // Sort some well known payloads out.
            string[] allNames = source.PayloadNames.ToArray();
            string[] filteredNames = allNames.Where(p => !_wellKnownPayloadNames.Contains(p)).ToArray();
            object[] payloads = source.Payload.ToArray();

            return (filteredNames.Length > 0)
                ? filteredNames.ToDictionary(name => name,
                    name => payloads[allNames.IndexOfName(name)])
                : null;
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