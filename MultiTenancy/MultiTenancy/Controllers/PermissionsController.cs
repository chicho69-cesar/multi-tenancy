using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Entities;
using MultiTenancy.Models;
using MultiTenancy.Security;
using MultiTenancy.Services.Interfaces;
using MultiTenancy.Validations;
using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Controllers {
    [Authorize]
    public class PermissionsController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly ITenantService _tenantService;

        public PermissionsController(
            ApplicationDbContext context,
            ITenantService tenantService
        ) {
            _context = context;
            _tenantService = tenantService;
        }

        [HttpGet]
        [HavePermission(Permissions.Product_Read)]
        public async Task<IActionResult> Index() {
            var tenantId = new Guid(_tenantService.GetTenant());

            var model = await _context.Enterprises
                .Include(e => e.EnterpriseUserPermissions)
                .ThenInclude(e => e.User)
                .Where(e => e.Id == tenantId)
                .Select(e => new IndexPermissionsDTO {
                    EnterpriseName = e.Name,
                    Employees = e.EnterpriseUserPermissions
                        .Select(eu => new UserDTO {
                            Email = eu.User!.Email
                        }).Distinct()
                }).FirstOrDefaultAsync();

            return View(model);
        }

        [HttpGet]
        [HavePermission(Permissions.Product_Read)]
        public async Task<IActionResult> Administrate(string email) {
            var tenantId = new Guid(_tenantService.GetTenant());
            var userId = await _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == null) {
                return RedirectToAction(nameof(Index));
            }

            var permissions = await _context.EnterpriseUserPermissions
                .Where(e => e.EnterpriseId == tenantId
                    && e.UserId == userId && e.Permission != Permissions.Null)
                .ToListAsync();

            var permissionsUserDictionary = permissions
                .ToDictionary(p => p.Permission);

            var model = new AdministratePermissionsDTO {
                UserId = userId,
                Email = email
            };

            foreach (var permission in Enum.GetValues<Permissions>()) {
                var field = typeof(Permissions).GetField(permission.ToString())!;
                var hide = field.IsDefined(typeof(HideAttribute), false);

                if (hide) continue;

                var description = permission.ToString();

                if (field.IsDefined(typeof(DisplayAttribute), false)) {
                    var displayAtrr = (DisplayAttribute)Attribute
                        .GetCustomAttribute(field, typeof(DisplayAttribute));
                    description = displayAtrr.Description;
                }

                model.Permissions.Add(new PermissionUserDTO {
                    Description = description,
                    Permission = permission,
                    ItHas = permissionsUserDictionary.ContainsKey(permission)
                });
            }

            return View(model);
        }

        [HttpPost]
        [HavePermission(Permissions.Permissions_Update)]
        public async Task<IActionResult> Administrate(AdministratePermissionsDTO model) {
            var tenantId = new Guid(_tenantService.GetTenant());

            // Siempre agregamos el permiso por defecto
            model.Permissions.Add(new PermissionUserDTO {
                ItHas = true,
                Permission = Permissions.Null
            });

            // Borramos los permisos de la persona
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                DELETE FROM EnterpriseUserPermissions 
                WHERE UserId = { model.UserId } AND 
                EnterpriseId = { tenantId }
            ");

            // Filtramos los permisos a cargar
            var permissionsFiltereds = model.Permissions
                .Where(p => p.ItHas)
                .Select(p => new EnterpriseUserPermission {
                    EnterpriseId = tenantId,
                    UserId = model.UserId,
                    Permission = p.Permission
                });

            _context.AddRange(permissionsFiltereds);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}