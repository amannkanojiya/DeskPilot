namespace Identity.Api.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = default!; // Admin | Agent | Customer
    public DateTime CreatedAt { get; set; }
}