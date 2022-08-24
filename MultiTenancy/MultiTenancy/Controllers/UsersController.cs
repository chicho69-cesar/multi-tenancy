using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Models;
using MultiTenancy.Services;
using System.Security.Claims;

namespace MultiTenancy.Controllers {
    public class UsersController : Controller {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UsersController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager
        ) {
            _userManager = userManager;
            _signInManager = signInManager;
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

            var personalitiesClaims = new List<Claim>() {
                new Claim(Constants.ClaimTenantId, user.Id)
            };

            await _userManager.AddClaimsAsync(user, personalitiesClaims);

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
    }
}