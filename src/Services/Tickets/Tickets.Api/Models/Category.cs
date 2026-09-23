namespace Tickets.Api.Models;

public class Category
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}