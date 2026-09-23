using Tickets.Api.Repositories;

namespace Tickets.Api.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    ITicketRepository Tickets { get; }
    ICommentRepository Comments { get; }
    ICategoryRepository Categories { get; }
    Task<int> CompleteAsync();
}