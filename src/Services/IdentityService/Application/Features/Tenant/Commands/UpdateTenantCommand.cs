using MediatR;
using System.Text.Json.Serialization;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public class UpdateTenantCommand : IRequest
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
    }
} 