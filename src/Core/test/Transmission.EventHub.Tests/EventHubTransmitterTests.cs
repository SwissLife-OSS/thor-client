using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Moq;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.EventHub.Tests
{
    public class EventHubTransmitterTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for buffer")]
        public void Constructor_BufferNull()
        {
            // assert
            IMemoryBuffer<EventData> buffer = null;
            ITransmissionBuffer<EventData, EventDataBatch> aggregator = Mock.Of<ITransmissionBuffer<EventData, EventDataBatch>>();
            ITransmissionSender<EventDataBatch> sender = Mock.Of<ITransmissionSender<EventDataBatch>>();
            ITransmissionStorage<EventData> storage = Mock.Of<ITransmissionStorage<EventData>>();

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage);

            // arrange
            Assert.Throws<ArgumentNullException>("buffer", verify);
        }

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for sender")]
        public void Constructor_SenderNull()
        {
            // assert
            IMemoryBuffer<EventData> buffer = Mock.Of<IMemoryBuffer<EventData>>();
            ITransmissionBuffer<EventData, EventDataBatch> aggregator = Mock.Of<ITransmissionBuffer<EventData, EventDataBatch>>();
            ITransmissionSender<EventDataBatch> sender = null;
            ITransmissionStorage<EventData> storage = Mock.Of<ITransmissionStorage<EventData>>();

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage);

            // arrange
            Assert.Throws<ArgumentNullException>("sender", verify);
        }

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for storage")]
        public void Constructor_StorageNull()
        {
            // assert
            IMemoryBuffer<EventData> buffer = Mock.Of<IMemoryBuffer<EventData>>();
            ITransmissionBuffer<EventData, EventDataBatch> aggregator = Mock.Of<ITransmissionBuffer<EventData, EventDataBatch>>();
            ITransmissionSender<EventDataBatch> sender = Mock.Of<ITransmissionSender<EventDataBatch>>();
            ITransmissionStorage<EventData> storage = null;

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage);

            // arrange
            Assert.Throws<ArgumentNullException>("storage", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            IMemoryBuffer<EventData> buffer = Mock.Of<IMemoryBuffer<EventData>>();
            ITransmissionBuffer<EventData, EventDataBatch> aggregator = Mock.Of<ITransmissionBuffer<EventData, EventDataBatch>>();
            ITransmissionSender<EventDataBatch> sender = Mock.Of<ITransmissionSender<EventDataBatch>>();
            ITransmissionStorage<EventData> storage = Mock.Of<ITransmissionStorage<EventData>>();

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region Enqueue

        [Fact(DisplayName = "Enqueue: Should throw an argument null exception for data")]
        public void Enqueue_DataNull()
        {
            // arrange
            IMemoryBuffer<EventData> buffer = new Mock<IMemoryBuffer<EventData>>().Object;
            ITransmissionBuffer<EventData, EventDataBatch> aggregator = Mock.Of<ITransmissionBuffer<EventData, EventDataBatch>>();
            ITransmissionSender<EventDataBatch> sender = new Mock<ITransmissionSender<EventDataBatch>>().Object;
            ITransmissionStorage<EventData> storage = new Mock<ITransmissionStorage<EventData>>().Object;
            ITelemetryEventTransmitter transmitter = new EventHubTransmitter(
                buffer, aggregator, sender, storage);
            TelemetryEvent data = null;

            // act
            Action verify = () => transmitter.Enqueue(data);

            // assert
            Assert.Throws<ArgumentNullException>("data", verify);
        }

        [Fact(DisplayName = "Enqueue: Should not throw any exception")]
        public void Enqueue_NoException()
        {
            // assert
            IMemoryBuffer<EventData> buffer = new Mock<IMemoryBuffer<EventData>>().Object;
            ITransmissionBuffer<EventData, EventDataBatch> aggregator = Mock.Of<ITransmissionBuffer<EventData, EventDataBatch>>();
            ITransmissionSender< EventDataBatch> sender = new Mock<ITransmissionSender<EventDataBatch>>().Object;
            ITransmissionStorage<EventData> storage = new Mock<ITransmissionStorage<EventData>>().Object;
            TelemetryEvent data = new TelemetryEvent();

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Enqueue: Should enqueue data and transfer it")]
        public void Enqueue_TransmissionFlow()
        {
            // assert
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            Mock<IMemoryBuffer<EventData>> buffer = new Mock<IMemoryBuffer<EventData>>();
            Mock<ITransmissionBuffer<EventData, EventDataBatch>> aggregator = new Mock<ITransmissionBuffer<EventData, EventDataBatch>>();
            Mock<ITransmissionSender<EventDataBatch>> sender = new Mock<ITransmissionSender<EventDataBatch>>();
            Mock<ITransmissionStorage<EventData>> storage = new Mock<ITransmissionStorage<EventData>>();
            ConcurrentQueue<EventDataBatch> bufferQueue = new ConcurrentQueue<EventDataBatch>();

            buffer
                .Setup(t => t.Enqueue(It.IsAny<EventData>()))
                .Callback((EventData d) => bufferQueue.Enqueue(EventHubsModelFactory.EventDataBatch(1, new[] {d})));
            aggregator
                .Setup(t => t.Dequeue(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    int count = 0;
                    List<EventDataBatch> results = new List<EventDataBatch>();

                    while (bufferQueue.TryDequeue(out EventDataBatch d) && count < 10)
                    {
                        results.Add(d);
                        count++;
                    }

                    return results.ToAsyncEnumerable();
                });
            sender
                .Setup(t => t.SendAsync(It.IsAny<IAsyncEnumerable<EventDataBatch>>(), It.IsAny<CancellationToken>()))
                .Callback(() => resetEvent.Set());

            ITelemetryEventTransmitter transmitter = new EventHubTransmitter(
                buffer.Object, aggregator.Object, sender.Object, storage.Object);
            TelemetryEvent data = new TelemetryEvent();

            // act
            transmitter.Enqueue(data);

            // arrange
            resetEvent.Wait(TimeSpan.FromSeconds(5));

            sender.Verify(s => s.SendAsync(
                It.IsAny<IAsyncEnumerable<EventDataBatch>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Dispose

        [Fact(DisplayName = "Dispose: Should work like a charm")]
        public void Dispose()
        {
            // arrange
            IMemoryBuffer<EventData> buffer = new Mock<IMemoryBuffer<EventData>>().Object;
            ITransmissionBuffer<EventData, EventDataBatch> aggregator = Mock.Of<ITransmissionBuffer<EventData, EventDataBatch>>();
            ITransmissionSender<EventDataBatch> sender = new Mock<ITransmissionSender<EventDataBatch>>().Object;
            ITransmissionStorage<EventData> storage = new Mock<ITransmissionStorage<EventData>>().Object;
            EventHubTransmitter transmitter = new EventHubTransmitter(
                buffer, aggregator, sender, storage);

            // act
            Action verify = () => transmitter.Dispose();

            // assert
            Assert.Null(Record.Exception(verify));
        }

        #endregion
    }
}
