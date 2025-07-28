using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public class WhatsAppPhoneNumber
    {
        public string PhoneNumberId { get; set; }
        public string DisplayPhoneNumber { get; set; }
        public string VerifiedName { get; set; }
        public string QualityRating { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
