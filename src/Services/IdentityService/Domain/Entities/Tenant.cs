using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Burada Tenant'a özgü diğer özellikler ve iş kuralları yer alabilir.
        // Örneğin: Adres, Vergi Numarası, Aktiflik Durumu vb.
    }
} 