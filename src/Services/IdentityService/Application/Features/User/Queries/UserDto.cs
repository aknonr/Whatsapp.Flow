using System;
using System.Collections.Generic;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Queries
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserStatus Status { get; set; }
        public Role SystemRole { get; set; }
        public List<string> TenantRoleIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
} 