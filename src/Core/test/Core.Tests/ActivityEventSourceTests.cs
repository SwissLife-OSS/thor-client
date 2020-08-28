using Thor.Analyzer;
using Xunit;

namespace Thor.Core.Tests
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
            Assert.False(report.HasErrors);
        }
    }
}