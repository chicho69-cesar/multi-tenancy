using Microsoft.AspNetCore.Authorization;
using MultiTenancy.Entities;
using MultiTenancy.Services;

namespace MultiTenancy.Security {
    public class HavePermissionPolicyProvider : IAuthorizationPolicyProvider {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() {
            return Task.FromResult(
                new AuthorizationPolicyBuilder("Identity.Application")
                    .RequireAuthenticatedUser()
                    .Build()
            );
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() {
            return Task.FromResult<AuthorizationPolicy>(null!);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName) {
            if (
                policyName.StartsWith(Constants.PrefixPolicy, StringComparison.OrdinalIgnoreCase) && 
                Enum.TryParse(typeof(Permissions), policyName[Constants.PrefixPolicy.Length..], out var permissionObj)
            ) {
                var permission = (Permissions)permissionObj!;
                var policy = new AuthorizationPolicyBuilder("Identity.Application");
                policy.AddRequirements(new HavePermissionRequirement(permission));

                return Task.FromResult<AuthorizationPolicy>(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy>(null!);
        }
    }
}