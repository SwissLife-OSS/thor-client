using System;
using System.IO;
using System.Threading.Tasks;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.BlobStorage.Tests
{
    public class BlobStorageTransmissionStorageTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for storagePath")]
        public void Constructor_StoragePathNull()
        {
            // assert
            string storagePath = null;

            // act
            Action verify = () => new BlobStorageTransmissionStorage(storagePath);

            // arrange
            Assert.Throws<ArgumentNullException>("storagePath", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            string storagePath = "C:\\Constructor_NoException_Test";

            // act
            Action verify = () => new BlobStorageTransmissionStorage(storagePath);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region DequeueAsync

        [Fact(DisplayName = "DequeueAsync: Should not throw any exception")]
        public async Task DequeueAsync_NoException()
        {
            // assert
            string storagePath = "DequeueAsync_NoException_Test";
            Directory.CreateDirectory(storagePath);
            await File.WriteAllTextAsync(Path.Combine(storagePath, "20200602-1a67cab4539a43ee9416b82657609236_info_Object.tmp"), "test");
            BlobStorageTransmissionStorage storage = new BlobStorageTransmissionStorage(storagePath);

            // act
            Func<Task> verify = () => storage.DequeueAsync(1, default);

            // arrange
            Assert.Null(await Record.ExceptionAsync(verify).ConfigureAwait(false));
        }

        #endregion

        #region EnqueueAsync

        [Fact(DisplayName = "EnqueueAsync: Should throw an argument null exception for batch")]
        public async Task EnqueueAsync_BatchNull()
        {
            // arrange
            string storagePath = "C:\\EnqueueAsync_BatchNull_Test";
            BlobStorageTransmissionStorage storage = new BlobStorageTransmissionStorage(storagePath);
            AttachmentDescriptor[] batch = null;

            // act
            Func<Task> verify = () => storage.EnqueueAsync(batch, default);

            // assert
            await Assert.ThrowsAsync<ArgumentNullException>("batch", verify).ConfigureAwait(false);
        }

        #endregion
    }
}
