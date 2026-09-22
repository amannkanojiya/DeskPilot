using Identity.Api.Models;
using Identity.Api.Repositories;
using Identity.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Dtos;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;

    public AuthController(IUserRepository users, ITokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("register-tenant")]
    public async Task<IActionResult> RegisterTenant(RegisterTenantRequest req)
    {
        var tenantId = await _users.CreateTenantAsync(req.TenantName);

        var user = new User
        {
            TenantId = tenantId,
            Email = req.AdminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.AdminPassword),
            Role = "Admin"
        };
        await _users.CreateUserAsync(user);

        return Ok(new { tenantId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req, [FromQuery] Guid tenantId)
    {
        var user = await _users.GetByEmailAsync(req.Email, tenantId);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _tokens.GenerateAccessToken(user);
        return Ok(new AuthResponse(token, "", DateTime.UtcNow.AddHours(2)));
    }
}