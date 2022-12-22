using Microsoft.AspNetCore.Authorization;
using MultiTenancy.Entities;
using MultiTenancy.Services;

namespace MultiTenancy.Security {
    public class HavePermissionAttribute : AuthorizeAttribute {
        public HavePermissionAttribute(Permissions permissions) {
            Permissions = permissions;
        }

        public Permissions Permissions { 
            get {
                // havePermissionProducts_Create -> Asi es como guardamos las politicas
                if (Enum.TryParse(typeof(Permissions), Policy!.Substring(Constants.PrefixPolicy.Length), ignoreCase: true, out var permission)) {
                    return (Permissions)permission!;
                }

                return Permissions.Null;
            }

            set {
                Policy = $"{ Constants.PrefixPolicy }{ value }";
            } 
        }
    }
}