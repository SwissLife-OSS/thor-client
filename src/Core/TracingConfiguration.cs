using System.Diagnostics;
using System.IO;

namespace Thor.Core
{
    /// <summary>
    /// A common configuration for tracing.
    /// </summary>
    public class TracingConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TracingConfiguration"/> class.
        /// </summary>
        public TracingConfiguration()
        {
            ApplicationRootPath = Directory.GetCurrentDirectory();
            Debug = Debugger.IsAttached;
            InProcess = true;
        }

        /// <summary>
        /// Gets or sets the application root folder path. The default value is reading its value
        /// from <see cref="Directory.GetCurrentDirectory()"/>.
        /// </summary>
        public string ApplicationRootPath { get; set; }

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
    }
}