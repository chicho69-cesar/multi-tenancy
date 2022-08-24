using MultiTenancy.Entities.Interfaces;

namespace MultiTenancy.Entities {
    public class Product : ITenantEntitie {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TenantId { get; set; }
    }
}