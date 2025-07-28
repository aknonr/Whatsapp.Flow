using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.API.Security;
using Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands;
using Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Queries;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [HasRole(Role.SuperAdmin)]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
        {
            var tenantId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTenantById), new { id = tenantId }, null);
        }

        [HttpGet("{id}")]
        [HasRole(Role.SuperAdmin)]
        public async Task<IActionResult> GetTenantById(string id)
        {
            var query = new GetTenantByIdQuery(id);
            var tenant = await _mediator.Send(query);
            return Ok(tenant);
        }

        [HttpPut("{id}")]
        [HasRole(Role.SuperAdmin)]
        public async Task<IActionResult> UpdateTenant(string id, [FromBody] UpdateTenantCommand command)
        {
            if (id != command.Id)
            {
                // Normalde id'yi command'a set edip devam ederiz ama güvenlik için kontrol de eklenebilir.
                command.Id = id;
            }

            await _mediator.Send(command);
            return NoContent(); // Başarılı güncelleme sonrası 204 No Content dönmek standarttır.
        }

        [HttpDelete("{id}")]
        [HasRole(Role.SuperAdmin)]
        public async Task<IActionResult> DeleteTenant(string id)
        {
            var command = new DeleteTenantCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
} 