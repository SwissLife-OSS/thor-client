using ChilliCream.Tracing.Analyzer;
using FluentAssertions;
using Xunit;

namespace ChilliCream.Tracing.Tests
{
    public class ActivityEventSourceTests
    {
        [Fact(DisplayName = "Analyze: Inspects ApplicationEventSource schema")]
        public void Analyze()
        {
            // Arrange
            EventSourceAnalyzer analyzer = new EventSourceAnalyzer();

            // Act
            Report report = analyzer.Inspect(ActivityEventSource.Log);

            // Assert
            report.HasErrors.Should().BeFalse();
        }
    }
}