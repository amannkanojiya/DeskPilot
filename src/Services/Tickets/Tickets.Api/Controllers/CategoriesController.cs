using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tickets.Api.Models;
using Tickets.Api.UnitOfWork;

namespace Tickets.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public CategoriesController(IUnitOfWork uow) => _uow = uow;

    private Guid TenantId => Guid.Parse(User.FindFirstValue("tenantId")!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _uow.Categories.GetAllAsync(TenantId));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(string name)
    {
        var id = await _uow.Categories.CreateAsync(new Category { TenantId = TenantId, Name = name });
        return Ok(new { id });
    }
}