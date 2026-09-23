using Tickets.Api.Models;

namespace Tickets.Api.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(Guid tenantId);
    Task<Guid> CreateAsync(Category category);
}