using Microsoft.AspNetCore.Authorization;
using MultiTenancy.Entities;

namespace MultiTenancy.Security {
    public class HavePermissionRequirement : IAuthorizationRequirement {
        public HavePermissionRequirement(Permissions permissions) {
            Permissions = permissions;
        }
        
        public Permissions Permissions { get; set; }
    }
}