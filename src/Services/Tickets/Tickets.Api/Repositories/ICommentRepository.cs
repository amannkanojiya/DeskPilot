using Tickets.Api.Models;

namespace Tickets.Api.Repositories;

public interface ICommentRepository
{
    Task<Guid> AddAsync(Comment comment);
    Task<List<Comment>> GetByTicketIdAsync(Guid ticketId, bool includeInternal);
}