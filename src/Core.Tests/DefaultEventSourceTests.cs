using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Thor.Analyzer;
using Thor.Core.Testing.Utilities;
using Xunit;

namespace Thor.Core.Tests
{
    public class DefaultEventSourceTests
    {
        [Fact(DisplayName = "Analyze: Inspects DefaultEventSource schema")]
        public void Analyze()
        {
            // arrange
            EventSourceAnalyzer analyzer = new EventSourceAnalyzer();

            // act
            Report report = analyzer.Inspect(DefaultEventSource.Log);

            // assert
            Assert.False(report.HasErrors);
        }

        #region Critical

        [Fact(DisplayName = "Critical: Should not write anything to the log stream")]
        public void CriticalDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Critical("Critical-Message-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Critical: Should write one critical message to the log stream")]
        public void CriticalEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Critical("Critical-Message-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [Fact(DisplayName = "Critical: Should not write anything to the log stream")]
        public void CriticalFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Critical("Critical-Format-{0}", "Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Critical: Should write one critical message to the log stream")]
        public void CriticalFormatEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Critical("Critical-Message-{0}", "Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Critical: Should throw argument null exception for format")]
        public void CriticalFormatNull(string format)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Critical(format, "");

            // assert
            Assert.Throws<ArgumentNullException>("format", validate);
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Critical: Should throw argument null exception for message")]
        public void CriticalMessageNull(string message)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Critical(message);

            // assert
            Assert.Throws<ArgumentNullException>("message", validate);
        }

        #endregion

        #region Error

        [Fact(DisplayName = "Error: Should not write anything to the log stream")]
        public void ErrorDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Error("Error-Message-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Error: Should write one error message to the log stream")]
        public void ErrorEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Error("Critical-Message-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [Fact(DisplayName = "Error: Should not write anything to the log stream")]
        public void ErrorFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Error("Error-Format-{0}", "Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Error: Should write one error message to the log stream")]
        public void ErrorFormatEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Error("Error-Message-{0}", "Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Error: Should throw argument null exception for format")]
        public void ErrorFormatNull(string format)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Error(format, "");

            // assert
            Assert.Throws<ArgumentNullException>("format", validate);
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Error: Should throw argument null exception for message")]
        public void ErrorMessageNull(string message)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Error(message);

            // assert
            Assert.Throws<ArgumentNullException>("message", validate);
        }

        #endregion

        #region Info

        [Fact(DisplayName = "Info: Should not write anything to the log stream")]
        public void InfoDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Info("Info-Message-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Info: Should write one info message to the log stream")]
        public void InfoEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Info("Info-Message-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [Fact(DisplayName = "Info: Should not write anything to the log stream")]
        public void InfoFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Info("Info-Format-{0}", "Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Info: Should write one info message to the log stream")]
        public void InfoFormatEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Info("Info-Message-{0}", "Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Info: Should throw argument null exception for format")]
        public void InfoFormatNull(string format)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Info(format, "");

            // assert
            Assert.Throws<ArgumentNullException>("format", validate);
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Info: Should throw argument null exception for message")]
        public void InfoMessageNull(string message)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Info(message);

            // assert
            Assert.Throws<ArgumentNullException>("message", validate);
        }

        #endregion

        #region Verbose

        [Fact(DisplayName = "Verbose: Should not write anything to the log stream")]
        public void VerboseDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Verbose("Verbose-Message-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Verbose: Should write one verbose message to the log stream")]
        public void VerboseEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Verbose("Verbose-Message-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [Fact(DisplayName = "Verbose: Should not write anything to the log stream")]
        public void VerboseFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Verbose("Verbose-Format-{0}", "Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Verbose: Should write one verbose message to the log stream")]
        public void VerboseFormatEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Verbose("Verbose-Message-{0}", "Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Verbose: Should throw argument null exception for format")]
        public void VerboseFormatNull(string format)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Verbose(format, "");

            // assert
            Assert.Throws<ArgumentNullException>("format", validate);
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Verbose: Should throw argument null exception for message")]
        public void VerboseMessageNull(string message)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Verbose(message);

            // assert
            Assert.Throws<ArgumentNullException>("message", validate);
        }

        #endregion

        #region Warning

        [Fact(DisplayName = "Warning: Should not write anything to the log stream")]
        public void WarningDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Warning("Warning-Message-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Warning: Should write one warning message to the log stream")]
        public void WarningEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Warning("Warning-Message-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [Fact(DisplayName = "Warning: Should not write anything to the log stream")]
        public void WarningFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                DefaultEventSource.Log.Warning("Warning-Format-{0}", "Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Warning: Should write one warning message to the log stream")]
        public void WarningFormatEnabled()
        {
            DefaultEventSource.Log.ProbeEvents((listener) =>
            {
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Warning("Warning-Message-{0}", "Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Warning: Should throw argument null exception for format")]
        public void WarningFormatNull(string format)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Warning(format, "");

            // assert
            Assert.Throws<ArgumentNullException>("format", validate);
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Warning: Should throw argument null exception for message")]
        public void WarningMessageNull(string message)
        {
            // act
            Action validate = () => DefaultEventSource.Log.Warning(message);

            // assert
            Assert.Throws<ArgumentNullException>("message", validate);
        }

        #endregion
    }
}