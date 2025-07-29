using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Commands;
using Microsoft.AspNetCore.RateLimiting;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Queries;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Whatsapp.Flow.Services.Identity.Application.Common.Security;
using Whatsapp.Flow.Services.Identity.API.Security;

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
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("login")]
        [EnableRateLimiting("login_policy")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Gets a list of all users for the current user's tenant.
        /// </summary>
        /// <returns>A list of users.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="403">If the user does not have 'users.read' permission.</response>
        [HttpGet]
        [HasPermission("users.read")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _mediator.Send(new GetUsersForTenantQuery());
            return Ok(users);
        }
    }
} 