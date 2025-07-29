using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Commands
{
    public record DeleteUserCommand(string Id) : IRequest;
} 