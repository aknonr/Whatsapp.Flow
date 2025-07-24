# RabbitMQ Test Scripti
Write-Host "🐰 RabbitMQ Test Scripti Başlatılıyor..." -ForegroundColor Green

# Docker Compose ile servisleri başlat
Write-Host "📦 Docker Compose ile servisler başlatılıyor..." -ForegroundColor Yellow
docker-compose up -d

# Servislerin hazır olmasını bekle
Write-Host "⏳ Servislerin başlaması bekleniyor..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# RabbitMQ Management UI'ı kontrol et
Write-Host "🔍 RabbitMQ Management UI kontrol ediliyor..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:15672" -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ RabbitMQ Management UI erişilebilir: http://localhost:15672" -ForegroundColor Green
        Write-Host "👤 Kullanıcı: admin, Şifre: admin123" -ForegroundColor Cyan
    }
} catch {
    Write-Host "❌ RabbitMQ Management UI erişilemiyor" -ForegroundColor Red
}

# MongoDB bağlantısını kontrol et
Write-Host "🔍 MongoDB bağlantısı kontrol ediliyor..." -ForegroundColor Yellow
try {
    $mongoResponse = Invoke-WebRequest -Uri "http://localhost:27017" -UseBasicParsing -TimeoutSec 5
    Write-Host "✅ MongoDB erişilebilir: localhost:27017" -ForegroundColor Green
} catch {
    Write-Host "❌ MongoDB erişilemiyor" -ForegroundColor Red
}

Write-Host "`n📋 Test Sonuçları:" -ForegroundColor Cyan
Write-Host "• RabbitMQ: http://localhost:15672" -ForegroundColor White
Write-Host "• MongoDB: localhost:27017" -ForegroundColor White
Write-Host "• Docker Compose: docker-compose ps" -ForegroundColor White

Write-Host "`n🚀 Test tamamlandı!" -ForegroundColor Green 