using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;
using MultiTenancy.Security;
using MultiTenancy.Services;
using MultiTenancy.Services.Interfaces;

namespace MultiTenancy {
    public class StartUp {
        public static WebApplication InitializeApplication(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            var app = builder.Build();
            ConfigureMiddlewares(app);
            return app;
        }

        private static void ConfigureServices(WebApplicationBuilder builder) {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => 
                options.SignIn.RequireConfirmedAccount = false
            )
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options => {
                options.LoginPath = new PathString("/users/login");
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddTransient<ITenantService, TenantService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddScoped<IChangeTenantService, ChangeTenantService>();
            builder.Services.AddScoped<IAuthorizationHandler, HavePermissionHandler>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, HavePermissionPolicyProvider>();
        }

        private static void ConfigureMiddlewares(WebApplication app) {
            if (app.Environment.IsDevelopment()) {
                app.UseMigrationsEndPoint();
            } else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.MapRazorPages();
        }
    }
}