using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class Flow : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TenantId { get; set; }
        public bool IsActive { get; set; }
        public FlowType Type { get; set; } = FlowType.Custom;
        public string Category { get; set; } // Örn: "Sales", "Support", "Marketing"
        public List<FlowNode> Nodes { get; set; } = new List<FlowNode>();
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
        public FlowSettings Settings { get; set; } = new FlowSettings();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }

    public enum FlowType
    {
        Custom,
        Template,
        System
    }

    public class FlowSettings
    {
        public string StartTrigger { get; set; } = "message"; // "message", "keyword", "schedule"
        public List<string> TriggerKeywords { get; set; } = new List<string>();
        public string Schedule { get; set; } // Cron expression
        public int MaxExecutionTime { get; set; } = 3600; // saniye
        public bool AllowMultipleInstances { get; set; } = false;
        public string DefaultLanguage { get; set; } = "tr";
    }
} 