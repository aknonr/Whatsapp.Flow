using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class FlowTemplate : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string PreviewImageUrl { get; set; }
        public List<FlowNode> Nodes { get; set; } = new List<FlowNode>();
        public Dictionary<string, TemplateVariable> TemplateVariables { get; set; } = new Dictionary<string, TemplateVariable>();
        public bool IsPublic { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UsageCount { get; set; } = 0;
        public double Rating { get; set; } = 0;
    }

    public class TemplateVariable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "string", "number", "boolean", "list"
        public object DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public List<string> AllowedValues { get; set; } // Eğer belirli değerler varsa
    }
} 