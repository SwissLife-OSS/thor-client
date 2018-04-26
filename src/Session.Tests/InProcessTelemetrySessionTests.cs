using System;
using Thor.Core.Abstractions;
using Thor.Core.Testing.Utilities;
using Xunit;

namespace Thor.Core.Session.Tests
{
    public class InProcessTelemetrySessionTests
    {
        #region Create

        [Fact(DisplayName = "Create: Should throw an argument out of range exception for applicationId")]
        public void Create_ApplicationIdLessOne()
        {
            // arrange
            int applicationId = 0;

            // act
            Action verify = () => InProcessTelemetrySession.Create(applicationId);

            // assert
            Assert.Throws<ArgumentOutOfRangeException>("applicationId", verify);
        }

        [Fact(DisplayName = "Create: Should not throw any exception")]
        public void Create_NoException()
        {
            // arrange
            int applicationId = 1;

            // act
            Action verify = () => InProcessTelemetrySession.Create(applicationId);

            // assert
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Create: Should have enabled default event providers if referenced")]
        public void Create_VerifyDefaultProviders()
        {
            // arrange
            int applicationId = 1;
            ProbeTransmitter transmitter = new ProbeTransmitter();

            // act
            int telemetryCount = 0;

            using (ITelemetrySession session = InProcessTelemetrySession.Create(applicationId))
            {
                session.SetTransmitter(transmitter);
                DefaultEventSource.Log.Info("VerifyDefaultProviders");
                telemetryCount = transmitter.Count;
            }

            // assert
            Assert.Equal(1, telemetryCount);
        }

        #endregion
    }
}