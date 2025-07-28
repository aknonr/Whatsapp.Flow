using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public enum TenantStatus
    {
        PendingSetup = 1,
        Active = 2,
        Suspended = 3,
        Inactive = 4
    }
}
