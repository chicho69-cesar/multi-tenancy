using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Services.Interfaces;
using System.Security.Claims;

namespace MultiTenancy.Services {
    public class ChangeTenantService : IChangeTenantService {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ChangeTenantService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context
        ) {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task ReplaceTenant(Guid EnterpriseId, string userId) {
            var user = await _userManager.FindByIdAsync(userId);

            var claimTenantExists = await _context.UserClaims
                .FirstOrDefaultAsync(
                    c => c.ClaimType == Constants.ClaimTenantId 
                    && c.UserId == user.Id
                );

            if (claimTenantExists is not null) {
                _context.Remove(claimTenantExists);
            }

            var newClaimTenant = new Claim(Constants.ClaimTenantId, EnterpriseId.ToString());
            await _userManager.AddClaimAsync(user, newClaimTenant);
            await _signInManager.SignInAsync(user, isPersistent: true);
        }
    }
}