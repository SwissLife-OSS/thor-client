using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.BlobStorage.Tests
{
    public class BlobContainerTests
    {
        [Fact(DisplayName = "Constructor: Should throw an argument null exception for container")]
        public void Constructor_ContainerNull()
        {
            // assert
            CloudBlobContainer container = null;

            // act
            Action verify = () => new BlobContainer(container);

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
                .GetContainerReference("test-456");

            // act
            Action verify = () => new BlobContainer(container);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "UploadAsync: Should throw an argument null exception for descriptor")]
        public async Task UploadAsync_DescriptorNull()
        {
            // arrange
            CloudBlobContainer reference = CloudStorageAccount
                .Parse(Constants.FakeConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference("test-456");
            BlobContainer container = new BlobContainer(reference);
            AttachmentDescriptor descriptor = null;

            // act
            Func<Task> verify = () => container.UploadAsync(descriptor);

            // arrange
            await Assert.ThrowsAsync<ArgumentNullException>("descriptor", verify).ConfigureAwait(false);
        }
    }
}