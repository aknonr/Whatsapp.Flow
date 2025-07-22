using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Whatsapp.Flow.Services.Flow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlowsController : ControllerBase
    {
        private readonly IFlowRepository _flowRepository;
        private readonly IFlowTemplateRepository _flowTemplateRepository;

        public FlowsController(IFlowRepository flowRepository, IFlowTemplateRepository flowTemplateRepository)
        {
            _flowRepository = flowRepository;
            _flowTemplateRepository = flowTemplateRepository;
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

        [HttpPost("template/{templateId}/tenant/{tenantId}")]
        public async Task<ActionResult<Domain.Entities.Flow>> CreateFromTemplate(string templateId, string tenantId, [FromBody] string flowName)
        {
            var template = await _flowTemplateRepository.GetByIdAsync(templateId);
            if (template == null)
            {
                return NotFound("Template not found.");
            }

            var newFlow = new Domain.Entities.Flow
            {
                Name = flowName,
                TenantId = tenantId,
                Nodes = template.Nodes,
                IsActive = false, // Başlangıçta pasif olarak oluşturulur
                // Diğer template özelliklerini buraya kopyalayabilirsiniz
            };

            var createdFlow = await _flowRepository.AddNewAsync(newFlow);
            return CreatedAtAction(nameof(GetById), new { id = createdFlow.Id }, createdFlow);
        }

        [HttpPost("{flowId}/nodes")]
        public async Task<ActionResult<FlowNode>> AddNode(string flowId, [FromBody] FlowNode newNode)
        {
            var flow = await _flowRepository.GetByIdAsync(flowId);
            if (flow == null)
            {
                return NotFound("Flow not found.");
            }

            if (newNode == null || string.IsNullOrEmpty(newNode.NodeType))
            {
                return BadRequest("Invalid node data.");
            }

            // UI'dan gelen Id'yi kullanmak yerine yeni bir Id atayabiliriz.
            // Şimdilik UI'dan geleni kabul edelim.
            // newNode.Id = Guid.NewGuid().ToString();

            flow.Nodes.Add(newNode);
            await _flowRepository.UpdateAsync(flowId, flow);

            return CreatedAtAction(nameof(GetById), new { id = flowId }, newNode);
        }

        [HttpPut("{flowId}/nodes/{nodeId}")]
        public async Task<IActionResult> UpdateNode(string flowId, string nodeId, [FromBody] FlowNode updatedNode)
        {
            var flow = await _flowRepository.GetByIdAsync(flowId);
            if (flow == null)
            {
                return NotFound("Flow not found.");
            }

            var nodeToUpdate = flow.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (nodeToUpdate == null)
            {
                return NotFound("Node not found.");
            }

            // Gelen node ile eskisini değiştiriyoruz.
            // Daha gelişmiş bir senaryoda sadece değişen alanlar güncellenebilir.
            flow.Nodes.Remove(nodeToUpdate);
            updatedNode.Id = nodeId; // ID'nin değişmediğinden emin olalım.
            flow.Nodes.Add(updatedNode);

            await _flowRepository.UpdateAsync(flowId, flow);

            return NoContent();
        }

        [HttpDelete("{flowId}/nodes/{nodeId}")]
        public async Task<IActionResult> DeleteNode(string flowId, string nodeId)
        {
            var flow = await _flowRepository.GetByIdAsync(flowId);
            if (flow == null)
            {
                return NotFound("Flow not found.");
            }

            var nodeToRemove = flow.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (nodeToRemove == null)
            {
                return NotFound("Node not found.");
            }

            flow.Nodes.Remove(nodeToRemove);

            // Bu node'a gelen bağlantıları da temizlemek gerekir.
            flow.Nodes.ForEach(n => n.Outputs.RemoveAll(o => o.TargetNodeId == nodeId));

            await _flowRepository.UpdateAsync(flowId, flow);

            return NoContent();
        }
    }
} 