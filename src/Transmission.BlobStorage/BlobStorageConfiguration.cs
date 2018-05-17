namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A concrete configuration for <c>Azure BLOB Storage</c> transmission.
    /// </summary>
    public class BlobStorageConfiguration
    {
        /// <summary>
        /// Gets or sets the <c>Azure BLOB Storage</c> connection string.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}