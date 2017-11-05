using System;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A marker interface for all types of activity.
    /// </summary>
    public interface IActivity
        : IDisposable
    {
        /// <summary>
        /// Gets the current activity identifier.
        /// </summary>
        Guid Id { get; }
    }
}