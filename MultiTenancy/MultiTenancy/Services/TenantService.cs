using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using MultiTenancy.Services.Interfaces;

namespace MultiTenancy.Services {
    public class TenantService : ITenantService {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenant() {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null) {
                return string.Empty;
            }

            var authTicket = DecryptAuthCookie(httpContext);

            if (authTicket is null) {
                return string.Empty;
            }

            var claimTenant = authTicket.Principal.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTenantId);

            if (claimTenant is null) {
                return string.Empty;
            }

            return claimTenant.Value;
        }

        private static AuthenticationTicket DecryptAuthCookie(HttpContext httpContext) {
            var opt = httpContext.RequestServices
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get("Identity.Application");

            var cookie = opt.CookieManager.GetRequestCookie(httpContext, opt.Cookie.Name!);

            return opt.TicketDataFormat.Unprotect(cookie);
        }
    }
}