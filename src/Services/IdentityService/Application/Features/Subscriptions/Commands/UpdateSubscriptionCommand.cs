using MediatR;
using System;
using System.Text.Json.Serialization;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Commands
{
    public class UpdateSubscriptionCommand : IRequest
    {
        [JsonIgnore]
        public string TenantId { get; set; }

        public SubscriptionPlan? Plan { get; set; }
        public SubscriptionStatus? Status { get; set; }
        public DateTime? EndDate { get; set; }
    }
} 