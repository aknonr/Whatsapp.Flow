using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.API.Security;
using Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands;
using Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Queries;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Identity.API.Controllers
{
    [ApiController]
    [Route("api/tenant-roles")]
    [Authorize]
    public class TenantRolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantRolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all roles for the current user's tenant.
        /// </summary>
        /// <returns>A list of roles within the tenant.</returns>
        /// <response code="200">Returns the list of roles.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have 'roles.read' permission.</response>
        [HttpGet]
        [HasPermission("roles.read")]
        [ProducesResponseType(typeof(IEnumerable<TenantRoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetRolesForTenant()
        {
            var roles = await _mediator.Send(new GetRolesForTenantQuery());
            return Ok(roles);
        }

        /// <summary>
        /// Creates a new role within the current user's tenant.
        /// </summary>
        /// <param name="command">The details of the role to create.</param>
        /// <returns>The ID of the newly created role.</returns>
        /// <response code="201">Returns the newly created role's ID.</response>
        /// <response code="400">If the command is invalid.</response>
        [HttpPost]
        [HasPermission("roles.create")] 
        public async Task<IActionResult> CreateRole([FromBody] CreateTenantRoleCommand command)
        {
            var roleId = await _mediator.Send(command);
            // Daha sonra GetRoleById endpoint'i oluşturulunca ona yönlendirilecek.
            return CreatedAtAction(null, new { id = roleId }, new { id = roleId });
        }

        [HttpPost("assign")]
        [HasPermission("roles.assign")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing role within the current user's tenant.
        /// </summary>
        /// <param name="id">The ID of the role to update.</param>
        /// <param name="command">The updated details for the role.</param>
        /// <returns>A 204 No Content response indicating success.</returns>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the command is invalid.</response>
        /// <response code="404">If the role is not found.</response>
        /// <response code="403">If the user does not have 'roles.update' permission.</response>
        [HttpPut("{id}")]
        [HasPermission("roles.update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateTenantRoleCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific role within the current user's tenant.
        /// </summary>
        /// <remarks>
        /// System roles and roles that are currently assigned to users cannot be deleted.
        /// </remarks>
        /// <param name="id">The ID of the role to delete.</param>
        /// <returns>A 204 No Content response indicating success.</returns>
        /// <response code="204">If the deletion was successful.</response>
        /// <response code="404">If the role is not found.</response>
        /// <response code="400">If the role is a system role or is in use.</response>
        /// <response code="403">If the user does not have 'roles.delete' permission.</response>
        [HttpDelete("{id}")]
        [HasPermission("roles.delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            await _mediator.Send(new DeleteTenantRoleCommand(id));
            return NoContent();
        }
    }
} 