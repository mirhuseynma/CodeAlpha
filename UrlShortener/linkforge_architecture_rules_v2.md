# LINKFORGE — C# MONOLİTİK ONİON ARXİTEKTURASI
## Layihə Manifesti və İnkişaf Qaydaları (Development Rules & Conventions)

Bu sənəd **LinkForge (URL Shortener)** layihəsinin arxitektura tamlığını qorumaq, komanda üzvləri üçün vahid standartlar təyin etmək və texniki borcu (technical debt) minimuma endirmək məqsədilə hazırlanmış rəsmi **Layihə Qaydaları Manifestidir**.

---

## Mündəricat
1. [Onion Arxitekturası Qaydaları (Layering & Dependencies)](#1-onion-arxitekturası-qaydaları)
2. [DI və Global Usings Standartları (Critical Rules)](#2-di-və-global-usings-standartları-critical-rules)
3. [C# və Kodlaşdırma Standartları (Clean Code & Async)](#3-c-və-kodlaşdırma-standartları)
4. [Verilənlər Bazası və ORM Qaydaları (EF Core)](#4-verilənlər-bazası-və-orm-qaydaları)
5. [URL Shortener-ə Özəl Performans və Biznes Qaydaları](#5-url-shortener-ə-özəl-performans-və-biznes-qaydaları)
6. [Validasiya, Təhlükəsizlik və Rate Limiting](#6-validasiya-təhlükəsizlik-və-rate-limiting)
7. [Layihə Qovluq və Qat Struktur Standardı](#7-layihə-qovluq-və-qat-struktur-standardı)

---

## 1. Onion Arxitekturası Qaydaları

### 1.1. Domain Qatının Mütləq Məxfiliyi (Zero Dependency Policy)
* **Qayda:** `Domain` layihəsi yalnız `.NET` əsas kitabxanalarından asılı olmalıdır. Hər hansı bir xarici NuGet paketi (`Microsoft.EntityFrameworkCore`, `Newtonsoft.Json`, `AspNetCore` və s.) **qətiyyən əlavə edilməməlidir**.
* **Səbəb:** Domain qatı biznesin ürəyidir. Texnologiyalar və framework-lər dəyişsə belə, Domain toxunulmaz qalmalıdır.
* **Tərkibi:**
  * Entities (`ShortenedUrl`, `UrlVisit`, `User`, `Role` və s.)
  * Value Objects
  * Domain Exceptions
  * Biznes Qaydaları (Business Rules)

### 1.2. Asılılıq İstiqaməti (Dependency Direction)
```
[Presentation (API)] ---> [Infrastructure]
         |                       |
         +-----> [Application] <-+
                       |
                       v
                   [Domain]
```
* **Qayda:** Asılılıq oxu həmişə **xaricdən daxilə** yönəlir:
  * `Application` yalnız `Domain`-i tanıyır.
  * `Infrastructure` və `Persistence` yalnız `Application` və `Domain`-i tanıyır.
  * `Presentation` (API) `Application` ilə işləyir və Dependency Injection (DI) bağlaması üçün digər qatları referans edir.

### 1.3. Dependency Inversion Principle (DIP) & No Repository Pattern
* **Qayda:** Xarici sistemlərlə (DB, Cache, Email, Loglama) hər hansı bir əlaqə üçün interfeys **daxili qatlarda (`Application` və ya `Domain`)** təyin olunmalı, implementasiyası isə **`Infrastructure` / `Persistence`** qatında yazılmalıdır.
* **No Repository Pattern:** EF Core üzərində lazımsız abstraksiya (Repository Pattern) qurulmur. `Application` qatı verilənlər bazası ilə `IApplicationDbContext` interfeysi (və onun içindəki `DbSet<T>` mülkiyyətləri) vasitəsilə əlaqə qurur.

---

## 2. DI və Global Usings Standartları (Critical Rules)

### 2.1. Ultra-Təmiz Program.cs və Mərkəzləşdirilmiş DI (Modular Dependency Injection)
* **Qayda:** `Program.cs` faylı heç vaxt birbaşa servis qeydiyyatları (`builder.Services.AddScoped...`, `AddTransient...` və s.) ilə çirkləndirilməməlidir.
* **İmplementasiya:** Hər bir qat öz servis qeydiyyatları üçün statik **`DependencyInjection.cs`** genişlənmə (extension) sinfinə malik olmalıdır:
  * `AddApplicationServices(this IServiceCollection services)`
  * `AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)`
  * `AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)`
  * `AddPresentationServices(this IServiceCollection services)`
* **Nəticə:** `Program.cs` yalnız qatların metodlarını çağıran, oxunaqlı və maksimum **20-30 sətirlik** təmiz bir kompozisiya nöqtəsi (Composition Root) olaraq qalmalıdır:
  ```csharp
  builder.Services
      .AddApplicationServices()
      .AddPersistenceServices(builder.Configuration)
      .AddInfrastructureServices(builder.Configuration)
      .AddPresentationServices();
  ```

### 2.2. Hər Layihədə Məcburi GlobalUsings.cs (Clean Namespace Imports)
* **Qayda:** Solution daxilindəki hər bir layihənin (`.csproj`) kök qovluğunda xüsusi **`GlobalUsings.cs`** faylı yaradılmalıdır.
* **Tələb:** Ən çox istifadə olunan standart və kitabxana namespace-ləri (`System`, `System.Collections.Generic`, `System.Threading`, `System.Threading.Tasks`, `MediatR`, `FluentValidation`, `Microsoft.EntityFrameworkCore`, `AutoMapper`) yalnız bu faylda qeyd edilməlidir.
* **Səbəb:** Sinif və interface fayllarının yuxarı hissəsində təkrar olunan `using` sətirlərini aradan qaldırmaq, kodun oxunaqlığını maksimum səviyyəyə çatdırmaq və vizual təmizliyi təmin etmək.

---

## 3. C# və Kodlaşdırma Standartları

### 3.1. Entity-lər Əsla API-dən Çölə Çıxmır (No Bare Entities)
* **YALNIŞ:** Controller endpoint-in birbaşa Domain entitisini qaytarması.
* **DOĞRU:** Bütün əməliyyatlarda **DTO** (Data Transfer Object) və ya **CQRS (Command/Query)** modellərindən istifadə olunmalıdır.
  ```csharp
  public record ShortenedUrlResponseDto(string ShortCode, string ShortUrl, DateTime CreatedAt);
  ```

### 3.2. Asinxronluq Standartı (Async/Await Always)
* **Qayda:** Bütün I/O əməliyyatları (DB, File, HTTP, Cache) istisnasız olaraq asinxron olmalı, metod adları `...Async` şəkilçisi ilə bitməli və `CancellationToken` dəstəkləməlidir.
  ```csharp
  Task<ShortenedUrlDto?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);
  ```

### 3.3. Result Pattern və Xəta İdarəetməsi (Global Exception Handling)
* **Qayda:** Biznes axınlarında istisnalardan (Exception) idarəetmə mexanizmi kimi istifadə **qadağandır**. Bütün biznes metodları **`Result`** və ya **`Result<T>`** qaytarmalıdır.
* **Exception Handling:** İstisnalar yalnız gözlənilməz sistem xətaları üçün atılır və ASP.NET Core **Global Exception Middleware / Exception Handler** vasitəsilə tutulub standart HTTP status kodlarına çevrilir.

### 3.4. TimeProvider İstifadəsi
* **Qayda:** Kod daxilində heç vaxt birbaşa `DateTime.UtcNow` və ya `DateTime.Now` yazılmamalıdır. Sistem saatı ilə bağlı bütün əməliyyatlarda test oluna bilən **`TimeProvider`** inyeksiya edilib istifadə olunmalıdır.

---

## 4. Verilənlər Bazası və ORM Qaydaları (EF Core)

| Qayda | Təsvir | Nümunə |
| :--- | :--- | :--- |
| **Fluent API Only** | Data Annotation-lardan (`[Required]`, `[Table]`) istifadə **qadağandır**. Bütün DB konfiqurasiyaları `IEntityTypeConfiguration<T>` siniflərində olmalıdır. | `ShortenedUrlConfiguration.cs` |
| **NoTracking on Reads** | Sırf oxumaq məqsədli (ReadOnly) sorğularda yaddaşa qənaət və performans üçün `.AsNoTracking()` istifadə edilməlidir. | `dbContext.ShortenedUrls.AsNoTracking().FirstOrDefaultAsync(...)` |
| **Unique Indexing** | Qısa kodların toqquşmasının önünü kəsmək üçün `ShortCode` sütununa baza səviyyəsində unikal indeks qoyulmalıdır. | `builder.HasIndex(x => x.ShortCode).IsUnique();` |
| **Base Entity Audit** | Bütün entitilər ortaq audit xüsusiyyətlərinə malik təməl sinifdən törəməlidir: `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `DeletedAt`, `DeletedBy`, `IsDeleted`. | `public class ShortenedUrl : BaseAuditableEntity` |
| **Soft Delete** | Məlumatlar fiziki olaraq silinmir. Əvəzinə `IsDeleted` flag-i və EF Core **Global Query Filter** tətbiq edilir. | `builder.HasQueryFilter(x => !x.IsDeleted);` |

---

## 5. URL Shortener-ə Özəl Performans və Biznes Qaydaları

### 5.1. "Cache-First" Oxuma Strategiyası (Yönləndirmə Sürəti)
```
[İstifadəçi: GET /{code}]
          |
          v
   +--------------+  CACHE HIT    +--------------------------+
   | Redis Cache? | ------------> | 301/302 Redirect Qaytar |
   +--------------+               +--------------------------+
          |
          | CACHE MISS
          v
   +--------------+               +--------------------------+
   |  DB Sorğusu  | ------------> | Redis Cache-i Yenilə     |
   +--------------+               +--------------------------+
```
* **Qayda:** Hər hansı bir yönləndirmə (`GET /{code}`) zamanı ilk öncə Redis/MemoryCache yoxlanmalıdır. Əgər məlumat keştə varsa, Verilənlər Bazasını narahat etmədən dərhal yönləndirmə icra olunmalıdır.

### 5.2. Base62 Qısa Kod Alqoritmi
* **Qayda:** Qısa kodlar (ShortCode) təsadüfi `Guid` və ya ağır simvollar yerinə **Base62 (0-9, a-z, A-Z)** alqoritmi ilə yaranmalıdır. Bu, həm kodun qısa olmasını, həm də URL-safe olmasını təmin edir.

### 5.3. Asinxron Analitika (Zero-Blocking Analytics)
* **Qayda:** Yönləndirmə zamanı klik sayğacı, IP ünvanı, User-Agent və ziyarət tarixi kimi məlumatların qeydə alınması **əsas sorğunu ləngitməməlidir**.
* **İmplementasiya:**
  1. Yönləndirmə sorğusu məlumatı yaddaş daxili asinxron növbəyə (**`System.Threading.Channels.Channel<UrlVisitEvent>`**) göndərir.
  2. İstifadəçiyə dərhal `302 Found` və ya `301 Moved Permanently` qaytarılır.
  3. Arxa fonda çalışan **`BackgroundService` (HostedService)** növbədən paketləri oxuyub, toplu şəkildə (batch insert) bazaya qeyd edir.

---

## 6. Validasiya, Təhlükəsizlik və Rate Limiting

1. **FluentValidation Standartı:**
   * Daxil olan bütün Command və DTO-lar **FluentValidation** vasitəsilə doğrulama konveyerindən (validation pipeline) keçməlidir. API Controller daxilində manual validasiya yoxlanışları yazılmır.
2. **Infinite Loop Prevention:**
   * İstifadəçi sistemin öz domeninə aid URL-i qısaltmağa cəhd etdikdə xəta verilməlidir.
3. **Rate Limiting:**
   * `POST /api/shorten` endpoint-inə IP əsaslı limit tətbiq olunmalıdır.
4. **Security & Identity:**
   * ASP.NET Core Identity (xüsusi JWT və Refresh Token arxitekturası) istifadə olunacaq. Hər hansı bir parolu şifrələmədən loglamaq və ya açıq saxlamaq qəti qadağandır.
   * Cari istifadəçi məlumatlarına yalnız `ICurrentUserService` interfeysi üzərindən çıxış edilməlidir.

---

## 7. Layihə Qovluq və Qat Struktur Standardı (Feature-Based)

```text
LinkForge.Solution/
│
├── src/
│   ├── 1.Core/
│   │   ├── LinkForge.Domain/
│   │   │   ├── Common/
│   │   │   │   └── BaseAuditableEntity.cs
│   │   │   ├── Entities/
│   │   │   │   ├── ShortenedUrl.cs
│   │   │   │   └── UrlVisit.cs
│   │   │   ├── Exceptions/
│   │   │   └── GlobalUsings.cs
│   │   │
│   │   └── LinkForge.Application/
│   │       ├── Common/
│   │       │   ├── Interfaces/
│   │       │   │   ├── IApplicationDbContext.cs
│   │       │   │   ├── ICacheService.cs
│   │       │   │   └── ICurrentUserService.cs
│   │       │   └── Models/
│   │       │       └── Result.cs
│   │       ├── Features/
│   │       │   ├── Authentication/
│   │       │   ├── Links/
│   │       │   │   ├── Commands/
│   │       │   │   │   ├── CreateShortLinkCommand.cs
│   │       │   │   │   └── CreateShortLinkCommandHandler.cs
│   │       │   │   └── Queries/
│   │       │   └── Analytics/
│   │       ├── DependencyInjection.cs
│   │       └── GlobalUsings.cs
│   │
│   ├── 2.Infrastructure/
│   │   ├── LinkForge.Persistence/
│   │   │   ├── Contexts/
│   │   │   │   └── ApplicationDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   └── ShortenedUrlConfiguration.cs
│   │   │   ├── DependencyInjection.cs
│   │   │   └── GlobalUsings.cs
│   │   │
│   │   └── LinkForge.Infrastructure/
│   │       ├── Caching/
│   │       │   └── RedisCacheService.cs
│   │       ├── Identity/
│   │       ├── Services/
│   │       │   ├── Base62UrlShortenerService.cs
│   │       │   └── Background/
│   │       │       └── UrlVisitAnalyticsWorker.cs
│   │       ├── DependencyInjection.cs
│   │       └── GlobalUsings.cs
│   │
│   └── 3.Presentation/
│       └── LinkForge.API/
│           ├── Controllers/
│           │   ├── LinksController.cs
│           │   └── RedirectController.cs
│           ├── Middlewares/
│           │   └── GlobalExceptionHandlerMiddleware.cs
│           ├── DependencyInjection.cs
│           ├── GlobalUsings.cs
│           └── Program.cs
│
└── tests/
    ├── LinkForge.UnitTests/
    └── LinkForge.IntegrationTests/
```

---
*Bu manifest layihə boyu təməl fəlsəfəmiz olaraq qalacaq. Hər yeni xüsusiyyət (feature) və arxitektura qərarı bu sənədə əsasən kod rezyumesindən keçiriləcək.*
