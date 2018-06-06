using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.BlobStorage.Tests
{
    public class BlobContainerExtensionsTests
    {
        #region UploadAsync

        [Fact(DisplayName = "UploadAsync: Should throw an argument null exception for container")]
        public async Task UploadAsync_ContainerNull()
        {
            // arrange
            IBlobContainer container = null;
            AttachmentDescriptor descriptor = new AttachmentDescriptor();

            // act
            Func<Task> verify = () => container.UploadAsync(descriptor);

            // arrange
            await Assert.ThrowsAsync<ArgumentNullException>("container", verify).ConfigureAwait(false);
        }

        [Fact(DisplayName = "UploadAsync: Should not throw any exception")]
        public async Task UploadAsync_NoException()
        {
            // arrange
            Mock<IBlobContainer> container = new Mock<IBlobContainer>();

            container
                .Setup(t => t.UploadAsync(It.IsAny<AttachmentDescriptor>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            AttachmentDescriptor descriptor = new AttachmentDescriptor();

            // act
            Func<Task> verify = () => container.Object.UploadAsync(descriptor);

            // assert
            Assert.Null(await Record.ExceptionAsync(verify).ConfigureAwait(false));
        }

        #endregion
    }
}