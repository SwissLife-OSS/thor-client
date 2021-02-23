using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Xunit;

namespace Thor.Core.Transmission.EventHub.Tests
{
    public class EventHubTransmissionSenderTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for client")]
        public void Constructor_SourceNull()
        {
            // assert
            EventHubProducerClient client = null;

            // act
            Action verify = () => new EventHubTransmissionSender(client);

            // arrange
            Assert.Throws<ArgumentNullException>("client", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_Success()
        {
            // assert
            EventHubProducerClient client = new EventHubProducerClient(Constants.FakeConnectionString);

            // act
            Action verify = () => new EventHubTransmissionSender(client);

            // arrange
            Exception exception = Record.Exception(verify);
            Assert.Null(exception);
        }

        #endregion

        #region SendAsync

        [Fact(DisplayName = "SendAsync: Should throw an argument null exception for batch")]
        public async Task SendAsync_BatchNull()
        {
            // arrange
            EventHubProducerClient client = new EventHubProducerClient(Constants.FakeConnectionString);
            EventHubTransmissionSender sender = new EventHubTransmissionSender(client);
            IAsyncEnumerable<EventDataBatch> batch = null;

            // act
            Func<Task> verify = () => sender.SendAsync(batch, default);

            // assert
            await Assert.ThrowsAsync<ArgumentNullException>("batches", verify).ConfigureAwait(false);
        }

        #endregion
    }
}
