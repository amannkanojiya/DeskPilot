using Tickets.Api.Data;
using Tickets.Api.Repositories;

namespace Tickets.Api.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public ITicketRepository Tickets { get; }
    public ICommentRepository Comments { get; }
    public ICategoryRepository Categories { get; }

    public UnitOfWork(DapperContext context)
    {
        Tickets = new TicketRepository(context);
        Comments = new CommentRepository(context);
        Categories = new CategoryRepository(context);
    }

    // Dapper repositories open/close their own short-lived connections per call,
    // which is the standard Dapper pattern. CompleteAsync exists here so controllers
    // have one consistent "unit of work" entry point, and so later you can evolve this
    // into a shared-transaction implementation without changing controller code.
    public Task<int> CompleteAsync() => Task.FromResult(1);

    public void Dispose() { }
}