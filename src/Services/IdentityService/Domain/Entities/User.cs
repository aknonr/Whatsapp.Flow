using Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = string.Empty;

        public string TenantId { get; set; } // Bu kullanıcının hangi Tenant'a ait olduğunu belirtir.

        // Kullanıcıya özgü diğer özellikler eklenebilir.
        // Örneğin: Ad, Soyad, Rol vb.
    }
} 