namespace Tickets.Api.Dtos;

public record CreateTicketRequest(string Subject, string Description, Guid? CategoryId, string Priority);

public record UpdateStatusRequest(string NewStatus);

public record AssignTicketRequest(Guid AgentId);

public record TicketResponse(
    Guid Id, Guid CustomerId, Guid? AssignedAgentId, Guid? CategoryId,
    string Subject, string Description, string Status, string Priority,
    DateTime CreatedAt, DateTime? ResolvedAt);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);