using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Thor.Core.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.Abstractions.Tests
{
    public class AttachmentDispatcherTests
    {
        #region Attach
        
        [Fact(DisplayName = "Attach: Should throw an argument null excption for transmitter")]
        public void Attach_TransmitterNull()
        {
            // arrange
            ITelemetryAttachmentTransmitter transmitter = null;
            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);

            // act
            Action validate = () => dispatcher.Attach(transmitter);

            // assert
            Assert.Throws<ArgumentNullException>("transmitter", validate);
        }

        [Fact(DisplayName = "Attach: Should attach an transmitter")]
        public void Attach_Success()
        {
            // arrange
            int callCount = 0;
            Mock<IAttachment> attachment = new Mock<IAttachment>();
            Mock<ITelemetryAttachmentTransmitter> transmitter = new Mock<ITelemetryAttachmentTransmitter>();

            transmitter
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback(() => Interlocked.Increment(ref callCount));

            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);

            // act
            dispatcher.Attach(transmitter.Object);

            // assert
            dispatcher.Dispatch(attachment.Object);

            Assert.Equal(1, callCount);
        }

        #endregion

        #region Detach

        [Fact(DisplayName = "Detach: Should throw an argument null excption for transmitter")]
        public void Detach_TransmitterNull()
        {
            // arrange
            ITelemetryAttachmentTransmitter transmitter = null;
            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);

            // act
            Action validate = () => dispatcher.Detach(transmitter);

            // assert
            Assert.Throws<ArgumentNullException>("transmitter", validate);
        }

        [Fact(DisplayName = "Detach: Should detach an transmitter")]
        public void Detach_Success()
        {
            // arrange
            int callCount = 0;
            Mock<IAttachment> attachment = new Mock<IAttachment>();
            Mock<ITelemetryAttachmentTransmitter> transmitter = new Mock<ITelemetryAttachmentTransmitter>();

            transmitter
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback(() => Interlocked.Increment(ref callCount));

            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>
            {
                transmitter.Object
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);

            // act
            dispatcher.Detach(transmitter.Object);

            // assert
            dispatcher.Dispatch(attachment.Object);

            Assert.Equal(0, callCount);
        }

        #endregion

        #region Dispatch

        [Fact(DisplayName = "Dispatch: Should not throw an argument null excption for attachment")]
        public void Dispatch_AttachmentsNull()
        {
            // arrange
            IAttachment[] attachments = null;
            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);

            // act
            Action validate = () => dispatcher.Dispatch(attachments);

            // assert
            AssertEx.NotThrow(validate);
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument out of range excption for attachment")]
        public void Dispatch_AttachmentsEmpty()
        {
            // arrange
            IAttachment[] attachments = new IAttachment[0];
            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>();
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);

            // act
            Action validate = () => dispatcher.Dispatch(attachments);

            // assert
            AssertEx.NotThrow(validate);
        }

        [Fact(DisplayName = "Dispatch: Should dispatch a single attachment")]
        public void Dispatch_Single_Success()
        {
            // arrange
            int callCount = 0;
            IAttachment attachment = new Mock<IAttachment>().Object;
            Mock<ITelemetryAttachmentTransmitter> transmitter = new Mock<ITelemetryAttachmentTransmitter>();

            transmitter
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback(() => Interlocked.Increment(ref callCount));

            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>
            {
                transmitter.Object
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);

            // act
            dispatcher.Dispatch(attachment);

            // assert
            Assert.Equal(1, callCount);
        }

        [Fact(DisplayName = "Dispatch: Should dispatch multiple attachments")]
        public void Dispatch_Multiple_Success()
        {
            // arrange
            int callCount = 0;
            IAttachment[] attachments = new[]
            {
                new Mock<IAttachment>().Object,
                new Mock<IAttachment>().Object,
                new Mock<IAttachment>().Object
            };
            Mock<ITelemetryAttachmentTransmitter> transmitter = new Mock<ITelemetryAttachmentTransmitter>();

            transmitter
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback(() => Interlocked.Increment(ref callCount));

            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>
            {
                transmitter.Object
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);

            // act
            dispatcher.Dispatch(attachments);

            // assert
            Assert.Equal(3, callCount);
        }

        #endregion

        private static class AssertEx
        {
            public static void NotThrow(Action action)
            {
                try
                {
                    action();
                }
                catch
                {
                    Assert.False(true, "Expected no to be thrown");
                }
            }
        }
    }
}
