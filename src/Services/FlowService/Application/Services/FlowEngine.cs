using System.Linq;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Application.Interfaces;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;

namespace Whatsapp.Flow.Services.Flow.Application.Services
{
    public class FlowEngine
    {
        private readonly IFlowRepository _flowRepository;
        private readonly IFlowStateRepository _flowStateRepository;
        private readonly IWhatsappService _whatsappService;

        public FlowEngine(
            IFlowRepository flowRepository,
            IFlowStateRepository flowStateRepository,
            IWhatsappService whatsappService)
        {
            _flowRepository = flowRepository;
            _flowStateRepository = flowStateRepository;
            _whatsappService = whatsappService;
        }

        public async Task RunAsync(Tenant tenant, string messageFrom, string incomingMessageContent)
        {
            var flow = await _flowRepository.GetActiveFlowByTenantIdAsync(tenant.Id);
            if (flow == null)
            {
                return; // Opsiyonel: "Aktif bir akış bulunamadı" mesajı gönderilebilir.
            }

            var flowState = await _flowStateRepository.GetByUserPhoneAsync(messageFrom);
            FlowNode currentNode;

            if (flowState != null && !flowState.IsCompleted)
            {
                currentNode = flow.Nodes.FirstOrDefault(n => n.Id == flowState.CurrentNodeId);
                if (currentNode == null) // State'de kalan node ID artık akışta yoksa başa dön.
                {
                    currentNode = flow.Nodes.FirstOrDefault();
                    if (currentNode != null)
                    {
                        flowState.CurrentNodeId = currentNode.Id;
                    }
                }
            }
            else // Kullanıcı akışa ilk kez başlıyor veya tamamlamış, yeniden başlıyor.
            {
                currentNode = flow.Nodes.FirstOrDefault();
                if (currentNode != null)
                {
                    if (flowState != null) // Tamamlanmış bir state'i olan kullanıcı için state'i güncelle
                    {
                        flowState.CurrentNodeId = currentNode.Id;
                        flowState.IsCompleted = false;
                        await _flowStateRepository.UpdateAsync(flowState);
                    }
                    else // Yeni kullanıcı için yeni state oluştur
                    {
                        flowState = new FlowState
                        {
                            TenantId = tenant.Id,
                            FlowId = flow.Id,
                            UserPhoneNumber = messageFrom,
                            CurrentNodeId = currentNode.Id,
                            IsCompleted = false
                        };
                        await _flowStateRepository.CreateAsync(flowState);
                    }
                }
            }

            if (currentNode == null)
            {
                return; // Akışta hiç node yok.
            }

            await ProcessNodeAsync(flowState, currentNode, messageFrom, incomingMessageContent);
        }

        private async Task ProcessNodeAsync(FlowState currentState, FlowNode node, string messageFrom, string incomingMessageContent)
        {
            string nextNodeId = null;

            if (node is SendMessageNode sendMessageNode)
            {
                await _whatsappService.SendMessageAsync(messageFrom, sendMessageNode.Content);
                // Mesaj gönderiminden sonra genellikle tek bir çıkış olur.
                nextNodeId = node.Outputs.FirstOrDefault()?.TargetNodeId;
            }
            else if (node is DecisionNode decisionNode)
            {
                var matchedCondition = decisionNode.Conditions
                    .FirstOrDefault(c => c.ExpectedInput.Equals(incomingMessageContent, System.StringComparison.OrdinalIgnoreCase));

                if (matchedCondition != null)
                {
                    nextNodeId = node.Outputs
                        .FirstOrDefault(o => o.SourceHandle == matchedCondition.TargetOutputHandle)?.TargetNodeId;
                }
                else
                {
                    // Eşleşme bulunamazsa "default" veya ilk çıkışı kullanabiliriz. Şimdilik ilkini alıyoruz.
                    nextNodeId = node.Outputs.FirstOrDefault()?.TargetNodeId;
                }
            }

            if (nextNodeId != null)
            {
                currentState.CurrentNodeId = nextNodeId;
                await _flowStateRepository.UpdateAsync(currentState);
            }
            else
            {
                currentState.IsCompleted = true;
                await _flowStateRepository.UpdateAsync(currentState);
            }
        }
    }
} 