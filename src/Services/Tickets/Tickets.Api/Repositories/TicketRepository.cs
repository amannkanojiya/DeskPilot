using Dapper;
using Tickets.Api.Data;
using Tickets.Api.Models;

namespace Tickets.Api.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly DapperContext _context;
    public TicketRepository(DapperContext context) => _context = context;

    public async Task<Guid> CreateAsync(Ticket ticket)
    {
        using var conn = _context.CreateConnection();
        var sql = @"INSERT INTO Tickets (Id, TenantId, CustomerId, CategoryId, Subject, Description, Status, Priority)
                     OUTPUT INSERTED.Id
                     VALUES (NEWID(), @TenantId, @CustomerId, @CategoryId, @Subject, @Description, @Status, @Priority)";
        return await conn.QuerySingleAsync<Guid>(sql, ticket);
    }

    public async Task<Ticket?> GetByIdAsync(Guid id, Guid tenantId)
    {
        using var conn = _context.CreateConnection();
        var sql = "SELECT * FROM Tickets WHERE Id = @Id AND TenantId = @TenantId";
        return await conn.QuerySingleOrDefaultAsync<Ticket>(sql, new { Id = id, TenantId = tenantId });
    }

    public async Task<(List<Ticket> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId, string? status, string? priority, Guid? assignedAgentId, int page, int pageSize)
    {
        using var conn = _context.CreateConnection();

        var whereClauses = new List<string> { "TenantId = @TenantId" };
        if (!string.IsNullOrEmpty(status)) whereClauses.Add("Status = @Status");
        if (!string.IsNullOrEmpty(priority)) whereClauses.Add("Priority = @Priority");
        if (assignedAgentId.HasValue) whereClauses.Add("AssignedAgentId = @AssignedAgentId");
        var where = string.Join(" AND ", whereClauses);

        var countSql = $"SELECT COUNT(*) FROM Tickets WHERE {where}";
        var pageSql = $@"SELECT * FROM Tickets WHERE {where}
                          ORDER BY CreatedAt DESC
                          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var parameters = new
        {
            TenantId = tenantId,
            Status = status,
            Priority = priority,
            AssignedAgentId = assignedAgentId,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize
        };

        var totalCount = await conn.ExecuteScalarAsync<int>(countSql, parameters);
        var items = (await conn.QueryAsync<Ticket>(pageSql, parameters)).ToList();

        return (items, totalCount);
    }

    public async Task UpdateStatusAsync(Guid id, Guid tenantId, string newStatus)
    {
        using var conn = _context.CreateConnection();
        var sql = @"UPDATE Tickets SET Status = @NewStatus,
                     ResolvedAt = CASE WHEN @NewStatus = 'Resolved' THEN GETUTCDATE() ELSE ResolvedAt END
                     WHERE Id = @Id AND TenantId = @TenantId";
        await conn.ExecuteAsync(sql, new { Id = id, TenantId = tenantId, NewStatus = newStatus });
    }

    public async Task AssignAsync(Guid id, Guid tenantId, Guid agentId)
    {
        using var conn = _context.CreateConnection();
        var sql = "UPDATE Tickets SET AssignedAgentId = @AgentId WHERE Id = @Id AND TenantId = @TenantId";
        await conn.ExecuteAsync(sql, new { Id = id, TenantId = tenantId, AgentId = agentId });
    }
}