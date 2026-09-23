using Tickets.Api.Dtos;
using Tickets.Api.Models;

namespace Tickets.Api.Repositories;

public interface ITicketRepository
{
    Task<Guid> CreateAsync(Ticket ticket);
    Task<Ticket?> GetByIdAsync(Guid id, Guid tenantId);
    Task<(List<Ticket> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId, string? status, string? priority, Guid? assignedAgentId, int page, int pageSize);
    Task UpdateStatusAsync(Guid id, Guid tenantId, string newStatus);
    Task AssignAsync(Guid id, Guid tenantId, Guid agentId);
}