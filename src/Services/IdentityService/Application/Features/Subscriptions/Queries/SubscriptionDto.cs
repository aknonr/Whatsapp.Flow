using System;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Queries
{
    public class SubscriptionDto
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Plan { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }

        // Limits
        public int MaxUsers { get; set; }
        public int MaxFlows { get; set; }
        public int MaxMessagesPerMonth { get; set; }
        public int MaxPhoneNumbers { get; set; }

        // Usage
        public int CurrentUsers { get; set; }
        public int CurrentFlows { get; set; }
        public int MessagesThisMonth { get; set; }
        public int CurrentPhoneNumbers { get; set; }

        // Billing
        public decimal MonthlyPrice { get; set; }
        public string Currency { get; set; }
        public DateTime? NextPaymentDate { get; set; }
    }
} 