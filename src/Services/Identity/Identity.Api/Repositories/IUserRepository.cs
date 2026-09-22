using Identity.Api.Models;

namespace Identity.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, Guid tenantId);
    Task<Guid> CreateTenantAsync(string tenantName);
    Task<Guid> CreateUserAsync(User user);
}