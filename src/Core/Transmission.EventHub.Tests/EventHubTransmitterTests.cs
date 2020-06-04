using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
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
            IJobHealthCheck jobHealthCheck = Mock.Of<IJobHealthCheck>();
            IMemoryBuffer<EventData> buffer = null;
            ITransmissionBuffer<EventData> aggregator = Mock.Of<ITransmissionBuffer<EventData>>();
            ITransmissionSender<EventData[]> sender = Mock.Of<ITransmissionSender<EventData[]>>();
            ITransmissionStorage<EventData> storage = Mock.Of<ITransmissionStorage<EventData>>();

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage, jobHealthCheck, new EventsOptions());

            // arrange
            Assert.Throws<ArgumentNullException>("buffer", verify);
        }

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for sender")]
        public void Constructor_SenderNull()
        {
            // assert
            IJobHealthCheck jobHealthCheck = Mock.Of<IJobHealthCheck>();
            IMemoryBuffer<EventData> buffer = Mock.Of<IMemoryBuffer<EventData>>();
            ITransmissionBuffer<EventData> aggregator = Mock.Of<ITransmissionBuffer<EventData>>();
            ITransmissionSender<EventData[]> sender = null;
            ITransmissionStorage<EventData> storage = Mock.Of<ITransmissionStorage<EventData>>();

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage, jobHealthCheck, new EventsOptions());

            // arrange
            Assert.Throws<ArgumentNullException>("sender", verify);
        }

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for storage")]
        public void Constructor_StorageNull()
        {
            // assert
            IJobHealthCheck jobHealthCheck = Mock.Of<IJobHealthCheck>();
            IMemoryBuffer<EventData> buffer = Mock.Of<IMemoryBuffer<EventData>>();
            ITransmissionBuffer<EventData> aggregator = Mock.Of<ITransmissionBuffer<EventData>>();
            ITransmissionSender<EventData[]> sender = Mock.Of<ITransmissionSender<EventData[]>>();
            ITransmissionStorage<EventData> storage = null;

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage, jobHealthCheck, new EventsOptions());

            // arrange
            Assert.Throws<ArgumentNullException>("storage", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            IJobHealthCheck jobHealthCheck = Mock.Of<IJobHealthCheck>();
            IMemoryBuffer<EventData> buffer = Mock.Of<IMemoryBuffer<EventData>>();
            ITransmissionBuffer<EventData> aggregator = Mock.Of<ITransmissionBuffer<EventData>>();
            ITransmissionSender<EventData[]> sender = Mock.Of<ITransmissionSender<EventData[]>>();
            ITransmissionStorage<EventData> storage = Mock.Of<ITransmissionStorage<EventData>>();

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage, jobHealthCheck, new EventsOptions());

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region Enqueue

        [Fact(DisplayName = "Enqueue: Should throw an argument null exception for data")]
        public void Enqueue_DataNull()
        {
            // arrange
            IJobHealthCheck jobHealthCheck = Mock.Of<IJobHealthCheck>();
            IMemoryBuffer<EventData> buffer = new Mock<IMemoryBuffer<EventData>>().Object;
            ITransmissionBuffer<EventData> aggregator = Mock.Of<ITransmissionBuffer<EventData>>();
            ITransmissionSender<EventData[]> sender = new Mock<ITransmissionSender<EventData[]>>().Object;
            ITransmissionStorage<EventData> storage = new Mock<ITransmissionStorage<EventData>>().Object;
            ITelemetryEventTransmitter transmitter = new EventHubTransmitter(
                buffer, aggregator, sender, storage, jobHealthCheck, new EventsOptions());
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
            IJobHealthCheck jobHealthCheck = Mock.Of<IJobHealthCheck>();
            IMemoryBuffer<EventData> buffer = new Mock<IMemoryBuffer<EventData>>().Object;
            ITransmissionBuffer<EventData> aggregator = Mock.Of<ITransmissionBuffer<EventData>>();
            ITransmissionSender<EventData[]> sender = new Mock<ITransmissionSender<EventData[]>>().Object;
            ITransmissionStorage<EventData> storage = new Mock<ITransmissionStorage<EventData>>().Object;
            TelemetryEvent data = new TelemetryEvent();

            // act
            Action verify = () => new EventHubTransmitter(
                buffer, aggregator, sender, storage, jobHealthCheck, new EventsOptions());

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Enqueue: Should enqueue data and transfer it")]
        public void Enqueue_TransmissionFlow()
        {
            // assert
            IJobHealthCheck jobHealthCheck = Mock.Of<IJobHealthCheck>();
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            Mock<IMemoryBuffer<EventData>> buffer = new Mock<IMemoryBuffer<EventData>>();
            Mock<ITransmissionBuffer<EventData>> aggregator = new Mock<ITransmissionBuffer<EventData>>();
            Mock<ITransmissionSender<EventData[]>> sender = new Mock<ITransmissionSender<EventData[]>>();
            Mock<ITransmissionStorage<EventData>> storage = new Mock<ITransmissionStorage<EventData>>();
            ConcurrentQueue<EventData> bufferQueue = new ConcurrentQueue<EventData>();

            buffer
                .Setup(t => t.Enqueue(It.IsAny<EventData>()))
                .Callback((EventData d) => bufferQueue.Enqueue(d));
            aggregator
                .Setup(t => t.Dequeue(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    int count = 0;
                    List<EventData> results = new List<EventData>();

                    while (bufferQueue.TryDequeue(out EventData d) && count < 10)
                    {
                        results.Add(d);
                        count++;
                    }

                    return new List<EventData[]> {results.ToArray()}.ToAsyncEnumerable();
                });
            sender
                .Setup(t => t.SendAsync(It.IsAny<IAsyncEnumerable<EventData[]>>(), It.IsAny<CancellationToken>()))
                .Callback(() => resetEvent.Set());

            ITelemetryEventTransmitter transmitter = new EventHubTransmitter(
                buffer.Object, aggregator.Object, sender.Object, storage.Object, jobHealthCheck, new EventsOptions());
            TelemetryEvent data = new TelemetryEvent();

            // act
            transmitter.Enqueue(data);

            // arrange
            resetEvent.Wait(TimeSpan.FromSeconds(5));

            sender.Verify(s => s.SendAsync(
                It.IsAny<IAsyncEnumerable<EventData[]>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Dispose

        [Fact(DisplayName = "Dispose: Should work like a charm")]
        public void Dispose()
        {
            // arrange
            IJobHealthCheck jobHealthCheck = Mock.Of<IJobHealthCheck>();
            IMemoryBuffer<EventData> buffer = new Mock<IMemoryBuffer<EventData>>().Object;
            ITransmissionBuffer<EventData> aggregator = Mock.Of<ITransmissionBuffer<EventData>>();
            ITransmissionSender<EventData[]> sender = new Mock<ITransmissionSender<EventData[]>>().Object;
            ITransmissionStorage<EventData> storage = new Mock<ITransmissionStorage<EventData>>().Object;
            EventHubTransmitter transmitter = new EventHubTransmitter(
                buffer, aggregator, sender, storage, jobHealthCheck, new EventsOptions());

            // act
            Action verify = () => transmitter.Dispose();

            // assert
            Assert.Null(Record.Exception(verify));
        }

        #endregion
    }
}
