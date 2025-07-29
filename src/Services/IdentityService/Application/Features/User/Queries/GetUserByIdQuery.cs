using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Queries
{
    public record GetUserByIdQuery(string Id) : IRequest<UserDto>;
} 