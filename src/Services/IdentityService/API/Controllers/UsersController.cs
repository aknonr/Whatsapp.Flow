using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Commands;

namespace Whatsapp.Flow.Services.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RegisterUserCommandHandler _registerUserCommandHandler;

        public UsersController(RegisterUserCommandHandler registerUserCommandHandler)
        {
            _registerUserCommandHandler = registerUserCommandHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            try
            {
                await _registerUserCommandHandler.Handle(command);
                return Ok(new { Message = "Kullanıcı başarıyla oluşturuldu." });
            }
            catch (System.Exception ex)
            {
                // Basit hata yönetimi. Gerçek bir uygulamada daha detaylı loglama ve
                // kullanıcı dostu hata mesajları dönülmelidir.
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
} 