namespace Shared.Contracts.Dtos;

public record RegisterTenantRequest(string TenantName, string AdminEmail, string AdminPassword);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);