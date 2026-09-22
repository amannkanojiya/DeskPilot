namespace Identity.Api.Models;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}