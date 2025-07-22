using System.Linq;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Application.Interfaces;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;
using System;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Whatsapp.Webhook;
using System.Net.Http;

namespace Whatsapp.Flow.Services.Flow.Application.Services
{
    public class FlowEngine
    {
        private readonly IFlowRepository _flowRepository;
        private readonly IFlowStateRepository _flowStateRepository;
        private readonly IWhatsappService _whatsappService;
        private readonly IHttpClientFactory _httpClientFactory;

        public FlowEngine(
            IFlowRepository flowRepository,
            IFlowStateRepository flowStateRepository,
            IWhatsappService whatsappService,
            IHttpClientFactory httpClientFactory)
        {
            _flowRepository = flowRepository;
            _flowStateRepository = flowStateRepository;
            _whatsappService = whatsappService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task RunAsync(Tenant tenant, Message incomingMessage)
        {
            var userPhone = incomingMessage.From;
            var flowState = await _flowStateRepository.GetByUserPhoneAsync(userPhone);
            var activeFlow = await _flowRepository.GetActiveFlowByTenantIdAsync(tenant.Id);

            if (activeFlow == null) return;

            if (flowState == null || flowState.IsCompleted)
            {
                flowState = await StartNewFlowAsync(tenant, userPhone, activeFlow);
            }
            
            flowState.LastInteractionAt = DateTime.UtcNow;

            if (flowState.AwaitingAnswerState?.IsAwaiting ?? false)
            {
                await HandleAwaitingAnswerAsync(flowState, incomingMessage, activeFlow);
                return;
            }
            
            if (incomingMessage.Type == "interactive")
            {
                await HandleInteractiveReplyAsync(flowState, incomingMessage, activeFlow);
                return;
            }

            var currentNode = activeFlow.Nodes.FirstOrDefault(n => n.Id == flowState.CurrentNodeId);
            if (currentNode != null)
            {
                await ProcessNodeAsync(flowState, currentNode, incomingMessage, activeFlow);
            }
        }

        private async Task ProcessNodeAsync(FlowState currentState, FlowNode node, Message incomingMessage, Domain.Entities.Flow activeFlow)
        {
            string nextNodeId = null;

            switch (node)
            {
                case SendMessageNode smn:
                    await _whatsappService.SendMessageAsync(incomingMessage.From, smn.Content);
                    nextNodeId = node.Outputs.FirstOrDefault()?.TargetNodeId;
                    break;
                case AskQuestionNode aqn:
                    await _whatsappService.SendMessageAsync(incomingMessage.From, aqn.Content);
                    currentState.AwaitingAnswerState = new AwaitingAnswerState
                    {
                        IsAwaiting = true,
                        AwaitingNodeId = aqn.Id,
                        VariableToSave = aqn.VariableName
                    };
                    break;
                case DecisionNode dn:
                    var matchedCondition = dn.Conditions
                        .FirstOrDefault(c => c.ExpectedInput.Equals(incomingMessage.Text?.Body, StringComparison.OrdinalIgnoreCase));
                    nextNodeId = matchedCondition != null
                        ? node.Outputs.FirstOrDefault(o => o.SourceHandle == matchedCondition.TargetOutputHandle)?.TargetNodeId
                        : node.Outputs.FirstOrDefault()?.TargetNodeId;
                    break;
                case ButtonNode bn:
                     await _whatsappService.SendButtonMessageAsync(incomingMessage.From, bn);
                     // Interactive mesaj gönderildiği için akış burada durur, kullanıcı cevabı beklenir.
                    break;
                case ListMenuNode lmn:
                    await _whatsappService.SendListMessageAsync(incomingMessage.From, lmn);
                    // Interactive mesaj gönderildiği için akış burada durur, kullanıcı cevabı beklenir.
                    break;
                case WaitNode wn:
                    currentState.WaitingState = new WaitingState
                    {
                        IsWaiting = true,
                        WaitingNodeId = wn.Id,
                        WaitType = wn.WaitType,
                        WaitUntil = wn.WaitType == "delay" 
                            ? DateTime.UtcNow.AddSeconds(wn.DurationInSeconds) 
                            : wn.ScheduledTime
                    };
                    // Akış burada durur. Bir background job'un bu state'i periyodik olarak kontrol edip
                    // bekleme süresi dolduğunda akışı devam ettirmesi gerekir.
                    break;
                case WebhookNode whn:
                    await HandleWebhookNodeAsync(currentState, whn);
                    nextNodeId = node.Outputs.FirstOrDefault()?.TargetNodeId;
                    break;
            }

            if (nextNodeId != null)
            {
                var nextNode = activeFlow.Nodes.FirstOrDefault(n => n.Id == nextNodeId);
                if (nextNode != null)
                {
                    currentState.CurrentNodeId = nextNodeId;
                    await ProcessNodeAsync(currentState, nextNode, incomingMessage, activeFlow);
                }
                else
                {
                    await CompleteFlowAsync(currentState);
                }
            }
            else if(currentState.AwaitingAnswerState == null || !currentState.AwaitingAnswerState.IsAwaiting)
            {
                 await CompleteFlowAsync(currentState);
            }
            
            await _flowStateRepository.UpdateAsync(currentState);
        }

        private async Task HandleWebhookNodeAsync(FlowState currentState, WebhookNode webhookNode)
        {
            var client = _httpClientFactory.CreateClient();

            // Body template'deki değişkenleri doldurma
            var requestBody = webhookNode.BodyTemplate;
            foreach (var variable in currentState.Variables)
            {
                requestBody = requestBody.Replace($"{{{{{variable.Key}}}}}", variable.Value.ToString());
            }

            var request = new HttpRequestMessage(new HttpMethod(webhookNode.Method), webhookNode.Url)
            {
                Content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json")
            };

            foreach (var header in webhookNode.Headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            try
            {
                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(webhookNode.ResponseVariableName))
                {
                    currentState.Variables[webhookNode.ResponseVariableName] = responseContent;
                }

                // TODO: Yanıtı parse edip ResponseMapping'e göre değişkenlere atama
            }
            catch (Exception ex)
            {
                // TODO: Hata yönetimi - loglama ve belki akışta farklı bir yola sapma
                Console.WriteLine($"Webhook error: {ex.Message}");
            }
        }
        
        private async Task HandleInteractiveReplyAsync(FlowState currentState, Message incomingMessage, Domain.Entities.Flow activeFlow)
        {
            string selectedId = incomingMessage.Interactive?.ButtonReply?.Id ?? incomingMessage.Interactive?.ListReply?.Id;
            if (string.IsNullOrEmpty(selectedId)) return;
            
            // Mevcut node'u bulmamız lazım, ancak interaktif cevaplar state'i değiştirmediği için
            // CurrentNodeId'den bir önceki adıma bakmamız gerekebilir.
            // Şimdilik basitçe CurrentNodeId'yi kullanalım.
            var currentNode = activeFlow.Nodes.FirstOrDefault(n => n.Id == currentState.CurrentNodeId);
            string nextNodeId = null;

            if (currentNode is ButtonNode buttonNode)
            {
                var selectedButton = buttonNode.Buttons.FirstOrDefault(b => b.Id == selectedId);
                if (selectedButton != null)
                {
                    nextNodeId = buttonNode.Outputs.FirstOrDefault(o => o.SourceHandle == selectedButton.TargetOutputHandle)?.TargetNodeId;
                }
            }
            // ListMenuNode için de benzer bir mantık eklenebilir. Şimdilik butonlara odaklanalım.

            if (nextNodeId != null)
            {
                var nextNode = activeFlow.Nodes.FirstOrDefault(n => n.Id == nextNodeId);
                if (nextNode != null)
                {
                    currentState.CurrentNodeId = nextNodeId;
                    await ProcessNodeAsync(currentState, nextNode, incomingMessage, activeFlow);
                }
                else
                {
                    await CompleteFlowAsync(currentState);
                }
            }
            else
            {
                // Eşleşen bir buton/liste elemanı bulunamadıysa akışı sonlandırabilir veya bir hata mesajı gönderebiliriz.
                await CompleteFlowAsync(currentState);
            }
            await _flowStateRepository.UpdateAsync(currentState);
        }

        private async Task HandleAwaitingAnswerAsync(FlowState currentState, Message incomingMessage, Domain.Entities.Flow activeFlow)
        {
            var answer = incomingMessage.Text?.Body;
            if (!string.IsNullOrEmpty(answer))
            {
                currentState.Variables[currentState.AwaitingAnswerState.VariableToSave] = answer;
                var previousNodeId = currentState.AwaitingAnswerState.AwaitingNodeId;
                var previousNode = activeFlow.Nodes.FirstOrDefault(n => n.Id == previousNodeId);
                
                currentState.AwaitingAnswerState.IsAwaiting = false;
                currentState.AwaitingAnswerState.AwaitingNodeId = null;
                currentState.AwaitingAnswerState.VariableToSave = null;
                
                var nextNodeId = previousNode?.Outputs.FirstOrDefault()?.TargetNodeId;
                if (nextNodeId != null)
                {
                    var nextNode = activeFlow.Nodes.FirstOrDefault(n => n.Id == nextNodeId);
                    if (nextNode != null)
                    {
                        currentState.CurrentNodeId = nextNodeId;
                        await ProcessNodeAsync(currentState, nextNode, incomingMessage, activeFlow);
                    }
                    else
                    {
                        await CompleteFlowAsync(currentState);
                    }
                }
                else
                {
                    await CompleteFlowAsync(currentState);
                }
            }
            await _flowStateRepository.UpdateAsync(currentState);
        }


        private async Task<FlowState> StartNewFlowAsync(Tenant tenant, string userPhone, Domain.Entities.Flow flow)
        {
            var flowState = new FlowState
            {
                TenantId = tenant.Id,
                FlowId = flow.Id,
                UserPhoneNumber = userPhone,
                CurrentNodeId = flow.Nodes.FirstOrDefault()?.Id,
                IsCompleted = false,
                Variables = new Dictionary<string, object>()
            };
            await _flowStateRepository.CreateAsync(flowState);
return flowState;
        }

        private async Task CompleteFlowAsync(FlowState flowState)
        {
            flowState.IsCompleted = true;
            flowState.CompletedAt = DateTime.UtcNow;
            await _flowStateRepository.UpdateAsync(flowState);
        }
    }
} 