using System;
using Microsoft.Extensions.Http;
using Xunit;

namespace Thor.Core.Http.Tests
{
    public class TracingHttpMessageHandlerBuilderFilterTests
    {
        #region Configure

        [Fact(DisplayName = "Configure: Should throw an argument null exception for next")]
        public void Configure_NextNull()
        {
            // arrange
            Action<HttpMessageHandlerBuilder> next = null;
            TracingHttpMessageHandlerBuilderFilter filter = new TracingHttpMessageHandlerBuilderFilter();

            // act
            Action verify = () => filter.Configure(next);

            // assert
            Assert.Throws<ArgumentNullException>("next", verify);
        }

        [Fact(DisplayName = "Configure: Should return a new builder")]
        public void Configure_NotNull()
        {
            // arrange
            Action<HttpMessageHandlerBuilder> next = b => { };
            TracingHttpMessageHandlerBuilderFilter filter = new TracingHttpMessageHandlerBuilderFilter();

            // act
            Action<HttpMessageHandlerBuilder> result = filter.Configure(next);

            // assert
            Assert.NotNull(result);
        }

        #endregion
    }
}