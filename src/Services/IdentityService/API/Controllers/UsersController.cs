using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Commands;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Queries;
using Whatsapp.Flow.Services.Identity.API.Security;
using Asp.Versioning;

namespace Whatsapp.Flow.Services.Identity.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        /// <summary>
        /// Updates a specific user.
        /// </summary>
        /// <remarks>
        /// Updates a user's first name, last name, status, and assigned tenant roles.
        /// </remarks>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="command">The user update data.</param>
        /// <returns>A 204 No Content response indicating success.</returns>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the command data is invalid.</response>
        /// <response code="404">If the user is not found in the tenant.</response>
        /// <response code="403">If the user does not have 'users.update' permission.</response>
        [HttpPut("{id}")]
        [HasPermission("users.update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Deletes (soft) a specific user.
        /// </summary>
        /// <remarks>
        /// This operation does not permanently delete the user, but marks them as inactive.
        /// You cannot delete your own account.
        /// </remarks>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A 204 No Content response indicating success.</returns>
        /// <response code="204">If the deletion was successful.</response>
        /// <response code="400">If a user tries to delete their own account.</response>
        /// <response code="404">If the user is not found in the tenant.</response>
        /// <response code="403">If the user does not have 'users.delete' permission.</response>
        [HttpDelete("{id}")]
        [HasPermission("users.delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _mediator.Send(new DeleteUserCommand(id));
            return NoContent();
        }
    }
} 