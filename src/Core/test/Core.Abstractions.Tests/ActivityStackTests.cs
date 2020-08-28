using System;
using Xunit;

namespace Thor.Core.Abstractions.Tests
{
    public class ActivityStackTests
    {
        [Fact(DisplayName = "Id: Should return empty")]
        public void Id_Empty()
        {
            // act
            Guid result = ActivityStack.Id;

            // assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact(DisplayName = "Id: Should return an id")]
        public void Id_NotEmpty()
        {
            IDisposable disposable = null;

            try
            {
                // arrange
                Guid activityId = Guid.NewGuid();
                disposable = ActivityStack.Push(activityId);

                // act
                Guid result = ActivityStack.Id;

                // assert
                Assert.Equal(activityId, result);
            }
            finally
            {
                disposable?.Dispose();
            }
        }
    }
}