using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Whatsapp.Flow.Services.Flow.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Application.Services
{
    public class WhatsappService : IWhatsappService
    {
        private readonly HttpClient _httpClient;
        private readonly string _phoneNumberId;
        private readonly string _accessToken;

        public WhatsappService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _phoneNumberId = configuration["WhatsApp:PhoneNumberId"];
            _accessToken = configuration["WhatsApp:AccessToken"];

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
        }

        public async Task SendMessageAsync(string toPhoneNumber, string messageContent)
        {
            var requestBody = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = toPhoneNumber,
                type = "text",
                text = new { body = messageContent }
            };

            var response = await _httpClient.PostAsJsonAsync($"https://graph.facebook.com/v20.0/{_phoneNumberId}/messages", requestBody);

            response.EnsureSuccessStatusCode();
        }

        public async Task SendButtonMessageAsync(string toPhoneNumber, ButtonNode buttonNode)
        {
            var requestBody = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = toPhoneNumber,
                type = "interactive",
                interactive = new
                {
                    type = "button",
                    header = new { type = "text", text = buttonNode.HeaderText },
                    body = new { text = buttonNode.BodyText },
                    footer = new { text = buttonNode.FooterText },
                    action = new
                    {
                        buttons = buttonNode.Buttons.Select(b => new { type = "reply", reply = new { id = b.Id, title = b.Title } }).ToList()
                    }
                }
            };
            var response = await _httpClient.PostAsJsonAsync($"https://graph.facebook.com/v20.0/{_phoneNumberId}/messages", requestBody);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendListMessageAsync(string toPhoneNumber, ListMenuNode listMenuNode)
        {
            var requestBody = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = toPhoneNumber,
                type = "interactive",
                interactive = new
                {
                    type = "list",
                    header = new { type = "text", text = listMenuNode.HeaderText },
                    body = new { text = listMenuNode.BodyText },
                    footer = new { text = listMenuNode.FooterText },
                    action = new
                    {
                        button = listMenuNode.ButtonText,
                        sections = listMenuNode.Sections.Select(s => new
                        {
                            title = s.Title,
                            rows = s.Rows.Select(r => new { id = r.Id, title = r.Title, description = r.Description }).ToList()
                        }).ToList()
                    }
                }
            };
            var response = await _httpClient.PostAsJsonAsync($"https://graph.facebook.com/v20.0/{_phoneNumberId}/messages", requestBody);
            response.EnsureSuccessStatusCode();
        }
    }
} 