namespace Thor.Core.Abstractions
{
    /// <summary>
    /// Describe a contract for a diagnostic source listener
    /// </summary>
    public interface IDiagnosticsListener
    {
        /// <summary>
        /// Name of the diagnostic listener
        /// </summary>
        string Name { get; }
    }
}