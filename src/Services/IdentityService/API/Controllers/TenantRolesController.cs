using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.API.Security;
using Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands;

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
    }
} 