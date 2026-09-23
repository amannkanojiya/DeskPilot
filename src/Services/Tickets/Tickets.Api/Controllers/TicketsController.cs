using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Events;
using System.Security.Claims;
using Tickets.Api.Dtos;
using Tickets.Api.Models;
using Tickets.Api.Services;
using Tickets.Api.UnitOfWork;

namespace Tickets.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _events;

    public TicketsController(IUnitOfWork uow, IMapper mapper, IEventPublisher events)
    {
        _uow = uow;
        _mapper = mapper;
        _events = events;
    }

    private Guid TenantId => Guid.Parse(User.FindFirstValue("tenantId")!);
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
    private string Role => User.FindFirstValue(ClaimTypes.Role) ?? "";

    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketRequest req)
    {
        var ticket = new Ticket
        {
            TenantId = TenantId,
            CustomerId = UserId,
            CategoryId = req.CategoryId,
            Subject = req.Subject,
            Description = req.Description,
            Priority = string.IsNullOrEmpty(req.Priority) ? "Medium" : req.Priority
        };

        var id = await _uow.Tickets.CreateAsync(ticket);

        await _events.PublishAsync(new TicketCreated(id, TenantId, ticket.Subject, ticket.Description, DateTime.UtcNow));

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(id, TenantId);
        if (ticket is null) return NotFound();
        return Ok(_mapper.Map<TicketResponse>(ticket));
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string? status, [FromQuery] string? priority,
        [FromQuery] Guid? assignedAgentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (items, total) = await _uow.Tickets.GetPagedAsync(TenantId, status, priority, assignedAgentId, page, pageSize);
        var mapped = _mapper.Map<List<TicketResponse>>(items);
        return Ok(new PagedResult<TicketResponse>(mapped, total, page, pageSize));
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateStatusRequest req)
    {
        var existing = await _uow.Tickets.GetByIdAsync(id, TenantId);
        if (existing is null) return NotFound();

        await _uow.Tickets.UpdateStatusAsync(id, TenantId, req.NewStatus);
        await _events.PublishAsync(new TicketStatusChanged(id, TenantId, existing.Status, req.NewStatus));

        return NoContent();
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Assign(Guid id, AssignTicketRequest req)
    {
        await _uow.Tickets.AssignAsync(id, TenantId, req.AgentId);
        await _events.PublishAsync(new TicketAssigned(id, TenantId, req.AgentId));
        return NoContent();
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(Guid id, AddCommentRequest req)
    {
        // Customers should never be able to add "internal" (agent-only) notes
        var isInternal = Role == "Customer" ? false : req.IsInternal;

        var comment = new Comment { TicketId = id, AuthorId = UserId, Body = req.Body, IsInternal = isInternal };
        await _uow.Comments.AddAsync(comment);

        await _events.PublishAsync(new CommentAdded(id, TenantId, UserId, req.Body, isInternal));

        return NoContent();
    }

    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetComments(Guid id)
    {
        var includeInternal = Role != "Customer";
        var comments = await _uow.Comments.GetByTicketIdAsync(id, includeInternal);
        return Ok(_mapper.Map<List<CommentResponse>>(comments));
    }
}