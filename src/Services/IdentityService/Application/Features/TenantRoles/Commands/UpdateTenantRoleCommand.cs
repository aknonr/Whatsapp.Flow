using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public class UpdateTenantRoleCommand : IRequest
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string Description { get; set; }
        public List<Permission> Permissions { get; set; }
    }
} 