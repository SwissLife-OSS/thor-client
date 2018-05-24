using System;
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
            string storagePath = "C:\\DequeueAsync_NoException_Test";
            BlobStorageTransmissionStorage storage = new BlobStorageTransmissionStorage(storagePath);

            // act
            Func<Task> verify = () => storage.DequeueAsync();

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
            Func<Task> verify = () => storage.EnqueueAsync(batch);

            // assert
            await Assert.ThrowsAsync<ArgumentNullException>("batch", verify).ConfigureAwait(false);
        }

        [Fact(DisplayName = "EnqueueAsync: Should throw an argument out of range exception for batch")]
        public async Task EnqueueAsync_BatchOutOfRange()
        {
            // arrange
            string storagePath = "C:\\EnqueueAsync_BatchOutOfRange_Test";
            BlobStorageTransmissionStorage storage = new BlobStorageTransmissionStorage(storagePath);
            AttachmentDescriptor[] batch = new AttachmentDescriptor[0];

            // act
            Func<Task> verify = () => storage.EnqueueAsync(batch);

            // assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>("batch", verify).ConfigureAwait(false);
        }

        #endregion
    }
}