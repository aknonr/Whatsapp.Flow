using System;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public class TenantRole : BaseEntity
    {
        public string TenantId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public List<Permission> Permissions { get; set; } = new List<Permission>();
        public bool IsSystemRole { get; set; } // Sistem tarafından oluşturulan roller silinemez
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
    
    public class Permission
    {
        public string Resource { get; set; } // Örn: "flows", "users", "webhooks"
        public List<string> Actions { get; set; } = new List<string>(); // Örn: ["create", "read", "update", "delete"]
    }
} 