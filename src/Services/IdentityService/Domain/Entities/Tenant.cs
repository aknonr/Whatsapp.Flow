using System;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string TimeZone { get; set; } = "UTC";
        public string Language { get; set; } = "tr-TR";
        
        // WhatsApp Business Account Bilgileri
        public string WhatsAppBusinessAccountId { get; set; }
        public string MetaAppId { get; set; }
        public string SystemUserAccessToken { get; set; } // Şifrelenmiş saklanacak
        public List<WhatsAppPhoneNumber> PhoneNumbers { get; set; } = new List<WhatsAppPhoneNumber>();
        
        // Durum ve Tarihler
        public TenantStatus Status { get; set; } = TenantStatus.PendingSetup;
        public DateTime CreatedAt { get; set; }
        public DateTime? ActivatedAt { get; set; }
        public DateTime? SuspendedAt { get; set; }
        
        // Soft Delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        // Ayarlar
        public TenantSettings Settings { get; set; } = new TenantSettings();
    }
    
    public class WhatsAppPhoneNumber
    {
        public string PhoneNumberId { get; set; }
        public string DisplayPhoneNumber { get; set; }
        public string VerifiedName { get; set; }
        public string QualityRating { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedAt { get; set; }
    }
    
    public class TenantSettings
    {
        public bool EnableAutoReply { get; set; } = true;
        public int MessageRetentionDays { get; set; } = 30;
        public bool EnableWebhookLogging { get; set; } = true;
        public string WebhookUrl { get; set; }
        public Dictionary<string, object> CustomSettings { get; set; } = new Dictionary<string, object>();
    }
    
    public enum TenantStatus
    {
        PendingSetup = 1,
        Active = 2,
        Suspended = 3,
        Inactive = 4
    }
} 