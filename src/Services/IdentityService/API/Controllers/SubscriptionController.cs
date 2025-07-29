using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.API.Security;
using Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Queries;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;
using Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Commands;
using Microsoft.AspNetCore.Http;

namespace Whatsapp.Flow.Services.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubscriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("tenant/{tenantId}")]
        [HasRole(Role.SuperAdmin)] // Sadece SuperAdmin'ler herhangi bir tenant'ın aboneliğini görebilir
        public async Task<IActionResult> GetByTenantId(string tenantId)
        {
            var query = new GetSubscriptionByTenantIdQuery(tenantId);
            var subscription = await _mediator.Send(query);
            return Ok(subscription);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscription()
        {
            // Token'dan gelen kullanıcının tenantId'sini al
            var tenantId = User.FindFirstValue("tenantId");

            if (string.IsNullOrEmpty(tenantId))
            {
                return BadRequest("Tenant ID not found in token.");
            }

            var query = new GetSubscriptionByTenantIdQuery(tenantId);
            var subscription = await _mediator.Send(query);
            return Ok(subscription);
        }

        [HttpPut("tenant/{tenantId}")]
        [HasRole(Role.SuperAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateSubscription(string tenantId, [FromBody] UpdateSubscriptionCommand command)
        {
            command.TenantId = tenantId;
            await _mediator.Send(command);
            return NoContent();
        }
    }
} 