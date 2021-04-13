using System;
using System.Linq;
using System.Security.Claims;

namespace Thor.Hosting.AspNetCore
{
    internal static class ClaimsPrincipalExtensions
    {
        public static Guid? GetId(this ClaimsPrincipal user)
        {
            string rawUserId = user?.Claims?
                .Where(c => c.Type == "sub")
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
