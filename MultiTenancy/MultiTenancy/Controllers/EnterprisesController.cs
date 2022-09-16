using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Entities;
using MultiTenancy.Models;
using MultiTenancy.Services.Interfaces;

namespace MultiTenancy.Controllers {
    [Authorize]
    public class EnterprisesController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IChangeTenantService _changeTenantService;

        public EnterprisesController(
            ApplicationDbContext context,
            IUserService userService,
            IChangeTenantService changeTenantService
        ) {
            _context = context;
            _userService = userService;
            _changeTenantService = changeTenantService;
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEnterpriseViewModel model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError(string.Empty, "Los datos proporcionados no cumplen con las condiciones necesarias");
                return View(model);
            }

            var enterprise = new Enterprise {
                Name = model.Name
            };

            var userId = _userService.GetUserId();
            enterprise.UserCreationId = userId;
            await _context.AddAsync(enterprise);
            await _context.SaveChangesAsync();

            // Le damos todos los permisos al usuario que crea la empresa
            var userEnterprisePermissions = new List<EnterpriseUserPermission>();

            foreach (var permission in Enum.GetValues<Permissions>()) {
                userEnterprisePermissions.Add(new EnterpriseUserPermission {
                    EnterpriseId = enterprise.Id,
                    UserId = userId,
                    Permission = permission
                });
            }

            await _context.AddRangeAsync(userEnterprisePermissions);
            await _context.SaveChangesAsync();

            await _changeTenantService.ReplaceTenant(enterprise.Id, userId);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Change() {
            var userId = _userService.GetUserId();
            
            var enterprises = await _context.EnterpriseUserPermissions
                .Include(eup => eup.Enterprise)
                .Where(eup => eup.UserId == userId)
                .Select(eup => eup.Enterprise!)
                .Distinct()
                .ToListAsync();

            return View(enterprises);
        }

        [HttpPost]
        public async Task<IActionResult> Change(Guid id) {
            var userId = _userService.GetUserId();
            await _changeTenantService.ReplaceTenant(id, userId);
            return RedirectToAction("Index", "Home");
        }
    }
}