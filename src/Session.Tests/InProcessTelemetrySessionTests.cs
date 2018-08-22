using System;
using System.Diagnostics.Tracing;
using Thor.Core.Session.Abstractions;
using Thor.Core.Transmission.Abstractions;
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
                session.Attach(transmitter);
                Application.Start(applicationId);
                telemetryCount = transmitter.Count;
            }

            // assert
            Assert.Equal(1, telemetryCount);
        }

        [Fact(DisplayName = "Create: Should have enabled default event providers plus those who where allowed if referenced")]
        public void Create_VerifyDefaultAndAllowedProviders()
        {
            // arrange
            int applicationId = 1;
            ProbeTransmitter transmitter = new ProbeTransmitter();

            // act
            int telemetryCount = 0;

            using (ITelemetrySession session = InProcessTelemetrySession
                .Create(applicationId, EventLevel.Verbose,
                    new[] { "System.Threading" }))
            {
                session.Attach(transmitter);
                Application.Start(applicationId);
                telemetryCount = transmitter.Count;
            }

            // assert
            Assert.Equal(1, telemetryCount);
        }

        #endregion

        #region EnableProvider

        [Fact(DisplayName = "EnableProvider: Should throw an argument null exception for name")]
        public void EnableProvider_TransmitterNull()
        {
            using (ITelemetrySession session = InProcessTelemetrySession.Create(1))
            {
                // arrange
                string name = null;

                // act
                Action verify = () => session.EnableProvider(name, EventLevel.Critical);

                // assert
                Assert.Throws<ArgumentNullException>("name", verify);
            }
        }

        [Fact(DisplayName = "EnableProvider: Should not throw any exception")]
        public void EnableProvider_NoException()
        {
            using (ITelemetrySession session = InProcessTelemetrySession.Create(1))
            {
                // arrange
                string name = "Valid-Name";

                // act
                Action verify = () => session.EnableProvider(name, EventLevel.Critical);

                // assert
                Assert.Null(Record.Exception(verify));
            }
        }

        #endregion

        #region SetTransmitter

        [Fact(DisplayName = "SetTransmitter: Should throw an argument null exception for transmitter")]
        public void SetTransmitter_TransmitterNull()
        {
            using (ITelemetrySession session = InProcessTelemetrySession.Create(1))
            {
                // arrange
                ITelemetryEventTransmitter transmitter = null;

                // act
                Action verify = () => session.Attach(transmitter);

                // assert
                Assert.Throws<ArgumentNullException>("transmitter", verify);
            }
        }

        [Fact(DisplayName = "SetTransmitter: Should not throw any exception")]
        public void SetTransmitter_NoException()
        {
            using (ITelemetrySession session = InProcessTelemetrySession.Create(1))
            {
                // arrange
                ITelemetryEventTransmitter transmitter = new ProbeTransmitter();

                // act
                Action verify = () => session.Attach(transmitter);

                // assert
                Assert.Null(Record.Exception(verify));
            }
        }

        #endregion
    }
}