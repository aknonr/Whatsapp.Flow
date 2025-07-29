# IdentityService - Güvenlik ve Mimari Dokümantasyonu

## İçindekiler
1. [Genel Bakış](#genel-bakış)
2. [Mimari Yapı](#mimari-yapı)
3. [Güvenlik Analizi](#güvenlik-analizi)
4. [API Endpoints](#api-endpoints)
5. [Yetkilendirme Sistemi](#yetkilendirme-sistemi)
6. [Veri Güvenliği](#veri-güvenliği)
7. [Performans ve Ölçeklenebilirlik](#performans-ve-ölçeklenebilirlik)
8. [Geliştirme Önerileri](#geliştirme-önerileri)

## Genel Bakış

IdentityService, WhatsApp Flow platformunun kimlik doğrulama ve yetkilendirme servisini sağlar. Multi-tenant mimari ile tasarlanmış olup, JWT tabanlı authentication ve granüler permission sistemi kullanır.

### Temel Özellikler
- 🔐 JWT tabanlı kimlik doğrulama
- 👥 Multi-tenant desteği
- 🛡️ Role ve Permission tabanlı yetkilendirme
- 📊 Subscription yönetimi
- 🚦 Rate limiting
- 💾 Redis cache desteği
- 🔄 Event-driven mimari (RabbitMQ)

## Mimari Yapı

### Katman Yapısı

```
IdentityService/
├── API/                    # Presentation Layer
│   ├── Controllers/        # REST API endpoints
│   ├── Middleware/         # Custom middleware (Exception handling)
│   └── Security/          # Authorization attributes ve handlers
├── Application/           # Business Logic Layer
│   ├── Features/         # CQRS Commands ve Queries
│   ├── Behaviors/        # MediatR pipeline behaviors
│   ├── Interfaces/       # Service interfaces
│   └── Exceptions/       # Custom exceptions
├── Domain/               # Domain Layer
│   ├── Entities/        # Domain models
│   └── Repositories/    # Repository interfaces
└── Infrastructure/      # Infrastructure Layer
    ├── Repositories/    # MongoDB implementations
    └── Services/        # External service implementations
```

### Teknoloji Stack
- **.NET 8.0** - Framework
- **MongoDB** - Primary database
- **Redis** - Caching layer
- **RabbitMQ** - Message broker
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **JWT** - Authentication tokens

## Güvenlik Analizi

### 1. Authentication & Authorization

#### JWT Configuration
```csharp
// Token ayarları appsettings.json'da saklanıyor
"JwtSettings": {
    "Key": "your-256-bit-secret-key-here",
    "Issuer": "WhatsApp.Flow.Identity",
    "Audience": "WhatsApp.Flow.Services",
    "DurationInMinutes": 60
}
```

#### İki Seviyeli Yetkilendirme
1. **System-wide Roles**: `SuperAdmin`, `User`
2. **Tenant-specific Permissions**: `users.read`, `users.create`, `roles.assign` vb.

### 2. Güvenlik Kontrolleri

| Controller | Authentication | Authorization | Rate Limiting | Input Validation |
|------------|----------------|---------------|---------------|------------------|
| Subscription | ✅ | ✅ SuperAdmin | ⚠️ Eksik | ✅ |
| TenantRoles | ✅ | ✅ Permissions | ⚠️ Eksik | ✅ |
| Tenants | ✅ | ✅ SuperAdmin | ⚠️ Eksik | ✅ |
| Users | ✅* | ✅ Permissions | ✅ Login only | ✅ |

*Login ve Register endpoint'leri hariç

### 3. Güvenlik Açıkları ve Önlemler

#### ✅ Korunan Alanlar:
- **SQL Injection**: MongoDB kullanımı sayesinde SQL injection riski yok
- **XSS**: API JSON response döndüğü için XSS riski minimal
- **CSRF**: Token-based authentication kullanıldığı için CSRF koruması sağlanmış
- **Password Security**: BCrypt ile hash'leme
- **Sensitive Data**: PasswordHash gibi hassas veriler DTO'larda expose edilmiyor

#### ⚠️ İyileştirme Gereken Alanlar:
1. **Rate Limiting**: Sadece login endpoint'inde var, diğer endpoint'lere eklenmeli
2. **CORS Policy**: Program.cs'de CORS yapılandırması eksik
3. **API Versioning**: Versiyon yönetimi yapılmamış
4. **Audit Logging**: Kritik işlemler için audit log eksik

## API Endpoints

### 1. Tenants Controller
| Method | Endpoint | Yetki | Açıklama |
|--------|----------|-------|----------|
| POST | /api/tenants | SuperAdmin | Yeni tenant oluşturur |
| GET | /api/tenants/{id} | SuperAdmin | Tenant detaylarını getirir |
| PUT | /api/tenants/{id} | SuperAdmin | Tenant bilgilerini günceller |
| DELETE | /api/tenants/{id} | SuperAdmin | Tenant'ı soft delete yapar |

### 2. Users Controller
| Method | Endpoint | Yetki | Açıklama |
|--------|----------|-------|----------|
| POST | /api/users/register | Public | Yeni kullanıcı kaydı |
| POST | /api/users/login | Public (Rate Limited) | Kullanıcı girişi |
| GET | /api/users | users.read | Tenant kullanıcılarını listeler |
| GET | /api/users/{id} | users.read | Kullanıcı detayı |
| PUT | /api/users/{id} | users.update | Kullanıcı güncelleme |
| DELETE | /api/users/{id} | users.delete | Kullanıcı soft delete |

### 3. TenantRoles Controller
| Method | Endpoint | Yetki | Açıklama |
|--------|----------|-------|----------|
| GET | /api/tenant-roles | roles.read | Tenant rollerini listeler |
| POST | /api/tenant-roles | roles.create | Yeni rol oluşturur |
| POST | /api/tenant-roles/assign | roles.assign | Kullanıcıya rol atar |
| PUT | /api/tenant-roles/{id} | roles.update | Rol günceller |
| DELETE | /api/tenant-roles/{id} | roles.delete | Rol siler |

### 4. Subscription Controller
| Method | Endpoint | Yetki | Açıklama |
|--------|----------|-------|----------|
| GET | /api/subscription/tenant/{id} | SuperAdmin | Tenant abonelik bilgisi |
| GET | /api/subscription/my | Authenticated | Kendi abonelik bilgisi |

## Yetkilendirme Sistemi

### Role-Based Authorization
```csharp
[HasRole(Role.SuperAdmin)]  // System-wide rol kontrolü
```

### Permission-Based Authorization
```csharp
[HasPermission("users.read")]  // Tenant-specific izin kontrolü
```

### Permission Akışı
1. JWT token'dan `userId` ve `tenantId` alınır
2. Kullanıcının tenant rolleri kontrol edilir
3. Rollerdeki permission'lar birleştirilir
4. İstenen permission var mı kontrol edilir

## Veri Güvenliği

### 1. Şifreleme
- Parolalar BCrypt ile hash'leniyor
- JWT token'lar HMAC-SHA256 ile imzalanıyor
- HTTPS zorunlu (Production'da)

### 2. Veri İzolasyonu
- Multi-tenant yapıda her tenant'ın verisi izole
- Repository seviyesinde tenant filtreleme
- Cross-tenant erişim engelleniyor

### 3. Soft Delete
- Kritik veriler (Tenant, User) fiziksel silinmiyor
- `IsDeleted`, `DeletedAt`, `DeletedBy` alanları ile audit

## Performans ve Ölçeklenebilirlik

### 1. Caching Stratejisi
- Redis ile JWT token cache
- User permission cache (önerilir)
- Tenant ayarları cache (önerilir)

### 2. Database İndeksler
Önerilen MongoDB indeksler:
```javascript
// Users collection
db.Users.createIndex({ "Email": 1 }, { unique: true })
db.Users.createIndex({ "TenantId": 1 })

// Tenants collection
db.Tenants.createIndex({ "IsDeleted": 1 })

// TenantRoles collection
db.TenantRoles.createIndex({ "TenantId": 1 })
```

### 3. Event-Driven Mimari
- RabbitMQ ile asenkron iletişim
- TenantInfoUpdatedIntegrationEvent yayınlanıyor
- Retry mekanizması mevcut

## Geliştirme Önerileri

### 1. Güvenlik İyileştirmeleri
```csharp
// Program.cs'e eklenecek CORS yapılandırması
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Tüm controller'lara rate limiting ekle
app.MapControllers()
   .RequireRateLimiting("fixed")
   .RequireCors("ApiPolicy");
```

### 2. Audit Logging
```csharp
// AuditLog entity ekle
public class AuditLog
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string TenantId { get; set; }
    public string Action { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public Dictionary<string, object> Changes { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 3. API Versioning
```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
```

### 4. Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddMongoDb(mongoConnectionString, name: "mongodb")
    .AddRedis(redisConnectionString, name: "redis")
    .AddRabbitMQ(rabbitConnectionString, name: "rabbitmq");
```

### 5. Request/Response Logging Middleware
```csharp
public class RequestResponseLoggingMiddleware
{
    // Log request/response details
    // Exclude sensitive data (passwords, tokens)
    // Include correlation ID for tracing
}
```

## Sonuç

IdentityService, güvenli ve ölçeklenebilir bir kimlik doğrulama servisi olarak tasarlanmıştır. SOLID prensipleri ve Clean Architecture yaklaşımı ile geliştirilmiştir. Yukarıdaki öneriler uygulandığında, production-ready bir servis haline gelecektir.

### Kritik Güvenlik Kontrol Listesi
- [ ] CORS politikası ekle
- [ ] Tüm endpoint'lere rate limiting ekle
- [ ] API versioning implementasyonu
- [ ] Audit logging sistemi
- [ ] Health check endpoint'leri
- [ ] Request/Response logging
- [ ] Penetration testing
- [ ] Security headers (HSTS, X-Frame-Options, etc.) 