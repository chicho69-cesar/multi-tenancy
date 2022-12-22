using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Entities;
using MultiTenancy.Models;
using MultiTenancy.Security;
using System.Diagnostics;

namespace MultiTenancy.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context) {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var model = await BuildModelHomeIndex();
            return View(model);
        }

        [HttpPost]
        [HavePermission(Permissions.Products_Create)]
        public async Task<IActionResult> Index(Product product) {
            await _context.AddAsync(product);
            await _context.SaveChangesAsync();
            var model = await BuildModelHomeIndex();
            return View(model);
        }

        [HttpGet]
        public IActionResult Privacy() {
            return View();
        }

        private async Task<HomeIndexViewModel> BuildModelHomeIndex() {
            var products = await _context.Products
                .ToListAsync();

            var countries = await _context.Countries
                .ToListAsync();

            var model = new HomeIndexViewModel {
                Products = products,
                Countries = countries
            };

            return model;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}