using ChilliCream.Tracing.Analyzer;
using FluentAssertions;
using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Xunit;

namespace ChilliCream.Tracing.Tests
{
    public class DefaultEventSourceTests
    {
        [Fact(DisplayName = "Analyze: Inspects DefaultEventSource schema")]
        public void Analyze()
        {
            // Arrange
            EventSourceAnalyzer analyzer = new EventSourceAnalyzer();

            // Act
            Report report = analyzer.Inspect(DefaultEventSource.Log);

            // Assert
            report.HasErrors.Should().BeFalse();
        }

        #region Critical

        [Fact(DisplayName = "Critical: Should not write anything to the log stream")]
        public void CriticalDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Critical("Critical-Message-Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Critical: Should write one critical message to the log stream")]
        public void CriticalEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Critical("Critical-Message-Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [Fact(DisplayName = "Critical: Should not write anything to the log stream")]
        public void CriticalFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Critical("Critical-Format-{0}", "Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Critical: Should write one critical message to the log stream")]
        public void CriticalFormatEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Critical("Critical-Message-{0}", "Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Critical: Should throw argument null exception for format")]
        public void CriticalFormatNull(string format)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Critical(format, "");

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Critical: Should throw argument null exception for message")]
        public void CriticalMessageNull(string message)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Critical(message);

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("message");
        }

        #endregion

        #region Error

        [Fact(DisplayName = "Error: Should not write anything to the log stream")]
        public void ErrorDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Error("Error-Message-Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Error: Should write one critical message to the log stream")]
        public void ErrorEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Error("Critical-Message-Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [Fact(DisplayName = "Error: Should not write anything to the log stream")]
        public void ErrorFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Error("Error-Format-{0}", "Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Error: Should write one critical message to the log stream")]
        public void ErrorFormatEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Error("Error-Message-{0}", "Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Error: Should throw argument null exception for format")]
        public void ErrorFormatNull(string format)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Error(format, "");

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Error: Should throw argument null exception for message")]
        public void ErrorMessageNull(string message)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Error(message);

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("message");
        }

        #endregion

        #region Info

        [Fact(DisplayName = "Info: Should not write anything to the log stream")]
        public void InfoDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Info("Info-Message-Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Info: Should write one critical message to the log stream")]
        public void InfoEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Info("Info-Message-Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [Fact(DisplayName = "Info: Should not write anything to the log stream")]
        public void InfoFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Info("Info-Format-{0}", "Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Info: Should write one critical message to the log stream")]
        public void InfoFormatEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Info("Info-Message-{0}", "Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Info: Should throw argument null exception for format")]
        public void InfoFormatNull(string format)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Info(format, "");

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Info: Should throw argument null exception for message")]
        public void InfoMessageNull(string message)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Info(message);

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("message");
        }

        #endregion

        #region Verbose

        [Fact(DisplayName = "Verbose: Should not write anything to the log stream")]
        public void VerboseDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Verbose("Verbose-Message-Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Verbose: Should write one critical message to the log stream")]
        public void VerboseEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Verbose("Verbose-Message-Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [Fact(DisplayName = "Verbose: Should not write anything to the log stream")]
        public void VerboseFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Verbose("Verbose-Format-{0}", "Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Verbose: Should write one critical message to the log stream")]
        public void VerboseFormatEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Verbose("Verbose-Message-{0}", "Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Verbose: Should throw argument null exception for format")]
        public void VerboseFormatNull(string format)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Verbose(format, "");

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Verbose: Should throw argument null exception for message")]
        public void VerboseMessageNull(string message)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Verbose(message);

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("message");
        }

        #endregion

        #region Warning

        [Fact(DisplayName = "Warning: Should not write anything to the log stream")]
        public void WarningDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Warning("Warning-Message-Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Warning: Should write one critical message to the log stream")]
        public void WarningEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Warning("Warning-Message-Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [Fact(DisplayName = "Warning: Should not write anything to the log stream")]
        public void WarningFormatDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // Act
                DefaultEventSource.Log.Warning("Warning-Format-{0}", "Disabled");

                // Assert
                listener
                    .OrderedEvents
                    .Should()
                    .HaveCount(0);
            };
        }

        [Fact(DisplayName = "Warning: Should write one critical message to the log stream")]
        public void WarningFormatEnabled()
        {
            ProbeEvents(DefaultEventSource.Log, (listener) =>
            {
                // Arrange
                string expectedMessage = "{2}";

                // Act
                DefaultEventSource.Log.Warning("Warning-Message-{0}", "Enabled");

                // Assert
                listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage)
                    .Should()
                    .NotBeNull();
            });
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Warning: Should throw argument null exception for format")]
        public void WarningFormatNull(string format)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Warning(format, "");

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory(DisplayName = "Warning: Should throw argument null exception for message")]
        public void WarningMessageNull(string message)
        {
            // Act
            Action validate = () => DefaultEventSource.Log.Warning(message);

            // Assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("message");
        }

        #endregion

        private static void ProbeEvents(EventSource eventSource,
            Action<ProbeEventListener> execute)
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                try
                {
                    listener.EnableEvents(eventSource, EventLevel.Verbose);
                    execute(listener);
                }
                finally
                {
                    listener.DisableEvents(eventSource);
                }
            }
        }
    }
}