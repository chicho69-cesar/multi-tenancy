using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Services.Interfaces;

namespace MultiTenancy.Security {
    public class HavePermissionHandler : AuthorizationHandler<HavePermissionRequirement> {
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _dbContext;

        public HavePermissionHandler(
            ITenantService tenantService,
            IUserService userService,
            ApplicationDbContext dbContext
        ) {
            _tenantService = tenantService;
            _userService = userService;
            _dbContext = dbContext;
        }
        
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            HavePermissionRequirement requirement
        ) {
            var permission = requirement.Permissions;
            var userId = _userService.GetUserId();
            var tenantId = new Guid(_tenantService.GetTenant());

            var havePermission = await _dbContext.EnterpriseUserPermissions
                .AnyAsync(e => e.UserId == userId
                    && e.EnterpriseId == tenantId && e.Permission == permission);

            if (havePermission) {
                context.Succeed(requirement);
            }
        }
    }
}