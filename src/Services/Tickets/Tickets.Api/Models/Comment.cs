namespace Tickets.Api.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid AuthorId { get; set; }
    public string Body { get; set; } = default!;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
}