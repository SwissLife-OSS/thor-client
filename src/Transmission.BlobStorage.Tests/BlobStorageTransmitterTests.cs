using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.BlobStorage.Tests
{
    public class BlobStorageTransmitterTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for buffer")]
        public void Constructor_BufferNull()
        {
            // assert
            ITransmissionBuffer<AttachmentDescriptor> buffer = null;
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;

            // act
            Action verify = () => new BlobStorageTransmitter(buffer, sender);

            // arrange
            Assert.Throws<ArgumentNullException>("buffer", verify);
        }

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for sender")]
        public void Constructor_SenderNull()
        {
            // assert
            ITransmissionBuffer<AttachmentDescriptor> buffer = new Mock<ITransmissionBuffer<AttachmentDescriptor>>().Object;
            ITransmissionSender<AttachmentDescriptor> sender = null;

            // act
            Action verify = () => new BlobStorageTransmitter(buffer, sender);

            // arrange
            Assert.Throws<ArgumentNullException>("sender", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            ITransmissionBuffer<AttachmentDescriptor> buffer = new Mock<ITransmissionBuffer<AttachmentDescriptor>>().Object;
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;

            // act
            Action verify = () => new BlobStorageTransmitter(buffer, sender);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region Enqueue

        [Fact(DisplayName = "Enqueue: Should throw an argument null exception for data")]
        public void Enqueue_DataNull()
        {
            // arrange
            ITransmissionBuffer<AttachmentDescriptor> buffer = new Mock<ITransmissionBuffer<AttachmentDescriptor>>().Object;
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;
            ITelemetryAttachmentTransmitter transmitter = new BlobStorageTransmitter(buffer, sender);
            AttachmentDescriptor data = null;

            // act
            Action verify = () => transmitter.Enqueue(data);

            // assert
            Assert.Throws<ArgumentNullException>("data", verify);
        }

        [Fact(DisplayName = "Enqueue: Should not throw any exception")]
        public void Enqueue_NoException()
        {
            // assert
            ITransmissionBuffer<AttachmentDescriptor> buffer = new Mock<ITransmissionBuffer<AttachmentDescriptor>>().Object;
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;
            ITelemetryAttachmentTransmitter transmitter = new BlobStorageTransmitter(buffer, sender);
            AttachmentDescriptor data = new Mock<AttachmentDescriptor>().Object;

            // act
            Action verify = () => new BlobStorageTransmitter(buffer, sender);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Enqueue: Should enqueue data and transfer it")]
        public void Enqueue_TransmissionFlow()
        {
            // assert
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            Mock<ITransmissionBuffer<AttachmentDescriptor>> buffer = new Mock<ITransmissionBuffer<AttachmentDescriptor>>();
            Mock<ITransmissionSender<AttachmentDescriptor>> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>();
            ConcurrentQueue<AttachmentDescriptor> bufferQueue = new ConcurrentQueue<AttachmentDescriptor>();

            buffer
                .Setup(t => t.EnqueueAsync(It.IsAny<AttachmentDescriptor>(), It.IsAny<CancellationToken>()))
                .Callback((AttachmentDescriptor d, CancellationToken t) => bufferQueue.Enqueue(d));
            buffer
                .Setup(t => t.DequeueAsync(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    int count = 0;
                    List<AttachmentDescriptor> results = new List<AttachmentDescriptor>();

                    while (bufferQueue.TryDequeue(out AttachmentDescriptor d) && count < 10)
                    {
                        results.Add(d);
                        count++;
                    }

                    return Task.FromResult(results.ToArray());
                });
            sender
                .Setup(t => t.SendAsync(It.IsAny<AttachmentDescriptor[]>(), It.IsAny<CancellationToken>()))
                .Callback(() => resetEvent.Set());

            ITelemetryAttachmentTransmitter transmitter = new BlobStorageTransmitter(buffer.Object, sender.Object);
            AttachmentDescriptor data = new Mock<AttachmentDescriptor>().Object;

            // act
            transmitter.Enqueue(data);

            // arrange
            resetEvent.Wait(TimeSpan.FromSeconds(5));

            sender.Verify((ITransmissionSender<AttachmentDescriptor> s) =>
                s.SendAsync(It.Is((AttachmentDescriptor[] d) => d.Length == 1),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Dispose

        [Fact(DisplayName = "Dispose: Should work like a charm")]
        public void Dispose()
        {
            // arrange
            ITransmissionBuffer<AttachmentDescriptor> buffer = new Mock<ITransmissionBuffer<AttachmentDescriptor>>().Object;
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;
            BlobStorageTransmitter transmitter = new BlobStorageTransmitter(buffer, sender);

            // act
            Action verify = () => transmitter.Dispose();

            // assert
            Assert.Null(Record.Exception(verify));
        }

        #endregion
    }
}