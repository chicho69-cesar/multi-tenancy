using Microsoft.AspNetCore.Identity;
using MultiTenancy.Entities.Interfaces;

namespace MultiTenancy.Entities {
    public class Enterprise : ICommonEntitie {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserCreationId { get; set; }
        public IdentityUser UserCreation { get; set; }
        public List<EnterpriseUserPermission> enterpriseUserPermissions { get; set; }
    }
}