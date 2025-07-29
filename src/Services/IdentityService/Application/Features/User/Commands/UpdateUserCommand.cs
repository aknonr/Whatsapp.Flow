using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Commands
{
    public class UpdateUserCommand : IRequest
    {
        [JsonIgnore] 
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserStatus Status { get; set; }
        public List<string> TenantRoleIds { get; set; }
    }
} 