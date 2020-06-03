using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
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
            IBlobContainer container = null;

            // act
            Action verify = () => new BlobStorageTransmissionSender(container);

            // arrange
            Assert.Throws<ArgumentNullException>("container", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            IBlobContainer container = new Mock<IBlobContainer>().Object;

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
            Mock<IBlobContainer> container = new Mock<IBlobContainer>();

            container
                .Setup(t => t.UploadAsync(It.IsAny<AttachmentDescriptor>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            BlobStorageTransmissionSender sender = new BlobStorageTransmissionSender(container.Object);
            IAsyncEnumerable<AttachmentDescriptor> batch = null;

            // act
            Func<Task> verify = () => sender.SendAsync(batch, default);

            // assert
            await Assert.ThrowsAsync<ArgumentNullException>("batch", verify).ConfigureAwait(false);
        }

        [Fact(DisplayName = "SendAsync: Should not send when empty batch")]
        public async Task SendAsync_BatchOutOfRange()
        {
            // arrange
            Mock<IBlobContainer> container = new Mock<IBlobContainer>();

            container
                .Setup(t => t.UploadAsync(It.IsAny<AttachmentDescriptor>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            BlobStorageTransmissionSender sender = new BlobStorageTransmissionSender(container.Object);
            IAsyncEnumerable<AttachmentDescriptor> batch = new AttachmentDescriptor[0].ToAsyncEnumerable();

            // act
            await sender.SendAsync(batch, default);

            // assert
            container.Verify(c => c.UploadAsync(It.IsAny<AttachmentDescriptor>(), default), Times.Never);
        }

        [Fact(DisplayName = "SendAsync: Should not throw any exception")]
        public async Task SendAsync_NoException()
        {
            // assert
            Mock<IBlobContainer> container = new Mock<IBlobContainer>();

            container
                .Setup(t => t.UploadAsync(It.IsAny<AttachmentDescriptor>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            BlobStorageTransmissionSender sender = new BlobStorageTransmissionSender(container.Object);
            IAsyncEnumerable<AttachmentDescriptor> batch = new[] { new AttachmentDescriptor() }.ToAsyncEnumerable();

            // act
            Func<Task> verify = () => sender.SendAsync(batch, default);

            // arrange
            Assert.Null(await Record.ExceptionAsync(verify).ConfigureAwait(false));
        }

        #endregion
    }
}
