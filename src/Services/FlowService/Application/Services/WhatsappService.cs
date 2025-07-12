using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Application.Interfaces;

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
    }
} 