using System.Collections.Generic;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// Describe assemblies in where are event sources.
    /// </summary>
    public interface IProvidersDescriptor
    {
        /// <summary>
        /// List with assembly full name
        /// </summary>
        IEnumerable<string> Assemblies { get; }
    }
}