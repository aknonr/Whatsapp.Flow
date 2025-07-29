using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    
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