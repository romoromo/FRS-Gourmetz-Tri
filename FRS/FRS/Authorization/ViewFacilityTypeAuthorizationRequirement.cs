using DAL.Core;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRS.Authorization
{
    public class ViewFacilityAuthorizationRequirement : IAuthorizationRequirement
    {

    }



    public class ViewFacilityAuthorizationHandler : AuthorizationHandler<ViewFacilityAuthorizationRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewFacilityAuthorizationRequirement requirement, string roleName)
        {
            if (context.User == null)
                return Task.CompletedTask;

            if (context.User.HasClaim(CustomClaimTypes.Permission, ApplicationPermissionsTrees.RBFacilitiesMenu) || context.User.IsInRole(roleName))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
