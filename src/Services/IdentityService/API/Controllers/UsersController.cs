using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Commands;

namespace Whatsapp.Flow.Services.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { Message = "Kullanıcı başarıyla oluşturuldu." });
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token });
        }
    }
} 