using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Commands
{
    public class LoginCommand : IRequest<string>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
} 