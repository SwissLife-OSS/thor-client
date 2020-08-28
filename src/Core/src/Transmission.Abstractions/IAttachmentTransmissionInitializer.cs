namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A initializer for attachment transmission.
    /// </summary>
    public interface IAttachmentTransmissionInitializer
    {
        /// <summary>
        /// Setups everything for telemetry attachment transmission.
        /// </summary>
        void Initialize();
    }
}