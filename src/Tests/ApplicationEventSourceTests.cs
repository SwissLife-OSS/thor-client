using ChilliCream.Tracing.Analyzer;
using FluentAssertions;
using Xunit;

namespace ChilliCream.Tracing.Tests
{
    public class ApplicationEventSourceTests
    {
        [Fact(DisplayName = "Analyze: Inspects ApplicationEventSource schema")]
        public void Analyze()
        {
            // Arrange
            EventSourceAnalyzer analyzer = new EventSourceAnalyzer();

            // Act
            Report report = analyzer.Inspect(ApplicationEventSource.Log);

            // Assert
            report.HasErrors.Should().BeFalse();
        }
    }
}