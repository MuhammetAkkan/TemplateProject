# Template Clean Architecture v3

Bu proje, .NET 9 kullanılarak Clean Architecture prensiplerine göre hazırlanmış bir şablon (template) projedir.

## 🏗️ Proje Yapısı

- **App.Domain**: Entity'ler ve repository interface'leri
- **App.Application**: CQRS pattern (MediatR), business logic
- **App.Infrastructure**: Database, cache, external servislerin implementasyonları
- **App.Presentation**: API endpoints (Controllers)
- **App.Shared**: Ortak kullanılan modeller (Result, PagedResult)
- **App.ArchitectureTests**: Mimari kuralların test edilmesi

## 🚀 Teknolojiler

- .NET 9.0
- PostgreSQL (Veritabanı)
- Redis (Cache)
- Docker & Docker Compose
- Entity Framework Core
- MediatR (CQRS)
- FluentValidation
- Hangfire (Background Jobs)
- Scalar (API Documentation)
- JWT Authentication

## 📦 Kurulum

### Docker ile Çalıştırma (Önerilen)

```bash
# Tüm servisleri ayağa kaldır
docker-compose up -d

# Sadece rebuild etmek için
docker-compose up -d --build
```

Uygulama çalıştıktan sonra:
- API: http://localhost:5000
- Scalar (API Docs): http://localhost:5000/scalar/v1
- Health Check: http://localhost:5000/health
- Hangfire Dashboard: http://localhost:5000/hangfire

### Manuel Kurulum

1. PostgreSQL ve Redis'in çalıştığından emin olun
2. `appsettings.Development.json` dosyasında connection string'leri düzenleyin
3. Migration'ları çalıştırın:
```bash
dotnet ef database update --project App.Presentation
```
4. Uygulamayı çalıştırın:
```bash
dotnet run --project App.Presentation
```

## 🔧 Geliştirme

### Yeni Feature Ekleme

1. `App.Domain/Entities` içine entity ekleyin
2. `App.Domain/Repositories` içine repository interface ekleyin
3. `App.Application/Features` içine CQRS komutları/sorguları ekleyin
4. `App.Infrastructure/Persistence/Repositories` içine repository implementasyonu ekleyin
5. `App.Presentation/Controllers` içine controller ekleyin

### Migration Oluşturma

```bash
dotnet ef migrations add MigrationName --project App.Infrastructure --startup-project App.Presentation
dotnet ef database update --project App.Infrastructure --startup-project App.Presentation
```

## 📝 Lisans

Bu bir şablon projedir, istediğiniz gibi kullanabilirsiniz.
