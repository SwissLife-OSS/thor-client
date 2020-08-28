using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A transmission storage for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public class BlobStorageTransmissionStorage
        : FileStorage<AttachmentDescriptor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmissionStorage"/> class.
        /// </summary>
        /// <param name="storagePath">A storage path to save temporarily.</param>
        public BlobStorageTransmissionStorage(string storagePath)
            : base(storagePath)
        {
        }

        /// <inheritdoc/>
        protected override AttachmentDescriptor Deserialize(byte[] payload, string fileName)
        {
            string[] fileNameParts = fileName.Split('_');

            return new AttachmentDescriptor
            {
                Id = fileNameParts[0],
                Name = fileNameParts[1],
                TypeName = fileNameParts[2],
                Value = payload
            };
        }

        /// <inheritdoc/>
        protected override byte[] Serialize(AttachmentDescriptor data)
        {
            return data.Value;
        }

        /// <inheritdoc/>
        protected override string EncodeFileName(AttachmentDescriptor data)
        {
            return $"{data.Id}_{data.Name}_{data.TypeName}";
        }
    }
}
