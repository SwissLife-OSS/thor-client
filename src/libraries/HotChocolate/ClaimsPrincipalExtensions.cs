using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;


namespace Thor.HotChocolate
{
    internal static class ClaimsPrincipalExtensions
    {
        public static Guid? GetId(this ClaimsPrincipal user)
        {
            // todo: make the claim type for selecting the user identifier configurable

            string rawUserId = user?.Claims?
                .Where(c => c.Type == JwtRegisteredClaimNames.Sub)
                .Select(c => c.Value)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(rawUserId) && Guid.TryParse(rawUserId, out Guid userId))
            {
                return userId;
            }

            return null;
        }
    }
}