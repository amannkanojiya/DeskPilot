namespace Tickets.Api.Dtos;

public record AddCommentRequest(string Body, bool IsInternal);

public record CommentResponse(Guid Id, Guid AuthorId, string Body, bool IsInternal, DateTime CreatedAt);