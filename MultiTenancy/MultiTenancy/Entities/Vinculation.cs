using Microsoft.AspNetCore.Identity;
using MultiTenancy.Entities.Interfaces;

namespace MultiTenancy.Entities {
    public class Vinculation : ICommonEntitie {
        public int Id { get; set; }
        public Guid EnterpriseId { get; set; }
        public string UserId { get; set; }
        public VinculationStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public Enterprise Enterprise { get; set; }
        public IdentityUser User { get; set; }
    }
}