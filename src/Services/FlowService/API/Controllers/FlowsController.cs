using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Flow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlowsController : ControllerBase
    {
        private readonly IFlowRepository _flowRepository;

        public FlowsController(IFlowRepository flowRepository)
        {
            _flowRepository = flowRepository;
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Flow>>> GetByTenant(string tenantId)
        {
            var flows = await _flowRepository.GetByTenantIdAsync(tenantId);
            return Ok(flows);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Entities.Flow>> GetById(string id)
        {
            var flow = await _flowRepository.GetByIdAsync(id);
            if (flow == null)
            {
                return NotFound();
            }
            return Ok(flow);
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Entities.Flow>> Create(Domain.Entities.Flow flow)
        {
            var createdFlow = await _flowRepository.AddNewAsync(flow);
            return CreatedAtAction(nameof(GetById), new { id = createdFlow.Id }, createdFlow);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Domain.Entities.Flow flowIn)
        {
            var flow = await _flowRepository.GetByIdAsync(id);

            if (flow == null)
            {
                return NotFound();
            }

            await _flowRepository.UpdateAsync(id, flowIn);

            return NoContent();
        }
    }
} 