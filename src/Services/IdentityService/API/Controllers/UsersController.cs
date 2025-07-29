using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Commands;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Queries;
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

        /// <summary>
        /// Gets a specific user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user's data.</returns>
        /// <response code="200">Returns the specified user's data.</response>
        /// <response code="404">If the user is not found in the tenant.</response>
        /// <response code="403">If the user does not have 'users.read' permission.</response>
        [HttpGet("{id}")]
        [HasPermission("users.read")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
            return Ok(user);
        }
    }
} 