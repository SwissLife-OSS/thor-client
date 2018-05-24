namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A concrete configuration for <c>Azure BLOB Storage</c> transmission.
    /// </summary>
    public class BlobStorageConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageConfiguration"/> class.
        /// </summary>
        public BlobStorageConfiguration()
        {
            AttachmentContainerName = "attachments";
        }

        /// <summary>
        /// Gets or sets the <c>Azure BLOB Storage</c> container name for attachments.
        /// </summary>
        public string AttachmentContainerName { get; set; }

        /// <summary>
        /// Gets or sets the <c>Azure BLOB Storage</c> connection string.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}