using System;
using System.Linq;

namespace expenseTracker.Models
{
    public static class ClaimsIdentityExtensions
    {
        private const string PersistentLoginClaimType = "PersistentLogin";

        public static bool GetIsPersistent(this System.Security.Claims.ClaimsIdentity identity)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == PersistentLoginClaimType) != null;
        }

        public static void SetIsPersistent(this System.Security.Claims.ClaimsIdentity identity, bool isPersistent)
        {
            var claim = identity.Claims.FirstOrDefault(c => c.Type == PersistentLoginClaimType);
            if (isPersistent)
            {
                if (claim == null)
                {
                    identity.AddClaim(new System.Security.Claims.Claim(PersistentLoginClaimType, Boolean.TrueString));
                }
            }
            else if (claim != null)
            {
                identity.RemoveClaim(claim);
            }
        }
    }
}