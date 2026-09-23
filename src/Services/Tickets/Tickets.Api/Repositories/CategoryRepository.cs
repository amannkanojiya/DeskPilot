using Dapper;
using Tickets.Api.Data;
using Tickets.Api.Models;

namespace Tickets.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DapperContext _context;
    public CategoryRepository(DapperContext context) => _context = context;

    public async Task<List<Category>> GetAllAsync(Guid tenantId)
    {
        using var conn = _context.CreateConnection();
        var sql = "SELECT * FROM Categories WHERE TenantId = @TenantId ORDER BY Name";
        return (await conn.QueryAsync<Category>(sql, new { TenantId = tenantId })).ToList();
    }

    public async Task<Guid> CreateAsync(Category category)
    {
        using var conn = _context.CreateConnection();
        var sql = "INSERT INTO Categories (Id, TenantId, Name) OUTPUT INSERTED.Id VALUES (NEWID(), @TenantId, @Name)";
        return await conn.QuerySingleAsync<Guid>(sql, category);
    }
}