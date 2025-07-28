using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{

    public enum SubscriptionStatus
    {
        Active = 1,
        Trial = 2,
        Suspended = 3,
        Cancelled = 4,
        Expired = 5
    }
}
