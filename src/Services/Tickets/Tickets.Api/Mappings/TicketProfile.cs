using AutoMapper;
using Tickets.Api.Dtos;
using Tickets.Api.Models;

namespace Tickets.Api.Mappings;

public class TicketProfile : Profile
{
    public TicketProfile()
    {
        CreateMap<Ticket, TicketResponse>();

        CreateMap<Comment, CommentResponse>();
    }
}