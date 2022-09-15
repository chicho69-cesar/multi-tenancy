namespace MultiTenancy.Services.Interfaces {
    public interface IChangeTenantService {
        Task ReplaceTenant(Guid EnterpriseId, string userId);
    }
}