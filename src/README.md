# WhatsApp Flow - Mikroservis Mimarisi

Bu proje, WhatsApp Cloud API kullanarak tenant tabanlı mesajlaşma ve flow yönetimi sağlayan mikroservis mimarisidir.

## 🏗️ Mimari Yapı

### Servisler
- **API Gateway** (Port: 5000) - Ocelot ile routing
- **Identity Service** (Port: 5001) - Kullanıcı ve tenant yönetimi
- **Flow Service** (Port: 5002) - Flow motoru ve mesaj işleme
- **Webhook Service** (Port: 5003) - WhatsApp webhook'larını işleme

### Altyapı
- **RabbitMQ** (Port: 5672, Management: 15672) - Event Bus
- **MongoDB** (Port: 27017) - Veri depolama

## 🚀 Kurulum

### Gereksinimler
- Docker Desktop
- .NET 8.0 SDK

### Docker ile Çalıştırma

```bash
# Projeyi klonlayın
git clone <repository-url>
cd Whatsapp.Flow

# Docker Compose ile başlatın
docker-compose up -d

# Logları kontrol edin
docker-compose logs -f
```

### Servis Erişim Bilgileri

| Servis | URL | Açıklama |
|--------|-----|----------|
| API Gateway | http://localhost:5000 | Ana giriş noktası |
| Identity Service | http://localhost:5001 | Kullanıcı yönetimi |
| Flow Service | http://localhost:5002 | Flow motoru |
| Webhook Service | http://localhost:5003 | WhatsApp webhook'ları |
| RabbitMQ Management | http://localhost:15672 | admin/admin123 |
| MongoDB | localhost:27017 | root/example |

## 📋 Event Bus Sistemi

### Exchange Tipleri
- **Topic Exchange** - Routing key ile mesaj yönlendirme
- **Direct Exchange** - Tam eşleşme
- **Fanout Exchange** - Broadcast mesajlar

### Örnek Event'ler

```csharp
// WhatsApp mesajı alındığında
public record WhatsappMessageReceivedIntegrationEvent : IntegrationEvent
{
    public string PhoneNumber { get; init; }
    public string Message { get; init; }
    public string TenantId { get; init; }
    public DateTime MessageTimestamp { get; init; }
    public string MessageId { get; init; }
}
```

### Event Handler Örneği

```csharp
public class WhatsappMessageReceivedIntegrationEventHandler 
    : IIntegrationEventHandler<WhatsappMessageReceivedIntegrationEvent>
{
    public async Task Handle(WhatsappMessageReceivedIntegrationEvent @event)
    {
        // Flow engine'i tetikle
        await _flowEngine.RunAsync(tenant, message);
    }
}
```

## 🔄 Flow Sistemi

### Node Tipleri
- **SendMessageNode** - Mesaj gönderme
- **AskQuestionNode** - Soru sorma ve cevap bekleme
- **DecisionNode** - Koşullu dallanma
- **ButtonNode** - Buton menüsü
- **ListMenuNode** - Liste menüsü
- **WaitNode** - Bekleme
- **WebhookNode** - HTTP webhook çağrısı

### Flow Örneği

```json
{
  "id": "flow-1",
  "name": "Müşteri Hizmetleri",
  "nodes": [
    {
      "id": "node-1",
      "type": "SendMessageNode",
      "content": "Hoş geldiniz! Size nasıl yardımcı olabilirim?"
    },
    {
      "id": "node-2", 
      "type": "AskQuestionNode",
      "content": "Hangi konuda yardım istiyorsunuz?",
      "variableName": "helpTopic"
    }
  ]
}
```

## 🔐 Güvenlik

### API Key Authentication
Tüm servisler `X-API-KEY` header'ı ile korunmaktadır.

```bash
curl -H "X-API-KEY: your-api-key" http://localhost:5000/api/flows
```

## 📊 Monitoring

### RabbitMQ Management
- URL: http://localhost:15672
- Kullanıcı: admin
- Şifre: admin123

### MongoDB Monitoring
```bash
# MongoDB'ye bağlanma
docker exec -it whatsapp-flow-mongodb mongosh -u root -p example

# Veritabanlarını listeleme
show dbs

# Koleksiyonları listeleme
use WhatsappFlowDb
show collections
```

## 🛠️ Geliştirme

### Proje Yapısı
```
src/
├── BuildingBlocks/          # Ortak kütüphaneler
│   ├── Common/             # Ortak modeller
│   └── EventBus/           # Event Bus sistemi
├── Gateways/               # API Gateway
├── Services/               # Mikroservisler
│   ├── IdentityService/    # Kimlik yönetimi
│   ├── FlowService/        # Flow motoru
│   └── WebhookService/     # Webhook işleme
└── docker-compose.yml      # Docker yapılandırması
```

### Yeni Event Ekleme

1. Event sınıfını oluşturun:
```csharp
public record NewIntegrationEvent : IntegrationEvent
{
    public string Data { get; init; }
}
```

2. Handler'ı oluşturun:
```csharp
public class NewIntegrationEventHandler 
    : IIntegrationEventHandler<NewIntegrationEvent>
{
    public async Task Handle(NewIntegrationEvent @event)
    {
        // İşlem mantığı
    }
}
```

3. Program.cs'te kaydedin:
```csharp
services.AddTransient<NewIntegrationEventHandler>();
eventBus.Subscribe<NewIntegrationEvent, NewIntegrationEventHandler>();
```

## 🚨 Hata Ayıklama

### Log Kontrolü
```bash
# Tüm servislerin logları
docker-compose logs

# Belirli servisin logları
docker-compose logs flow-service

# Canlı log takibi
docker-compose logs -f webhook-service
```

### Servis Durumu
```bash
# Container durumları
docker-compose ps

# Servis sağlık kontrolü
docker-compose exec flow-service curl http://localhost/health
```

## 📝 Notlar

- RabbitMQ Topic Exchange kullanılarak mesaj yönlendirme yapılmaktadır
- MongoDB'de tenant bazlı veri ayrımı sağlanmaktadır
- Network kopması durumunda RabbitMQ mesajları yeniden gönderir
- Docker Compose ile tüm servisler otomatik başlatılır 