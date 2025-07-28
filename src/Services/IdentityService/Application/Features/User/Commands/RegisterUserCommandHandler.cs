using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Interfaces;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using UserEntity = Whatsapp.Flow.Services.Identity.Domain.Entities.User;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("Bu e-posta adresi zaten kullanılıyor.");
            }

            var passwordHash = _passwordService.HashPassword(request.Password);

            var user = new UserEntity
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                TenantId = request.TenantId,
                SystemRole = Domain.Entities.Role.User, // Role -> SystemRole olarak değiştirildi
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }
    }
} 