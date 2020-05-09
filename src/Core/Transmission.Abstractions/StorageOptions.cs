namespace Thor.Core.Transmission.EventHub
{
    /// <summary> 
    /// Options for storage throttling
    /// </summary>
    public class StorageOptions
    {
        /// <summary>
        /// Size of the dequeue batch from storage.
        /// </summary>
        public int DequeueBatchSize { get; set; } = 100;
    }
}
