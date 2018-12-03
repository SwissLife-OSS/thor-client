using System;
using Moq;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.BlobStorage.Tests
{
    public class AttachmentTransmissionInitializerTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for transmitters")]
        public void Constructor_ContainerNull()
        {
            // assert
            ITelemetryAttachmentTransmitter[] transmitters = null;

            // act
            Action verify = () => new AttachmentTransmissionInitializer(
                transmitters);

            // arrange
            Assert.Throws<ArgumentNullException>("transmitters", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            var transmitters = new ITelemetryAttachmentTransmitter[0];

            // act
            Action verify = () => new AttachmentTransmissionInitializer(
                transmitters);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region Initialize

        [Fact(DisplayName = "Initialize: Should attach one transmitter to the attachment dispatcher")]
        public void Initialize_Success()
        {
            // arrange
            var transmitterMock = new Mock<ITelemetryAttachmentTransmitter>();

            transmitterMock.Setup(t =>
                t.Enqueue(It.IsAny<AttachmentDescriptor>()));

            var transmitter = transmitterMock.Object;
            var transmitters = new[] { transmitter };
            var initializer = new AttachmentTransmissionInitializer(transmitters);

            // act
            initializer.Initialize();

            // assert
            var attachment = new ExceptionAttachment
            {
                Id = AttachmentId.NewId(),
                Name = "Foo",
                Value = new byte[] { 1, 2, 3 }
            };

            AttachmentDispatcher.Instance.Dispatch(attachment);

            transmitterMock.Verify(
                t => t.Enqueue(It.IsAny<AttachmentDescriptor>()),
                Times.Once());

            AttachmentDispatcher.Instance.Detach(transmitter);
        }

        #endregion
    }
}