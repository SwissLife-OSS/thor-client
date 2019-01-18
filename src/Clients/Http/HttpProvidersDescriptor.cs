using System.Collections.Generic;
using Thor.Core.Abstractions;

namespace Thor.Core.Http
{
    /// <summary>
    /// Http event sources location
    /// </summary>
    public class HttpProvidersDescriptor : IProvidersDescriptor
    {
        /// <inheritdoc cref="IProvidersDescriptor"/>
        public IEnumerable<string> Assemblies { get; } =
            new[] {typeof(HttpProvidersDescriptor).Assembly.FullName};
    }
}