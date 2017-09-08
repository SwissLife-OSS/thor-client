using Xunit;
using ChilliCream.Tracing.Analyzer;
using FluentAssertions;

namespace ChilliCream.Tracing.Tests
{
    public class DefaultEventSourceTests
    {
        [Fact(DisplayName = "Analyze: Inspects DefaultEventSource schema")]
        public void Analyze()
        {
            // Arrange
            EventSourceAnalyzer analyzer = new EventSourceAnalyzer();
            DefaultEventSource eventSource = DefaultEventSource.Log;

            // Act
            Report report = analyzer.Inspect(DefaultEventSource.Log);

            // Assert
            report.HasErrors.Should().BeFalse();
        }
    }
}