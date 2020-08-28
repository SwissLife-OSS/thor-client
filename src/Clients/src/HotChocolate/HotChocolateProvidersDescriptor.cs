using System.Collections.Generic;
using Thor.Core.Abstractions;

namespace Thor.Extensions.HotChocolate
{
    /// <summary>
    /// HotChocolate event sources location
    /// </summary>
    public class HotChocolateProvidersDescriptor : IProvidersDescriptor
    {
        /// <inheritdoc cref="IProvidersDescriptor"/>
        public IEnumerable<string> Assemblies { get; } =
            new[] {typeof(HotChocolateProvidersDescriptor).Assembly.FullName};
    }
}