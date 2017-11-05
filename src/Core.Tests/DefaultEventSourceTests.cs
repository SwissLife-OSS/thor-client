using Thor.Analyzer;
using FluentAssertions;
using System;
using System.Diagnostics.Tracing;
using System.Linq;
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
            report.HasErrors.Should().BeFalse();
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Critical("Critical-Message-Enabled");

                // assert
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
                // act
                DefaultEventSource.Log.Critical("Critical-Format-{0}", "Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Critical("Critical-Message-{0}", "Enabled");

                // assert
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
            // act
            Action validate = () => DefaultEventSource.Log.Critical(format, "");

            // assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
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
                // act
                DefaultEventSource.Log.Error("Error-Message-Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Error("Critical-Message-Enabled");

                // assert
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
                // act
                DefaultEventSource.Log.Error("Error-Format-{0}", "Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Error("Error-Message-{0}", "Enabled");

                // assert
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
            // act
            Action validate = () => DefaultEventSource.Log.Error(format, "");

            // assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
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
                // act
                DefaultEventSource.Log.Info("Info-Message-Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Info("Info-Message-Enabled");

                // assert
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
                // act
                DefaultEventSource.Log.Info("Info-Format-{0}", "Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Info("Info-Message-{0}", "Enabled");

                // assert
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
            // act
            Action validate = () => DefaultEventSource.Log.Info(format, "");

            // assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
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
                // act
                DefaultEventSource.Log.Verbose("Verbose-Message-Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Verbose("Verbose-Message-Enabled");

                // assert
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
                // act
                DefaultEventSource.Log.Verbose("Verbose-Format-{0}", "Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Verbose("Verbose-Message-{0}", "Enabled");

                // assert
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
            // act
            Action validate = () => DefaultEventSource.Log.Verbose(format, "");

            // assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
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
                // act
                DefaultEventSource.Log.Warning("Warning-Message-Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Warning("Warning-Message-Enabled");

                // assert
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
                // act
                DefaultEventSource.Log.Warning("Warning-Format-{0}", "Disabled");

                // assert
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
                // arrange
                string expectedMessage = "{2}";

                // act
                DefaultEventSource.Log.Warning("Warning-Message-{0}", "Enabled");

                // assert
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
            // act
            Action validate = () => DefaultEventSource.Log.Warning(format, "");

            // assert
            validate.ShouldThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("format");
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