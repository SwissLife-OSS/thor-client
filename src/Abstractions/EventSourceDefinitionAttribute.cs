using System;

namespace ChilliCream.Logging.Abstractions
{
    // todo: Improve documentation

    /// <summary>
    /// Allows the event tracing for Windows (ETW) name to be defined independently of the name of the event source interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class EventSourceDefinitionAttribute
        : Attribute
    {
        /// <summary>
        /// Gets or sets the event source identifier.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the name of the localization resource file.
        /// </summary>
        public string LocalizationResources { get; set; }

        /// <summary>
        /// Gets or sets the name of the event source.
        /// </summary>
        public string Name { get; set; }
    }
}