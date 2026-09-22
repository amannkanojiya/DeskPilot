namespace Shared.Contracts.Events;

public record TicketCreated(Guid TicketId, Guid TenantId, string Subject, string Description, DateTime CreatedAt);

public record TicketAssigned(Guid TicketId, Guid TenantId, Guid AgentId);

public record TicketStatusChanged(Guid TicketId, Guid TenantId, string OldStatus, string NewStatus);

public record CommentAdded(Guid TicketId, Guid TenantId, Guid AuthorId, string Body, bool IsInternal);