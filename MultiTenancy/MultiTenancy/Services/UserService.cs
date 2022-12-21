using MultiTenancy.Services.Interfaces;
using System.Security.Claims;

namespace MultiTenancy.Services {
    public class UserService : IUserService {
        private readonly HttpContext _httpContext;
        
        public UserService(IHttpContextAccessor httpContextAccessor) {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public string GetUserId() {
            if (_httpContext.User.Identity.IsAuthenticated) {
                var claim = _httpContext.User.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .FirstOrDefault();

                if (claim is null) {
                    throw new ApplicationException("El usuario no tiene el claim del ID");
                }

                return claim.Value;
            } else {
                throw new ApplicationException("El usuario no está autenticado");
            }
        }
    }
}
