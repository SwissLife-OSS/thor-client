using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace Thor.Core.Session
{
    /// <summary>
    /// A concrete configuration for the telemetry event session.
    /// </summary>
    public class SessionConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionConfiguration"/> class.
        /// </summary>
        public SessionConfiguration()
        {
            Debug = Debugger.IsAttached;
            InProcess = true;
        }

        /// <summary>
        /// Gets or sets the application's identifier.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application runs in debug mode.
        /// The default value is <c>true</c> if the debugger is attached; otherwise <c>false</c>.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tracing session runs <c>in-process</c> or
        /// <c>out-of-process</c> mode. The default value is <c>true</c>.
        /// </summary>
        public bool InProcess { get; set; }

        /// <summary>
        /// Gets or sets the tracing severity.
        /// </summary>
        public EventLevel Level { get; set; }
    }
}