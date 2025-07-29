# IdentityService - Teknik Dokümantasyon

## Proje Yapısı ve Bağımlılıklar

### 1. Whatsapp.Flow.Services.Identity.API
**Tip:** ASP.NET Core Web API  
**Framework:** .NET 8.0  
**Sorumluluk:** HTTP endpoint'leri, güvenlik, middleware

#### Bağımlılıklar:
```xml
- BCrypt.Net-Next (4.0.3) - Şifre hash'leme
- Microsoft.AspNetCore.Authentication.JwtBearer (8.0.6) - JWT authentication
- MongoDB.Driver (2.25.0) - MongoDB bağlantısı
- Swashbuckle.AspNetCore (8.1.4) - Swagger/OpenAPI dokümantasyonu
- System.IdentityModel.Tokens.Jwt (8.13.0) - JWT token işlemleri
```

#### Önemli Sınıflar:

##### Controllers
1. **TenantsController**
   - Tenant CRUD işlemleri
   - SuperAdmin yetkisi gerektirir
   - Soft delete desteği

2. **UsersController**
   - Kullanıcı yönetimi (Register, Login, CRUD)
   - Permission-based yetkilendirme
   - Rate limiting (login endpoint)

3. **TenantRolesController**
   - Tenant içi rol yönetimi
   - Granüler permission sistemi
   - Rol atama işlemleri

4. **SubscriptionController**
   - Abonelik bilgilerini yönetir
   - Tenant bazlı limit kontrolü

##### Security
1. **HasRoleAttribute**
   ```csharp
   // System-wide rol kontrolü için
   [HasRole(Role.SuperAdmin)]
   ```

2. **HasPermissionAttribute**
   ```csharp
   // Tenant-specific permission kontrolü için
   [HasPermission("users.read")]
   ```

3. **PermissionAuthorizationHandler**
   - Permission kontrolünü yapar
   - Kullanıcının tenant rollerini kontrol eder
   - Cache mekanizması eklenebilir

4. **PermissionPolicyProvider**
   - Dynamic policy oluşturur
   - Permission string'lerini policy'e çevirir

##### Middleware
1. **ExceptionHandlerMiddleware**
   - Global hata yönetimi
   - ValidationException → 400 Bad Request
   - NotFoundException → 404 Not Found
   - UnauthorizedException → 401 Unauthorized
   - Diğer hatalar → 500 Internal Server Error

### 2. Whatsapp.Flow.Services.Identity.Application
**Tip:** Class Library  
**Framework:** .NET 8.0  
**Sorumluluk:** İş mantığı, CQRS, validation

#### Bağımlılıklar:
```xml
- FluentValidation.AspNetCore (11.3.0) - Input validation
- MediatR (12.3.0) - CQRS pattern
- Microsoft.IdentityModel.Tokens (8.13.0) - Token işlemleri
```

#### Klasör Yapısı ve İçerik:

##### Features (CQRS Pattern)

###### Tenant
**Commands:**
- `CreateTenantCommand/Handler` - Yeni tenant oluşturur, trial subscription başlatır
- `UpdateTenantCommand/Handler` - Tenant bilgilerini günceller
- `DeleteTenantCommand/Handler` - Soft delete yapar, kim sildiğini kaydeder

**Queries:**
- `GetTenantByIdQuery/Handler` - ID ile tenant getirir
- `TenantDto` - Tenant veri transfer objesi

**Validators:**
- `CreateTenantCommandValidator` - Email, telefon formatı kontrolü
- `UpdateTenantCommandValidator` - Güncelleme verisi doğrulama

###### User
**Commands:**
- `RegisterUserCommand/Handler` - Yeni kullanıcı kaydı
- `LoginCommand/Handler` - JWT token üretir
- `UpdateUserCommand/Handler` - Kullanıcı bilgilerini günceller
- `DeleteUserCommand/Handler` - Kullanıcıyı inactive yapar

**Queries:**
- `GetUsersForTenantQuery/Handler` - Tenant kullanıcılarını listeler
- `GetUserByIdQuery/Handler` - Kullanıcı detayı getirir
- `UserDto` - Hassas bilgileri içermeyen kullanıcı DTO'su

**Validators:**
- `UpdateUserCommandValidator` - İsim, soyisim, status kontrolü

###### TenantRoles
**Commands:**
- `CreateTenantRoleCommand/Handler` - Yeni rol oluşturur
- `AssignRoleToUserCommand/Handler` - Kullanıcıya rol atar
- `UpdateTenantRoleCommand/Handler` - Rol günceller (sistem rolleri hariç)
- `DeleteTenantRoleCommand/Handler` - Rol siler

**Queries:**
- `GetRolesForTenantQuery/Handler` - Tenant rollerini listeler
- `TenantRoleDto` - Rol bilgilerini içerir

###### Subscription
**Queries:**
- `GetSubscriptionByTenantIdQuery/Handler` - Abonelik detayları
- `SubscriptionDto` - Plan, limit, kullanım bilgileri

##### Common
1. **Behaviors**
   - `ValidationBehavior` - MediatR pipeline'da validation yapar

2. **Exceptions**
   - `ValidationException` - Validation hataları için
   - `NotFoundException` - Kayıt bulunamadığında

##### Interfaces
- `ICacheService` - Cache operasyonları için
- `IPasswordService` - Şifre hash/verify işlemleri

##### IntegrationEvents
- `TenantInfoUpdatedIntegrationEvent` - Tenant güncellendiğinde yayınlanır

### 3. Whatsapp.Flow.Services.Identity.Domain
**Tip:** Class Library  
**Framework:** .NET 8.0  
**Sorumluluk:** Domain modelleri ve repository interface'leri

#### Entities

1. **User**
   ```csharp
   - Email, PasswordHash
   - FirstName, LastName, PhoneNumber
   - SystemRole (User/SuperAdmin)
   - TenantRoleIds (List<string>)
   - Status (Active/Inactive/Suspended)
   - Security fields (EmailConfirmed, TwoFactorEnabled, etc.)
   - Audit fields (CreatedAt, LastLoginAt)
   ```

2. **Tenant**
   ```csharp
   - Name, CompanyName
   - ContactEmail, ContactPhone
   - WhatsApp Business Account bilgileri
   - Status (Active/Suspended/Cancelled)
   - Soft delete fields (IsDeleted, DeletedAt, DeletedBy)
   - Settings (TenantSettings)
   ```

3. **TenantRole**
   ```csharp
   - RoleName, Description
   - Permissions (List<Permission>)
   - IsSystemRole
   - Audit fields (CreatedAt, CreatedBy)
   ```

4. **Subscription**
   ```csharp
   - Plan (Trial/Basic/Professional/Enterprise)
   - Status (Active/Expired/Cancelled)
   - Limits (MaxUsers, MaxFlows, MaxMessagesPerMonth)
   - CurrentUsage
   - Billing information
   ```

#### Repositories (Interfaces)
- `IUserRepository` - User CRUD + GetByEmail, GetByTenantId
- `ITenantRepository` - Tenant CRUD + SoftDelete
- `ITenantRoleRepository` - Role CRUD + GetUserPermissions
- `ISubscriptionRepository` - Subscription CRUD + CheckLimit, IncrementUsage

### 4. Whatsapp.Flow.Services.Identity.Infrastructure
**Tip:** Class Library  
**Framework:** .NET 8.0  
**Sorumluluk:** External service implementasyonları

#### Bağımlılıklar:
```xml
- BCrypt.Net-Next (4.0.3) - PasswordService için
- MongoDB.Driver (2.25.0) - Repository implementasyonları
- StackExchange.Redis (2.7.33) - Cache service
```

#### Repositories (MongoDB Implementations)

1. **MongoUserRepository**
   - MongoDB `Users` collection
   - Email unique index gerekli
   - TenantId index önerilir

2. **MongoTenantRepository**
   - MongoDB `Tenants` collection
   - Soft delete filtreleme
   - IsDeleted index önerilir

3. **MongoTenantRoleRepository**
   - MongoDB `TenantRoles` collection
   - GetUserPermissionsAsync - Kullanıcının tüm permission'larını toplar

4. **MongoSubscriptionRepository**
   - MongoDB `Subscriptions` collection
   - Limit kontrol ve usage artırma

#### Services

1. **PasswordService**
   ```csharp
   - HashPassword(string password) - BCrypt ile hash'ler
   - VerifyPassword(string password, string hash) - Hash doğrulama
   ```

2. **RedisCacheService**
   ```csharp
   - GetAsync<T>(string key)
   - SetAsync<T>(string key, T value, TimeSpan? expiry)
   - RemoveAsync(string key)
   - RemoveByPatternAsync(string pattern)
   ```

## Veri Akışı

### 1. Authentication Flow
```
1. Client → POST /api/users/login
2. LoginCommand → LoginCommandHandler
3. UserRepository.GetByEmailAsync()
4. PasswordService.VerifyPassword()
5. JWT Token üretimi (claims: userId, email, tenantId, role)
6. Token → Client
```

### 2. Authorization Flow
```
1. Client → Request with JWT Bearer token
2. JWT Middleware → Token validation
3. [HasPermission("users.read")] → PermissionAuthorizationHandler
4. TenantRoleRepository.GetUserPermissionsAsync()
5. Permission check → Allow/Deny
```

### 3. CQRS Flow
```
1. Controller → MediatR.Send(Command/Query)
2. ValidationBehavior → FluentValidation
3. Handler → Business logic
4. Repository → Database operation
5. Response → DTO mapping → Client
```

## Best Practices Uygulanan Alanlar

### 1. SOLID Principles
- **S**ingle Responsibility: Her sınıf tek bir sorumluluğa sahip
- **O**pen/Closed: Extension için açık, modification için kapalı
- **L**iskov Substitution: Interface'ler doğru şekilde implement edilmiş
- **I**nterface Segregation: Küçük, özel interface'ler
- **D**ependency Inversion: Repository pattern ile soyutlama

### 2. Clean Architecture
- Domain katmanı hiçbir şeye bağımlı değil
- Application katmanı sadece Domain'e bağımlı
- Infrastructure dış sistemleri soyutluyor
- API katmanı sadece Application'ı kullanıyor

### 3. Security Best Practices
- Şifreler hash'leniyor, plain text saklanmıyor
- Sensitive data DTO'larda expose edilmiyor
- Multi-tenant data isolation
- Rate limiting brute force koruması

### 4. Performance Considerations
- Redis cache kullanımı
- Async/await pattern her yerde
- Database index önerileri
- Event-driven mimari ile loose coupling

## Eksik veya İyileştirilebilecek Alanlar

1. **Unit Test Coverage** - Test projesi görünmüyor
2. **Integration Tests** - API endpoint testleri
3. **Logging** - Structured logging (Serilog önerilir)
4. **API Documentation** - XML comments tamamlanmalı
5. **Configuration Validation** - Options pattern validation
6. **Database Migrations** - MongoDB migration stratejisi
7. **Background Jobs** - Hangfire veya hosted services
8. **Monitoring** - Application Insights veya Prometheus

## Sonuç

IdentityService, modern .NET 8 best practice'lerini takip eden, güvenli ve ölçeklenebilir bir kimlik doğrulama servisidir. Clean Architecture prensiplerine uygun olarak tasarlanmış olup, maintenance ve extension açısından oldukça esnektir. 