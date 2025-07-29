using System;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Queries
{
    public class SubscriptionDto
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public SubscriptionPlan Plan { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public SubscriptionLimitsDto Limits { get; set; }
        public SubscriptionUsageDto CurrentUsage { get; set; }
    }

    public class SubscriptionLimitsDto
    {
        public int MaxUsers { get; set; }
        public int MaxFlows { get; set; }
        public int MaxMessagesPerMonth { get; set; }
        public int MaxPhoneNumbers { get; set; }
    }

    public class SubscriptionUsageDto
    {
        public int Users { get; set; }
        public int Flows { get; set; }
        public int MessagesSentThisMonth { get; set; }
        public int PhoneNumbers { get; set; }
    }
} 