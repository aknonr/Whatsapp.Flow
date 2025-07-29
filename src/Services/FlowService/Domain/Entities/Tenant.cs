using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    /// <summary>
    /// Represents a Tenant within the context of the FlowService.
    /// This is a simplified version of the Tenant from IdentityService,
    /// containing only the data needed for flow management.
    /// </summary>
    public class Tenant : BaseEntity
    {
        public string Name { get; private set; }
        public List<string> PhoneNumbers { get; private set; } = new List<string>();

        // Private constructor for persistence frameworks
        private Tenant() { }

        public Tenant(string id, string name, List<string> phoneNumbers)
        {
            Id = id;
            Name = name;
            PhoneNumbers = phoneNumbers ?? new List<string>();
        }

        public void UpdateInfo(string name, List<string> phoneNumbers)
        {
            Name = name;
            PhoneNumbers = phoneNumbers ?? new List<string>();
        }
    }
} 