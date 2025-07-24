# 🐰 RabbitMQ Kurulum ve Kullanım Kılavuzu

## 📋 Gereksinimler

- Docker Desktop
- .NET 8.0 SDK
- PowerShell (Windows)

## 🚀 Hızlı Başlangıç

### 1. Docker Compose ile Servisleri Başlat

```powershell
# Test scriptini çalıştır
.\test-rabbitmq.ps1
```

### 2. Manuel Başlatma

```powershell
# Docker Compose ile başlat
docker-compose up -d

# Servisleri kontrol et
docker-compose ps
```

## 🔧 Konfigürasyon

### RabbitMQ Ayarları

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| Host | localhost | RabbitMQ sunucu adresi |
| Port | 5672 | AMQP port |
| Management UI | http://localhost:15672 | Web arayüzü |
| Kullanıcı | admin | Yönetici kullanıcı |
| Şifre | admin123 | Yönetici şifresi |

### MongoDB Ayarları

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| Host | localhost | MongoDB sunucu adresi |
| Port | 27017 | MongoDB port |
| Database | WhatsappFlowDb | Veritabanı adı |
| Kullanıcı | root | Root kullanıcı |
| Şifre | example | Root şifresi |

## 🧪 Test Etme

### RabbitMQ Management UI

1. Tarayıcıda `http://localhost:15672` adresine git
2. Kullanıcı: `admin`, Şifre: `admin123`
3. Queues, Exchanges ve Connections'ları kontrol et

### MongoDB Bağlantısı

```powershell
# MongoDB Compass ile bağlan
mongodb://root:example@localhost:27017/?authSource=admin
```

### API Test

```powershell
# FlowService'i test et
curl -X GET "http://localhost:5000/api/flows/tenant/test-tenant"
```

## 📊 Monitoring

### RabbitMQ Metrics

- **Queue Depth**: Mesaj sayısı
- **Message Rate**: Saniyede işlenen mesaj
- **Connection Count**: Aktif bağlantı sayısı

### MongoDB Metrics

- **Document Count**: Koleksiyon boyutu
- **Index Usage**: Index kullanım oranı
- **Query Performance**: Sorgu performansı

## 🔍 Troubleshooting

### RabbitMQ Bağlantı Sorunları

```powershell
# RabbitMQ loglarını kontrol et
docker-compose logs rabbitmq

# RabbitMQ container'ını yeniden başlat
docker-compose restart rabbitmq
```

### MongoDB Bağlantı Sorunları

```powershell
# MongoDB loglarını kontrol et
docker-compose logs mongodb

# MongoDB container'ını yeniden başlat
docker-compose restart mongodb
```

## 🛠️ Geliştirme

### Event Publishing

```csharp
// Event yayınlama
await _eventBus.PublishAsync(new WhatsappMessageReceivedIntegrationEvent
{
    TenantId = "tenant-1",
    UserPhoneNumber = "+905551234567",
    Message = "Merhaba"
});
```

### Event Handling

```csharp
// Event işleme
public class WhatsappMessageReceivedIntegrationEventHandler 
    : IIntegrationEventHandler<WhatsappMessageReceivedIntegrationEvent>
{
    public async Task Handle(WhatsappMessageReceivedIntegrationEvent @event)
    {
        // Event işleme mantığı
    }
}
```

## 📚 Faydalı Komutlar

```powershell
# Tüm servisleri durdur
docker-compose down

# Logları görüntüle
docker-compose logs -f

# Servisleri yeniden başlat
docker-compose restart

# Volume'ları temizle
docker-compose down -v
``` 