using MultiTenancy.Entities;

namespace MultiTenancy.Models {
    public class PermissionUserDTO {
        public Permissions Permission { get; set; }
        public bool ItHas { get; set; }
        public string Description { get; set; }
    }
}