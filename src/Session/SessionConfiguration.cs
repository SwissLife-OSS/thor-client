using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Thor.Core.Session
{
    /// <summary>
    /// A concrete configuration for the telemetry event session.
    /// </summary>
    public class SessionConfiguration
    {
        /// <summary>
        /// Gets or sets allowed assembly name prefixes.
        /// </summary>
        public IEnumerable<string> AllowedPrefixes { get; set; }

        /// <summary>
        /// Gets or sets the application's identifier.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the tracing severity.
        /// </summary>
        public EventLevel Level { get; set; }
    }
}