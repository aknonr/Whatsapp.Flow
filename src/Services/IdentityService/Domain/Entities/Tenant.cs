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
        public bool IsDeleted { get; set; }


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
        public DateTime DeletedAt { get; set; }

        // Ayarlar
        public TenantSettings Settings { get; set; } = new TenantSettings();
    }
    
   
} 