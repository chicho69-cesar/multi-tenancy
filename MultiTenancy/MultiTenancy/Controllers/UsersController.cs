using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using MultiTenancy.Models;
using MultiTenancy.Services;
using System.Security.Claims;
using MultiTenancy.Data;
using MultiTenancy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy.Controllers {
    public class UsersController : Controller {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IChangeTenantService _changeTenantService;

        public UsersController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            IChangeTenantService changeTenantService
        ) {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _changeTenantService = changeTenantService;
        }

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError(string.Empty, "Hay un error con la informacion proporcionada");
                return View(model);
            }

            var user = new IdentityUser {
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, password: model.Password);

            if (result.Succeeded) {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            } else {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError(string.Empty, "Los datos que proporcionaste no son correctos escribelos de nuevo");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded) {
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                var enterprisesVinculate = await _context.EnterpriseUserPermissions
                    .Where(eup => eup.UserId == user.Id && eup.Permission == Entities.Permissions.Null)
                    .OrderBy(eup => eup.EnterpriseId)
                    .Take(2)
                    .Select(eup => eup.EnterpriseId)
                    .ToListAsync();

                if (enterprisesVinculate.Count == 0) {
                    return RedirectToAction("Index", "Home");
                } else if (enterprisesVinculate.Count == 1) {
                    await _changeTenantService.ReplaceTenant(enterprisesVinculate[0], user.Id);
                    return RedirectToAction("Index", "Home");
                } else {
                    return RedirectToAction("Change", "Enterprises");
                }
            } else {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrectos");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}