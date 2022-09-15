using Microsoft.AspNetCore.Identity;
using MultiTenancy.Entities.Interfaces;

namespace MultiTenancy.Entities {
    public class EnterpriseUserPermission : ICommonEntitie {
        public string UserId { get; set; }
        public Guid EnterpriseId { get; set; }
        public Permissions Permission { get; set; }
        public IdentityUser User { get; set; }
        public Enterprise Enterprise { get; set; }
    }
}