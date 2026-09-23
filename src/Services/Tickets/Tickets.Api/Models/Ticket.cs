namespace Tickets.Api.Models;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? AssignedAgentId { get; set; }
    public Guid? CategoryId { get; set; }
    public string Subject { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "Medium";
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}