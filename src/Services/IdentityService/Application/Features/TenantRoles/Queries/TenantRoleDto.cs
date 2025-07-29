using System;
using System.Collections.Generic;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Queries
{
    public class TenantRoleDto
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public List<Permission> Permissions { get; set; }
        public bool IsSystemRole { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
} 