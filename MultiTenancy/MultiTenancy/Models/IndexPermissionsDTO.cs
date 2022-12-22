namespace MultiTenancy.Models {
    public class IndexPermissionsDTO {
        public string EnterpriseName { get; set; } = null!;
        public IEnumerable<UserDTO> Employees { get; set; } = null!;
    }
}