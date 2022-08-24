using Microsoft.AspNetCore.Identity;
using MultiTenancy.Entities.Interfaces;

namespace MultiTenancy.Services {
    public static class TypeExtenssions {
        public static bool ShouldSkipTenantValidation(this Type t) {
            var bools = new List<bool> {
                t.IsAssignableFrom(typeof(IdentityRole)),
                t.IsAssignableFrom(typeof(IdentityRoleClaim<string>)),
                t.IsAssignableFrom(typeof(IdentityUser)),
                t.IsAssignableFrom(typeof(IdentityUserLogin<string>)),
                t.IsAssignableFrom(typeof(IdentityUserRole<string>)),
                t.IsAssignableFrom(typeof(IdentityUserToken<string>)),
                t.IsAssignableFrom(typeof(IdentityUserClaim<string>)),
                typeof(ICommonEntitie).IsAssignableFrom(t)
            };

            var result = bools.Aggregate((b1, b2) => b1 || b2);

            return result;
        }
    }
}