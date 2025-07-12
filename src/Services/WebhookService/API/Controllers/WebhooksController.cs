using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using Whatsapp.Flow.BuildingBlocks.Common.Whatsapp.Webhook;
using Whatsapp.Flow.BuildingBlocks.EventBus;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.Services.WebhookService.Application.IntegrationEvents.Events;

namespace Whatsapp.Flow.Services.Webhook.API.Controllers
{
    [ApiController]
    [Route("/")] // Kök dizini dinliyoruz
    public class WebhooksController : ControllerBase
    {
        private readonly ILogger<WebhooksController> _logger;
        private readonly IEventBus _eventBus;
        private const string VerifyToken = "YOUR_SUPER_SECRET_VERIFY_TOKEN"; // Bu token'ı daha sonra appsettings'e taşıyacağız.

        public WebhooksController(ILogger<WebhooksController> logger, IEventBus eventBus)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new System.ArgumentNullException(nameof(eventBus));
        }

        [HttpGet]
        public IActionResult Verify([FromQuery(Name = "hub.mode")] string mode,
                                    [FromQuery(Name = "hub.challenge")] int challenge,
                                    [FromQuery(Name = "hub.verify_token")] string token)
        {
            _logger.LogInformation("Webhook doğrulama isteği alındı.");

            if (mode == "subscribe" && token == VerifyToken)
            {
                _logger.LogInformation("Webhook başarıyla doğrulandı.");
                return Ok(challenge);
            }
            else
            {
                _logger.LogWarning("Webhook doğrulama başarısız. Mode: {Mode}, Token: {Token}", mode, token);
                return Forbid();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Receive()
        {
            using var reader = new System.IO.StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            _logger.LogDebug("Gelen webhook payload'u: {Payload}", body);

            try
            {
                var payload = JsonSerializer.Deserialize<WebhookPayload>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (payload != null)
                {
                    _logger.LogInformation("----- Gelen Webhook bir mesaja dönüştürülüyor ve entegrasyon olayı olarak yayınlanıyor.");
                    var @event = new WhatsappMessageReceivedIntegrationEvent(payload);
                    _eventBus.Publish(@event);
                }


                if (payload?.Entry != null)
                {
                    foreach (var entry in payload.Entry)
                    {
                        foreach (var change in entry.Changes)
                        {
                            // Gelen mesajları işle (şimdilik sadece logla)
                            if (change.Value?.Messages != null)
                            {
                                foreach (var message in change.Value.Messages)
                                {
                                    _logger.LogInformation("{From} kullanıcısından '{Type}' tipinde yeni bir mesaj alındı.", 
                                        message.From, message.Type);

                                    // Burada mesajı bir sonraki servise (örn. RabbitMQ üzerinden) göndereceğiz.
                                }
                            }
                        }
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Webhook payload'u deserialize edilirken hata oluştu.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Webhook işlenirken beklenmedik bir hata oluştu.");
            }

            return Ok();
        }
    }
} 