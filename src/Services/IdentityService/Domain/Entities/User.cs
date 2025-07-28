using System;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string TenantId { get; set; }
        
        // Kullanıcı Bilgileri
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
        
        // Roller
        public Role SystemRole { get; set; } = Role.User; // Sistem geneli rol
        public List<string> TenantRoleIds { get; set; } = new List<string>(); // Tenant içi roller
        
        // Durum ve Güvenlik
        public UserStatus Status { get; set; } = UserStatus.Active;
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? LockoutEnd { get; set; }
        
        // Tarihler
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        
        // Ayarlar
        public UserPreferences Preferences { get; set; } = new UserPreferences();
    }
    
    public class UserPreferences
    {
        public string Language { get; set; } = "tr-TR";
        public string TimeZone { get; set; } = "UTC";
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public Dictionary<string, object> CustomPreferences { get; set; } = new Dictionary<string, object>();
    }
    
    public enum UserStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        PendingApproval = 4
    }
} 