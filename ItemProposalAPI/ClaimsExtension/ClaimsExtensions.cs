using System.Linq;
using System.Security.Claims;

namespace ItemProposalAPI.ClaimsExtension
{
    public static class ClaimsExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.Claims.SingleOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier));

            return userIdClaim?.Value ?? "Unknown ID";
        }

        public static string GetUsername(this ClaimsPrincipal user)
        {
            var username = user.Claims.SingleOrDefault(c => c.Type.Equals(ClaimTypes.GivenName));

            return username?.Value ?? "Unknown Username";
        }
    }
}
