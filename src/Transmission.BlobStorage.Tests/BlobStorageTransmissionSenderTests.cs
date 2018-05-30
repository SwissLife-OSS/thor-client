using System;
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
            Mock<IBlobContainer> container = new Mock<IBlobContainer>();

            container
                .Setup(t => t.UploadAsync(It.IsAny<AttachmentDescriptor>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            BlobStorageTransmissionSender sender = new BlobStorageTransmissionSender(container.Object);
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
            Mock<IBlobContainer> container = new Mock<IBlobContainer>();

            container
                .Setup(t => t.UploadAsync(It.IsAny<AttachmentDescriptor>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            BlobStorageTransmissionSender sender = new BlobStorageTransmissionSender(container.Object);
            AttachmentDescriptor[] batch = new[] { new AttachmentDescriptor() };

            // act
            Func<Task> verify = () => sender.SendAsync(batch);

            // arrange
            Assert.Null(await Record.ExceptionAsync(verify).ConfigureAwait(false));
        }

        #endregion
    }
}