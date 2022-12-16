using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Entities;
using MultiTenancy.Models;
using MultiTenancy.Services.Interfaces;

namespace MultiTenancy.Controllers {
    [Authorize]
    public class VinculationsController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ITenantService _tenantService;

        public VinculationsController(
            ApplicationDbContext context, 
            IUserService userService, 
            ITenantService tenantService
        ) {
            _context = context;
            _userService = userService;
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var userId = _userService.GetUserId();
            return await ReturnPendingVinculations(userId);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Guid enterpriseId, VinculationStatus vinculationStatus) {
            var userId = _userService.GetUserId();
            var vinculation = await _context.Vinculations
                .FirstOrDefaultAsync(v => v.UserId == userId
                    && v.EnterpriseId == enterpriseId
                    && v.Status == VinculationStatus.Pending);

            if (vinculation == null) {
                ModelState.AddModelError("", "Ha ocurrido un error: Vinculacion no encontrada");
                return await ReturnPendingVinculations(userId);
            }

            if (vinculationStatus == VinculationStatus.Accept) {
                var nullPermission = new EnterpriseUserPermission {
                    Permission = Permissions.Null,
                    EnterpriseId = enterpriseId,
                    UserId = userId
                };

                _context.Add(nullPermission);
            }

            vinculation.Status = vinculationStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction("Change", "Enterprises");
        }

        [HttpGet]
        public async Task<IActionResult> Vinculate() {
            var enterpriseId = _tenantService.GetTenant();

            if (string.IsNullOrEmpty(enterpriseId)) {
                return RedirectToAction("Index", "Home");
            }

            var enterpriseIdGuid = new Guid(enterpriseId);

            var enterprise = await _context.Enterprises
                .FirstOrDefaultAsync(e => e.Id== enterpriseIdGuid);

            if (enterprise == null) {
                return RedirectToAction("Index", "Home");
            }

            var model = new VinculateUserViewModel {
                EnterpriseId = enterprise.Id,
                EnterpriseName = enterprise.Name
            };

            return View(model);
        }

        // TODO: Aqui voy

        private async Task<IActionResult> ReturnPendingVinculations(string userId) {
            var pendingVinculations = await _context.Vinculations
                .Include(v => v.Enterprise)
                .Where(v => v.Status == VinculationStatus.Pending && v.UserId == userId)
                .ToListAsync();

            return View(pendingVinculations);
        }
    }
}