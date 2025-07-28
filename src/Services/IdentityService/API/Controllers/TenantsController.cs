using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.API.Security;
using Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands;
using Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Queries;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Whatsapp.Flow.Services.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new Tenant.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only by SuperAdmins.
        /// When a new tenant is created, a default 14-day trial subscription is also initiated.
        /// </remarks>
        /// <param name="command">The details for the new tenant.</param>
        /// <returns>A 201 Created response with the location and details of the newly created tenant.</returns>
        /// <response code="201">Returns the newly created tenant's details.</response>
        /// <response code="400">If the command is invalid (e.g., missing required fields).</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not a SuperAdmin.</response>
        [HttpPost]
        [HasRole(Role.SuperAdmin)]
        [ProducesResponseType(typeof(TenantDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
        {
            var tenantId = await _mediator.Send(command);
            var tenantDto = await _mediator.Send(new GetTenantByIdQuery(tenantId));
            return CreatedAtAction(nameof(GetTenantById), new { id = tenantId }, tenantDto);
        }

        /// <summary>
        /// Gets a specific Tenant by its ID.
        /// </summary>
        /// <param name="id">The ID of the tenant to retrieve.</param>
        /// <returns>The details of the tenant.</returns>
        /// <response code="200">Returns the requested tenant.</response>
        /// <response code="404">If the tenant with the specified ID is not found.</response>
        [HttpGet("{id}")]
        [HasRole(Role.SuperAdmin)]
        [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTenantById(string id)
        {
            var query = new GetTenantByIdQuery(id);
            var tenant = await _mediator.Send(query);
            return Ok(tenant);
        }

        /// <summary>
        /// Updates an existing Tenant.
        /// </summary>
        /// <param name="id">The ID of the tenant to update.</param>
        /// <param name="command">The updated details for the tenant.</param>
        /// <returns>A 204 No Content response indicating success.</returns>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the command is invalid.</response>
        /// <response code="404">If the tenant is not found.</response>
        [HttpPut("{id}")]
        [HasRole(Role.SuperAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTenant(string id, [FromBody] UpdateTenantCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Soft deletes a specific Tenant.
        /// </summary>
        /// <remarks>
        /// This performs a soft delete, marking the tenant as inactive rather than physically removing it. The user performing the action is recorded.
        /// </remarks>
        /// <param name="id">The ID of the tenant to delete.</param>
        /// <returns>A 204 No Content response indicating success.</returns>
        /// <response code="204">If the deletion was successful.</response>
        /// <response code="404">If the tenant is not found.</response>
        [HttpDelete("{id}")]
        [HasRole(Role.SuperAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTenant(string id)
        {
            var command = new DeleteTenantCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
} 