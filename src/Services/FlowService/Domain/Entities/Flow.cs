using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class Flow : BaseEntity
    {
        public string Name { get; set; }
        public string TenantId { get; set; }
        public bool IsActive { get; set; }
        public List<FlowNode> Nodes { get; set; } = new List<FlowNode>();
    }
} 