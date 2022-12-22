using Microsoft.AspNetCore.Authorization;
using MultiTenancy.Entities;
using MultiTenancy.Services;
using System.Security.Claims;

namespace MultiTenancy.Security {
    public static class IAuthorizationServiceExtensions {
        public static async Task<bool> HavePermission(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user, Permissions permission
        ) {
            if (!user.Identity!.IsAuthenticated) {
                return false;
            }

            var policyName = $"{ Constants.PrefixPolicy }{ permission }";
            var result = await authorizationService
                .AuthorizeAsync(user, policyName);
            
            return result.Succeeded;
        }
    }
}