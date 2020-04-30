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
            ITransmissionStorage<AttachmentDescriptor> storage = null;
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;

            // act
            Action verify = () => new BlobStorageTransmitter(storage, sender);

            // arrange
            Assert.Throws<ArgumentNullException>("storage", verify);
        }

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for sender")]
        public void Constructor_SenderNull()
        {
            // assert
            ITransmissionStorage<AttachmentDescriptor> storage = new Mock<ITransmissionStorage<AttachmentDescriptor>>().Object;
            ITransmissionSender<AttachmentDescriptor> sender = null;

            // act
            Action verify = () => new BlobStorageTransmitter(storage, sender);

            // arrange
            Assert.Throws<ArgumentNullException>("sender", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = CreateEmptyStorage();
            var sender = new Mock<ITransmissionSender<AttachmentDescriptor>>();

            // act
            Action verify = () => new BlobStorageTransmitter(storage.Object, sender.Object);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region Enqueue

        [Fact(DisplayName = "Enqueue: Should throw an argument null exception for data")]
        public void Enqueue_DataNull()
        {
            // arrange
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = CreateEmptyStorage();
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;
            ITelemetryAttachmentTransmitter transmitter = new BlobStorageTransmitter(storage.Object, sender);
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
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = CreateEmptyStorage();
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;
            ITelemetryAttachmentTransmitter transmitter = new BlobStorageTransmitter(storage.Object, sender);
            AttachmentDescriptor data = new Mock<AttachmentDescriptor>().Object;

            // act
            Action verify = () => new BlobStorageTransmitter(storage.Object, sender);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Enqueue: Should enqueue data and transfer it")]
        public void Enqueue_TransmissionFlow()
        {
            // assert
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = new Mock<ITransmissionStorage<AttachmentDescriptor>>();
            Mock<ITransmissionSender<AttachmentDescriptor>> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>();
            ConcurrentQueue<AttachmentDescriptor[]> bufferQueue = new ConcurrentQueue<AttachmentDescriptor[]>();

            storage
                .Setup(t => t.EnqueueAsync(It.IsAny<AttachmentDescriptor[]>(), It.IsAny<CancellationToken>()))
                .Callback((AttachmentDescriptor[] d, CancellationToken t) => bufferQueue.Enqueue(d));
            storage
                .Setup(t => t.DequeueAsync(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    int count = 0;
                    List<AttachmentDescriptor> results = new List<AttachmentDescriptor>();

                    while (bufferQueue.TryDequeue(out AttachmentDescriptor[] d) && count < 10)
                    {
                        results.AddRange(d);
                        count++;
                    }

                    return Task.FromResult<IReadOnlyCollection<AttachmentDescriptor>>(results);
                });
            sender
                .Setup(t => t.SendAsync(It.IsAny<IReadOnlyCollection<AttachmentDescriptor>>(), It.IsAny<CancellationToken>()))
                .Callback(() => resetEvent.Set());

            ITelemetryAttachmentTransmitter transmitter = new BlobStorageTransmitter(storage.Object, sender.Object);
            AttachmentDescriptor data = new Mock<AttachmentDescriptor>().Object;

            // act
            transmitter.Enqueue(data);

            // arrange
            resetEvent.Wait(TimeSpan.FromSeconds(5));

            sender.Verify(s =>
                s.SendAsync(It.Is<IReadOnlyCollection<AttachmentDescriptor>>(d => d.Count == 1),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Dispose

        [Fact(DisplayName = "Dispose: Should work like a charm")]
        public void Dispose()
        {
            // arrange
            Mock<ITransmissionStorage<AttachmentDescriptor>> storage = CreateEmptyStorage();
            ITransmissionSender<AttachmentDescriptor> sender = new Mock<ITransmissionSender<AttachmentDescriptor>>().Object;
            BlobStorageTransmitter transmitter = new BlobStorageTransmitter(storage.Object, sender);

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
                .ReturnsAsync(new List<AttachmentDescriptor>());

            return storage;
        }
    }
}
