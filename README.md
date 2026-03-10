# 📦 Enterprise Order Management System

Bu proje, modern yazılım mimarileri ve kurumsal tasarım desenleri (Design Patterns) kullanılarak geliştirilmiş, ölçeklenebilir bir **Sipariş Yönetim Sistemi** ekosistemidir.

## 🏗️ Mimari Yapı: Clean Architecture
Proje, bağımlılıkların içe doğru (Domain'e) olduğu 4 temel katman üzerine inşa edilmiştir:
- **Domain:** Entity'ler ve iş kuralları.
- **Application:** CQRS Handlers (MediatR), DTO'lar, Validators (FluentValidation) ve Mapping (AutoMapper).
- **Infrastructure:** Veri erişimi (EF Core & Dapper), Identity yönetimi, JWT üretimi ve Background Jobs (Hangfire).
- **API:** RESTful endpoint'ler, Middleware ve Global Exception Handling.

## 🚀 Teknik Özellikler & Uygulanan Patternler
- **CQRS (MediatR):** Komut (Yazma) ve Sorgu (Okuma) sorumlulukları ayrılarak performans ve okunabilirlik artırıldı.
- **Hibrit ORM Yaklaşımı:** Yazma işlemleri için **Entity Framework Core**, yüksek performanslı raporlama sorguları için **Dapper** kullanıldı.
- **Güvenlik:** ASP.NET Core Identity altyapısı ile **JWT (JSON Web Token)** tabanlı kimlik doğrulama ve Role-based yetkilendirme sistemi.
- **Asenkron Arka Plan İşleri:** Sipariş onay mailleri gibi süreçler için **Hangfire** kullanılarak kullanıcı deneyimi (UX) iyileştirildi.
- **Performans:** Sık erişilen veriler için **Distributed Caching (MemoryCache/Redis)** ve büyük veri setleri için dinamik **Pagination** (Sayfalama).
- **Global Exception Handling:** Tüm uygulama genelinde hataların merkezi bir Middleware üzerinden yakalanıp standart bir hata şablonuyla dönülmesi sağlandı.
- **Soft Delete & Query Filters:** Verilerin fiziksel silinmesi engellenerek `IsDeleted` bayrağı üzerinden otomatik filtrelenmesi sağlandı.

## 🛠️ Teknoloji Yığını
- **Framework:** .NET 8 (ASP.NET Core)
- **Database:** MS SQL Server (LocalDB)
- **Patterns:** CQRS, Repository, Unit of Work, Dependency Injection
- **Libraries:** MediatR, AutoMapper, FluentValidation, Serilog, Hangfire, Dapper
- **DevOps:** Docker, Docker Compose (Hazır yapılandırma)

## 🔧 Kurulum ve Çalıştırma
1. Projeyi klonlayın.
2. `OrderService.API` içerisindeki `appsettings.json` dosyasında bağlantı dizesini kontrol edin.
3. Veritabanını oluşturun:
   ```bash
   dotnet ef database update --project src/OrderService.Infrastructure --startup-project src/OrderService.API
