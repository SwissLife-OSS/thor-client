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
            // arrange
            EventSourceAnalyzer analyzer = new EventSourceAnalyzer();

            // act
            Report report = analyzer.Inspect(ActivityEventSource.Log);

            // assert
            report.HasErrors.Should().BeFalse();
        }
    }
}