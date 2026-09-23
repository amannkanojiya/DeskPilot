using Dapper;
using Tickets.Api.Data;
using Tickets.Api.Models;

namespace Tickets.Api.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DapperContext _context;
    public CommentRepository(DapperContext context) => _context = context;

    public async Task<Guid> AddAsync(Comment comment)
    {
        using var conn = _context.CreateConnection();
        var sql = @"INSERT INTO Comments (Id, TicketId, AuthorId, Body, IsInternal)
                     OUTPUT INSERTED.Id
                     VALUES (NEWID(), @TicketId, @AuthorId, @Body, @IsInternal)";
        return await conn.QuerySingleAsync<Guid>(sql, comment);
    }

    public async Task<List<Comment>> GetByTicketIdAsync(Guid ticketId, bool includeInternal)
    {
        using var conn = _context.CreateConnection();
        var sql = includeInternal
            ? "SELECT * FROM Comments WHERE TicketId = @TicketId ORDER BY CreatedAt"
            : "SELECT * FROM Comments WHERE TicketId = @TicketId AND IsInternal = 0 ORDER BY CreatedAt";
        return (await conn.QueryAsync<Comment>(sql, new { TicketId = ticketId })).ToList();
    }
}