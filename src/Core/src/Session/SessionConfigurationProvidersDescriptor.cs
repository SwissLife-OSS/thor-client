using System;
using System.Collections.Generic;
using System.Linq;
using Thor.Core.Abstractions;

namespace Thor.Core.Session
{
    /// <summary>
    /// Event sources location from configuration
    /// </summary>
    public class SessionConfigurationProvidersDescriptor : IProvidersDescriptor
    {
        /// <summary>
        /// Creates a new instance of <see cref="SessionConfigurationProvidersDescriptor"/>
        /// </summary>
        /// <param name="sessionConfiguration">
        /// Configuration for the telemetry event session.
        /// </param>
        public SessionConfigurationProvidersDescriptor(
            SessionConfiguration sessionConfiguration)
        {
            if (sessionConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sessionConfiguration));
            }

            Assemblies = sessionConfiguration.AllowedPrefixes ??
                Enumerable.Empty<string>();
        }

        /// <inheritdoc cref="IProvidersDescriptor"/>
        public IEnumerable<string> Assemblies { get; }
    }
}
