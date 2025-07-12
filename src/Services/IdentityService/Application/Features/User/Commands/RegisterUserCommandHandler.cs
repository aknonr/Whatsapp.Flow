using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using UserEntity = Whatsapp.Flow.Services.Identity.Domain.Entities.User;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Commands
{
    public class RegisterUserCommandHandler
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(RegisterUserCommand command)
        {
            // 1. E-postanın zaten kayıtlı olup olmadığını kontrol et (isteğe bağlı, ama önerilir)
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);
            if (existingUser != null)
            {
                throw new Exception("Bu e-posta adresi zaten kullanılıyor."); // Daha sonra özel exception türleri kullanılacak
            }

            // 2. Parolayı hash'le (Bu kısım daha sonra gerçek bir hash kütüphanesi ile yapılacak)
            var passwordHash = $"hashed_{command.Password}";

            // 3. Yeni kullanıcı nesnesini oluştur
            var user = new UserEntity
            {
                Email = command.Email,
                PasswordHash = passwordHash,
                TenantId = command.TenantId
            };

            // 4. Kullanıcıyı veritabanına ekle
            await _userRepository.AddAsync(user);
            
            // 5. Unit of Work deseni kullanılırsa burada SaveChanges çağrılır.
        }
    }
} 