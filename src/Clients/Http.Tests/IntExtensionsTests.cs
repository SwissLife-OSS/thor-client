using Xunit;

namespace Thor.Core.Http.Tests
{
    public class IntExtensionsTests
    {
        [Theory(DisplayName = "GetHttpStatusText: Should return a value that fits to the input")]
        [InlineData(-1, "UNKNOWN")]
        [InlineData(0, "UNKNOWN")]
        [InlineData(1, "UNKNOWN")]
        [InlineData(200, "OK")]
        [InlineData(404, "NOTFOUND")]
        [InlineData(1000, "UNKNOWN")]
        public void GetHttpStatusText(int input, string output)
        {
            // arrange

            // act
            string statusText = input.GetHttpStatusText();

            // assert
            Assert.Equal(output, statusText);
        }
    }
}