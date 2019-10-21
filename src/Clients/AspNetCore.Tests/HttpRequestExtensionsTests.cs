using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Thor.Core.Abstractions;
using Xunit;

namespace Thor.Hosting.AspNetCore.Tests
{
    public class HttpRequestExtensionsTests
    {
        #region GetActivityId

        [Fact(DisplayName = "GetActivityId: Should return null if request is null")]
        public void GetActivityId_RequestNull()
        {
            // arrange
            HttpRequest request = null;

            // act
            Guid? requestId = request.GetActivityId();

            // assert
            Assert.Null(requestId);
        }

        [Fact(DisplayName = "GetActivityId: Should return null if activity id header not found")]
        public void GetActivityId_HeaderNotFound()
        {
            // arrange
            HttpContext context = new DefaultHttpContext();
            HttpRequest request = context.Request;

            // act
            Guid? activityId = request.GetActivityId();

            // assert
            Assert.Null(activityId);
        }

        [Fact(DisplayName = "GetActivityId: Should return null if activity id header is empty")]
        public void GetActivityId_HeaderEmpty()
        {
            // arrange
            HttpContext context = new DefaultHttpContext();
            HttpRequest request = context.Request;

            request.Headers.Add(MessageHeaderKeys.ActivityId, new StringValues(""));

            // act
            Guid? activityId = request.GetActivityId();

            // assert
            Assert.Null(activityId);
        }

        [Fact(DisplayName = "GetActivityId: Should return null if 'sub' value is invalid")]
        public void GetActivityId_SubValueInvalid()
        {
            // arrange
            HttpContext context = new DefaultHttpContext();
            HttpRequest request = context.Request;

            request.Headers.Add(MessageHeaderKeys.ActivityId, new StringValues("invalid"));

            // act
            Guid? activityId = request.GetActivityId();

            // assert
            Assert.Null(activityId);
        }

        [Fact(DisplayName = "GetActivityId: Should return a valid user id if 'sub' value is valid")]
        public void GetActivityId_SubValueValid()
        {
            // arrange
            Guid expectedUserId = Guid.NewGuid();
            HttpContext context = new DefaultHttpContext();
            HttpRequest request = context.Request;

            request.Headers.Add(MessageHeaderKeys.ActivityId, new StringValues(expectedUserId.ToString()));

            // act
            Guid? activityId = request.GetActivityId();

            // assert
            Assert.Equal(expectedUserId, activityId);
        }

        #endregion
    }
}