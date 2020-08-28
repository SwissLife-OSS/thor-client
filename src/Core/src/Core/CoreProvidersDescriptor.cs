using System.Collections.Generic;
using Thor.Core.Abstractions;

namespace Thor.Core
{
    /// <summary>
    /// Core event sources location
    /// </summary>
    public class CoreProvidersDescriptor : IProvidersDescriptor
    {
        /// <inheritdoc cref="IProvidersDescriptor"/>
        public IEnumerable<string> Assemblies { get; } =
            new[] {typeof(CoreProvidersDescriptor).Assembly.FullName};
    }
}