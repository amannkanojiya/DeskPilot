using Dapper;
using Identity.Api.Data;
using Identity.Api.Models;

namespace Identity.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;
    public UserRepository(DapperContext context) => _context = context;

    public async Task<User?> GetByEmailAsync(string email, Guid tenantId)
    {
        using var conn = _context.CreateConnection();
        var sql = "SELECT * FROM Users WHERE Email = @Email AND TenantId = @TenantId";
        return await conn.QuerySingleOrDefaultAsync<User>(sql, new { Email = email, TenantId = tenantId });
    }

    public async Task<Guid> CreateTenantAsync(string tenantName)
    {
        using var conn = _context.CreateConnection();
        var sql = "INSERT INTO Tenants (Id, Name) OUTPUT INSERTED.Id VALUES (NEWID(), @Name)";
        return await conn.QuerySingleAsync<Guid>(sql, new { Name = tenantName });
    }

    public async Task<Guid> CreateUserAsync(User user)
    {
        using var conn = _context.CreateConnection();
        var sql = @"INSERT INTO Users (Id, TenantId, Email, PasswordHash, Role)
                     OUTPUT INSERTED.Id
                     VALUES (NEWID(), @TenantId, @Email, @PasswordHash, @Role)";
        return await conn.QuerySingleAsync<Guid>(sql, user);
    }
}