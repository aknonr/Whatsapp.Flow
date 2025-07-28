using System;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public class Subscription : BaseEntity
    {
        public string TenantId { get; set; }
        public SubscriptionPlan Plan { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        
        // Limitler
        public int MaxUsers { get; set; }
        public int MaxFlows { get; set; }
        public int MaxMessagesPerMonth { get; set; }
        public int MaxPhoneNumbers { get; set; }
        
        // Kullanım
        public int CurrentUsers { get; set; }
        public int CurrentFlows { get; set; }
        public int MessagesThisMonth { get; set; }
        public int CurrentPhoneNumbers { get; set; }
        
        // Faturalama
        public decimal MonthlyPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
    }
    
   
} 