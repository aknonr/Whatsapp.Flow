using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands;

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
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
        {
            var tenantId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTenantById), new { id = tenantId }, null);
        }

        // Bu metod şimdilik sadece CreatedAtAction'ın çalışması için bir iskelet.
        // Gelecekte GetTenantByIdQuery oluşturulunca doldurulacak.
        [HttpGet("{id}")]
        public IActionResult GetTenantById(string id)
        {
            return Ok($"Tenant {id}");
        }
    }
} 