using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using Thor.Core.Transmission.Abstractions;
using Thor.Core.Transmission.EventHub;
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
            IMemoryBuffer<AttachmentDescriptor> buffer = new Mock<IMemoryBuffer<AttachmentDescriptor>>().Object;
            ITransmissionStorage<AttachmentDescriptor> storage = null;
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;

            // act
            Action verify = () => new BlobStorageTransmitter(buffer, storage, sender, new AttachmentsOptions());

            // arrange
            Assert.Throws<ArgumentNullException>("storage", verify);
        }

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for sender")]
        public void Constructor_SenderNull()
        {
            // assert
            IMemoryBuffer<AttachmentDescriptor> buffer = new Mock<IMemoryBuffer<AttachmentDescriptor>>().Object;
            ITransmissionStorage<AttachmentDescriptor> storage = new Mock<ITransmissionStorage<AttachmentDescriptor>>().Object;
            ITransmissionSender<AttachmentDescriptor> sender = null;

            // act
            Action verify = () => new BlobStorageTransmitter(buffer, storage, sender, new AttachmentsOptions());

            // arrange
            Assert.Throws<ArgumentNullException>("sender", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            IMemoryBuffer<AttachmentDescriptor> buffer = new Mock<IMemoryBuffer<AttachmentDescriptor>>().Object;
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = CreateEmptyStorage();
            var sender = new Mock<ITransmissionSender<AttachmentDescriptor>>();

            // act
            Action verify = () => new BlobStorageTransmitter(buffer, storage.Object, sender.Object, new AttachmentsOptions());

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region Enqueue

        [Fact(DisplayName = "Enqueue: Should throw an argument null exception for data")]
        public void Enqueue_DataNull()
        {
            // arrange
            IMemoryBuffer<AttachmentDescriptor> buffer = new Mock<IMemoryBuffer<AttachmentDescriptor>>().Object;
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = CreateEmptyStorage();
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;
            ITelemetryAttachmentTransmitter transmitter = new BlobStorageTransmitter(buffer, storage.Object, sender, new AttachmentsOptions());
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
            IMemoryBuffer<AttachmentDescriptor> buffer = new Mock<IMemoryBuffer<AttachmentDescriptor>>().Object;
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = CreateEmptyStorage();
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;

            // act
            Action verify = () => new BlobStorageTransmitter(buffer, storage.Object, sender, new AttachmentsOptions());

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Enqueue: Should enqueue data and transfer it")]
        public void Enqueue_TransmissionFlow()
        {
            // assert
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            Mock<IMemoryBuffer<AttachmentDescriptor>> buffer = new Mock<IMemoryBuffer<AttachmentDescriptor>>();
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = new Mock<ITransmissionStorage<AttachmentDescriptor>>();
            Mock<ITransmissionSender<AttachmentDescriptor>> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>();
            ConcurrentQueue<AttachmentDescriptor> bufferQueue = new ConcurrentQueue<AttachmentDescriptor>();

            buffer
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback((AttachmentDescriptor d) => bufferQueue.Enqueue(d));
            storage
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

                    return results.ToAsyncEnumerable();
                });
            sender
                .Setup(t => t.SendAsync(It.IsAny<IAsyncEnumerable<AttachmentDescriptor>>(), It.IsAny<CancellationToken>()))
                .Callback(() => resetEvent.Set());

            ITelemetryAttachmentTransmitter transmitter = new BlobStorageTransmitter(
                buffer.Object, storage.Object, sender.Object, new AttachmentsOptions());
            AttachmentDescriptor data = new Mock<AttachmentDescriptor>().Object;

            // act
            transmitter.Enqueue(data);

            // arrange
            resetEvent.Wait(TimeSpan.FromSeconds(5));

            sender.Verify(s =>
                s.SendAsync(It.IsAny<IAsyncEnumerable<AttachmentDescriptor>>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Dispose

        [Fact(DisplayName = "Dispose: Should work like a charm")]
        public void Dispose()
        {
            // arrange
            Mock<IMemoryBuffer<AttachmentDescriptor>> buffer = new Mock<IMemoryBuffer<AttachmentDescriptor>>();
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = CreateEmptyStorage();
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;
            BlobStorageTransmitter transmitter = new BlobStorageTransmitter(
                buffer.Object, storage.Object, sender, new AttachmentsOptions());

            // act
            Action verify = () => transmitter.Dispose();

            // assert
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        private Mock<ITransmissionStorage<AttachmentDescriptor>> CreateEmptyStorage()
        {
            var storage = new Mock<ITransmissionStorage<AttachmentDescriptor>>();
            storage
                .Setup(x => x.DequeueAsync(It.IsAny<CancellationToken>()))
                .Returns(new List<AttachmentDescriptor>().ToAsyncEnumerable());

            return storage;
        }
    }
}
