using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public class TenantSettings
    {
        public bool EnableAutoReply { get; set; } = true;
        public int MessageRetentionDays { get; set; } = 30;
        public bool EnableWebhookLogging { get; set; } = true;
        public string WebhookUrl { get; set; }
        public Dictionary<string, object> CustomSettings { get; set; } = new Dictionary<string, object>();
    }
}
