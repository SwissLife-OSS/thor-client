using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.EventHub.Tests
{
    public class EventHubTransmissionBufferTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for client")]
        public void Constructor_SourceNull()
        {
            // assert
            EventHubClient client = null;

            // act
            Action verify = () => new EventHubTransmissionBuffer(client);

            // arrange
            Assert.Throws<ArgumentNullException>("client", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_Success()
        {
            // assert
            EventHubClient client = EventHubClient.CreateFromConnectionString(Constants.FakeConnectionString);

            // act
            Action verify = () => new EventHubTransmissionBuffer(client);

            // arrange
            Exception exception = Record.Exception(verify);
            Assert.Null(exception);
        }

        #endregion

        #region DequeueAsync

        [Fact(DisplayName = "DequeueAsync: Should not throw any exception")]
        public async Task DequeueAsync_NoException()
        {
            // assert
            EventHubClient client = EventHubClient.CreateFromConnectionString(Constants.FakeConnectionString);
            EventHubTransmissionBuffer buffer = new EventHubTransmissionBuffer(client);

            // act
            Func<Task> verify = () => buffer.DequeueAsync();

            // arrange
            Assert.Null(await Record.ExceptionAsync(verify).ConfigureAwait(false));
        }

        #endregion

        #region EnqueueAsync

        [Fact(DisplayName = "EnqueueAsync: Should throw an argument null exception for data")]
        public async Task EnqueueAsync_DataNull()
        {
            // arrange
            EventHubClient client = EventHubClient.CreateFromConnectionString(Constants.FakeConnectionString);
            EventHubTransmissionBuffer buffer = new EventHubTransmissionBuffer(client);
            EventData data = null;

            // act
            Func<Task> verify = () => buffer.EnqueueAsync(data);

            // assert
            await Assert.ThrowsAsync<ArgumentNullException>("data", verify).ConfigureAwait(false);
        }

        #endregion
    }
}