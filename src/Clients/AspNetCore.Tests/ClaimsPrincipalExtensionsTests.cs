using System;
using System.Security.Claims;
using Xunit;

namespace Thor.AspNetCore.Tests
{
    public class ClaimsPrincipalExtensionsTests
    {
        #region GetId

        [Fact(DisplayName = "GetId: Should return null if user is null")]
        public void GetId_UserNull()
        {
            // arrange
            ClaimsPrincipal user = null;

            // act
            Guid? userId = user.GetId();

            // assert
            Assert.Null(userId);
        }

        [Fact(DisplayName = "GetId: Should return null if 'sub' is not found")]
        public void GetId_SubNotFound()
        {
            // arrange
            Claim[] claims = new Claim[0];
            ClaimsIdentity identity = new ClaimsIdentity(claims);
            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            // act
            Guid? userId = user.GetId();

            // assert
            Assert.Null(userId);
        }

        [Fact(DisplayName = "GetId: Should return null if 'sub' value is empty")]
        public void GetId_SubValueEmpty()
        {
            // arrange
            Claim[] claims =
            {
                new Claim(JwtClaimNames.Sub, "")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims);
            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            // act
            Guid? userId = user.GetId();

            // assert
            Assert.Null(userId);
        }

        [Fact(DisplayName = "GetId: Should return null if 'sub' value is invalid")]
        public void GetId_SubValueInvalid()
        {
            // arrange
            Claim[] claims =
            {
                new Claim(JwtClaimNames.Sub, "werweee")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims);
            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            // act
            Guid? userId = user.GetId();

            // assert
            Assert.Null(userId);
        }
        
        [Fact(DisplayName = "GetId: Should return a valid user id if 'sub' value is valid")]
        public void GetId_SubValueValid()
        {
            // arrange
            Guid expectedUserId = Guid.NewGuid();
            Claim[] claims =
            {
                new Claim(JwtClaimNames.Sub, expectedUserId.ToString())
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims);
            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            // act
            Guid? userId = user.GetId();

            // assert
            Assert.Equal(expectedUserId, userId);
        }

        #endregion
    }
}