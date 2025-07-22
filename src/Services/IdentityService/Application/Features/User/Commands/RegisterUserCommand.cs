using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Commands
{
    public class RegisterUserCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
    }
} 