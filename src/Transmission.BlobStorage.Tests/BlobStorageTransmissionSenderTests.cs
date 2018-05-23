using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.BlobStorage.Tests
{
    public class BlobStorageTransmissionSenderTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for container")]
        public void Constructor_ContainerNull()
        {
            // assert
            CloudBlobContainer container = null;

            // act
            Action verify = () => new BlobStorageTransmissionSender(container);

            // arrange
            Assert.Throws<ArgumentNullException>("container", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            CloudBlobContainer container = CloudStorageAccount
                .Parse(Constants.FakeConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(Guid.NewGuid().ToString());

            // act
            Action verify = () => new BlobStorageTransmissionSender(container);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region SendAsync

        [Fact(DisplayName = "SendAsync: Should throw an argument null exception for batch")]
        public async Task SendAsync_BatchNull()
        {
            // arrange
            CloudBlobContainer container = CloudStorageAccount
                .Parse(Constants.FakeConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(Guid.NewGuid().ToString());
            BlobStorageTransmissionSender sender = new BlobStorageTransmissionSender(container);
            AttachmentDescriptor[] batch = null;

            // act
            Func<Task> verify = () => sender.SendAsync(batch);

            // assert
            await Assert.ThrowsAsync<ArgumentNullException>("batch", verify).ConfigureAwait(false);
        }

        [Fact(DisplayName = "SendAsync: Should throw an argument out of range exception for batch")]
        public async Task SendAsync_BatchOutOfRange()
        {
            // arrange
            CloudBlobContainer container = CloudStorageAccount
                .Parse(Constants.FakeConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(Guid.NewGuid().ToString());
            BlobStorageTransmissionSender sender = new BlobStorageTransmissionSender(container);
            AttachmentDescriptor[] batch = new AttachmentDescriptor[0];

            // act
            Func<Task> verify = () => sender.SendAsync(batch);

            // assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>("batch", verify).ConfigureAwait(false);
        }

        [Fact(DisplayName = "SendAsync: Should not throw any exception")]
        public async Task SendAsync_NoException()
        {
            // assert
            CloudBlobContainer container = CloudStorageAccount
                .Parse(Constants.FakeConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(Guid.NewGuid().ToString());
            BlobStorageTransmissionSender sender = new BlobStorageTransmissionSender(container);
            AttachmentDescriptor[] batch = new[] { new AttachmentDescriptor() };

            // act
            Func<Task> verify = () => sender.SendAsync(batch);

            // arrange
            Assert.Null(await Record.ExceptionAsync(verify).ConfigureAwait(false));
        }

        #endregion
    }
}