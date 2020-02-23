using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Web.Filters
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(AuthorizationFilter))
        {
            Arguments = new object[] {new Claim(claimType, claimValue) };
        }
    }
}