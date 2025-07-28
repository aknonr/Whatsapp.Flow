using System;
using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Queries
{
    public class TenantDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ActivatedAt { get; set; }
        
        // WhatsApp ve Abonelik bilgileri özet olarak eklenebilir.
        // Örneğin: public int ActivePhoneNumbers { get; set; }
        // Örneğin: public string SubscriptionPlan { get; set; }
    }
} 