# 🔗 LinkForge — Enterprise URL Shortener

<div align="center">

![LinkForge](https://img.shields.io/badge/LinkForge-URL%20Shortener-6366f1?style=for-the-badge&logo=link&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React_19-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL_15-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis_7-FF4438?style=for-the-badge&logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

**Production-grade URL shortening platform built with Clean Architecture, CQRS, and real-time analytics.**

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [API Reference](#-api-reference)
- [Security](#-security)
- [Known Issues & Limitations](#-known-issues--limitations)

---

## 🌟 Overview

LinkForge is a full-stack URL shortener built as a 7-day intensive engineering sprint. It goes far beyond simple redirects — featuring a JWT-secured REST API, admin dashboard, per-link click analytics with GeoIP, background processing, Redis caching, Role-Based Access Control (RBAC), and link expiration logic.

---

## ✨ Features

### 👤 User Features
| Feature | Description |
|---|---|
| **Shorten URLs** | Generate short codes or set a custom alias |
| **Link Expiration** | Set an optional expiry date/time (timezone-aware) |
| **Soft Delete** | Deactivate a link — removed from your list, not from the database |
| **Toggle Active/Inactive** | Pause and resume link redirects |
| **Pagination** | Efficiently browse all your links |
| **Click Analytics** | View per-link visitor details: IP, country, user-agent, referrer, timestamp |
| **Link Status Badges** | Visual badges for Active, Inactive, and Expired states |

### 🛡️ Admin Features
| Feature | Description |
|---|---|
| **Admin Dashboard** | Stats panel: total users, total links, active links, total clicks |
| **Manage All Links** | View, activate/deactivate, or hard-delete any link across all users |
| **Manage All Users** | Browse all users and permanently hard-delete a user with all their data |
| **Link Analytics (Admin)** | View analytics for any link in the system |
| **Expired Link Detection** | Expired links show a red "Expired" badge in the admin table |

### ⚡ System / Infrastructure
| Feature | Description |
|---|---|
| **Redis Caching** | Cache-aside pattern for redirect lookups (1-hour TTL) |
| **Background Worker** | In-memory `Channel`-based queue for async analytics writes |
| **Recovery Service** | On startup, re-processes analytics that failed to persist |
| **GeoIP Lookup** | Visitor country resolved from IP on every redirect |
| **Rate Limiting** | 20 link-creation requests/min per IP (fixed window) |
| **JWT + Refresh Token** | Access token (60 min) + refresh token (7 days) rotation |
| **RBAC Permissions** | Granular permission system: `ShortLinks.Create`, `Admin.*`, etc. |
| **Soft Delete via EF Interceptor** | All user-facing deletes are soft — `IsDeleted` flag set automatically |
| **Hard Delete (Admin Only)** | Admin bypass via `IgnoreQueryFilters()` + `ExecuteDeleteAsync()` |

---

## 🏛️ Architecture

The project follows **Clean Architecture** with strict layer separation:

```
src/
├── Core/
│   ├── LinkForge.Domain/          # Entities, base types, domain logic
│   └── LinkForge.Application/     # CQRS (MediatR), DTOs, Validators, Interfaces
├── Infrastructure/
│   ├── LinkForge.Persistence/     # EF Core, AppDbContext, Identity, Migrations
│   └── LinkForge.Infrastructure/  # Redis, JWT, Background services, GeoIP
└── Presentation/
    └── LinkForge.API/             # ASP.NET Controllers, DI wiring, Middleware
frontend/
    └── src/                       # React 19 + TypeScript + TanStack Query
```

### Key Architecture Rules (LinkForge Architecture Rules v2)
- ❌ **No Repository Pattern** — Direct `IAppDbContext` usage in handlers
- ✅ **CQRS via MediatR** — Every operation is a `Command` or `Query`
- ✅ **GlobalUsings.cs** — Shared imports per layer, no repetition
- ✅ **DependencyInjection.cs** per layer — Clean, modular registration
- ✅ **FluentValidation** — All commands validated before handling
- ✅ **EF Core Interceptors** — Audit fields (`CreatedAt`, `UpdatedAt`, `IsDeleted`) set automatically

### Request Flow (Redirect)
```
Browser → GET /{code}
    → RedirectController
    → GetUrlByCodeQuery (MediatR)
    → Redis Cache? → HIT: return URL → Redirect
                  → MISS: query DB → check IsActive, ExpiresAt
                          → cache result (1h) → Redirect
    → (async) UrlVisitQueue.Enqueue(visitDto)
    → UrlVisitAnalyticsWorker reads queue → writes to DB
```

---

## 🧰 Tech Stack

### Backend
| Layer | Technology |
|---|---|
| Runtime | **.NET 10** |
| Web Framework | **ASP.NET Core 10** |
| ORM | **Entity Framework Core 10** |
| Database | **PostgreSQL 15** |
| Cache | **Redis 7** (StackExchange.Redis) |
| Messaging | **MediatR** (CQRS) |
| Validation | **FluentValidation** |
| Auth | **ASP.NET Identity** + **JWT Bearer** |
| RBAC | Custom `IAuthorizationPolicyProvider` + `HasPermission` attribute |
| Background | `IHostedService` + `System.Threading.Channels` |
| API Docs | **NSwag** (OpenAPI/Swagger) |
| GeoIP | `MaxMind.GeoIP2` (or ip-api.com) |

### Frontend
| Layer | Technology |
|---|---|
| Framework | **React 19** |
| Language | **TypeScript** |
| Build Tool | **Vite** |
| Styling | **TailwindCSS v4** |
| State / Data | **TanStack Query (React Query)** |
| HTTP Client | **Axios** (with JWT interceptor) |
| Forms | **react-hook-form** + **Zod** |
| Icons | **Lucide React** |

### Infrastructure
| Service | Technology |
|---|---|
| Containerization | **Docker** + **Docker Compose** |
| CI (optional) | GitHub Actions |

---

## 📦 Prerequisites

Make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) + npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/mirhuseynma/CodeAlpha.git
cd CodeAlpha/UrlShortener
```

### 2. Start infrastructure services (PostgreSQL + Redis)

```bash
# Copy and fill in the environment file
cp .env.example .env
# Edit .env with your values (see Configuration section)

# Start containers
docker compose up -d
```

### 3. Configure the backend

```bash
cd src/Presentation/LinkForge.API

# Copy the development config template and fill it in
cp appsettings.Development.json.example appsettings.Development.json
# (or use dotnet user-secrets — see Configuration section)
```

> [!IMPORTANT]
> You must configure `ConnectionStrings`, `JwtSettings__Secret`, and `AdminSettings` before running.

### 4. Apply database migrations

```bash
# From the solution root
dotnet ef database update --project src/Infrastructure/LinkForge.Persistence --startup-project src/Presentation/LinkForge.API
```

### 5. Run the API

```bash
cd src/Presentation/LinkForge.API
dotnet run
# API runs at: https://localhost:7048
# Swagger UI: https://localhost:7048/swagger
```

### 6. Configure and run the frontend

```bash
cd frontend

# Copy env and set the API URL
cp .env.example .env
# VITE_API_URL=https://localhost:7048/api

npm install
npm run dev
# Frontend runs at: http://localhost:5173
```

### 7. Login as Admin

The admin account is seeded automatically on first run using the values from `AdminSettings` in your config:
- **Email**: value from `AdminSettings__Email`
- **Password**: value from `AdminSettings__Password`

---

## ⚙️ Configuration

### Backend — `appsettings.Development.json`

> ⚠️ This file is **gitignored** and must be created locally. Never commit secrets.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=linkforge_db;Username=postgres;Password=yourpassword",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-jwt-key-minimum-32-characters-long"
  },
  "AdminSettings": {
    "Email": "admin@linkforge.io",
    "Password": "Admin@Str0ngP@ss!"
  }
}
```

> [!TIP]
> For production, use **`dotnet user-secrets`** or environment variables (`JwtSettings__Secret=...`) instead of config files.

### Frontend — `frontend/.env`

```env
VITE_API_URL=https://localhost:7048/api
```

Change the URL to your deployed API in production.

### Docker — `.env` (for docker-compose)

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_strong_password
POSTGRES_DB=linkforge_db
```

---

## 📡 API Reference

Base URL: `https://localhost:7048/api`

### Authentication
| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/auth/register` | Register new user |
| `POST` | `/auth/login` | Login, receive JWT + refresh token |
| `POST` | `/auth/refresh` | Rotate access token using refresh token |
| `POST` | `/auth/confirm-email` | Confirm email address |

### Links (requires auth)
| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/links` | Create short link (with optional alias + expiry) |
| `GET` | `/links` | List your links (paginated) |
| `GET` | `/links/stats` | Get your quick stats |
| `GET` | `/links/{code}` | Get link details |
| `DELETE` | `/links/{id}` | Soft-delete a link |
| `PATCH` | `/links/{id}/status` | Toggle active/inactive |
| `GET` | `/links/{id}/analytics` | Get click analytics (paginated) |

### Redirect (public)
| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/{code}` | Redirect to original URL |

### Admin (requires `Admin` role)
| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/admin/stats` | System-wide statistics |
| `GET` | `/admin/users` | List all users (paginated) |
| `DELETE` | `/admin/users/{id}` | Hard-delete user + all their links |
| `GET` | `/admin/links` | List all links (paginated, includes soft-deleted) |
| `DELETE` | `/admin/links/{id}` | Hard-delete a link permanently |

---

## 🔐 Security

### What's protected
| Concern | How it's handled |
|---|---|
| **Secrets** | `appsettings.Development.json` and `.env` are gitignored |
| **JWT** | Signed with HS256, short-lived (60 min), refresh token rotation |
| **Admin Access** | Role-based (`[HasPermission]` attribute), admin role seeded at startup |
| **Rate Limiting** | 20 requests/min/IP for link creation (FixedWindowLimiter) |
| **Hard Delete** | Only possible via `/api/admin` endpoints, requires `Admin` role |
| **Soft Delete** | Global query filter on `IsDeleted` — users never see deleted data |
| **Input Validation** | FluentValidation on all commands, Zod on all frontend forms |

### ⚠️ Production Checklist
Before deploying to production, make sure to:

- [ ] Replace `CORS AllowAnyOrigin` with specific allowed origins
- [ ] Replace `AllowedHosts: "*"` with your actual domain
- [ ] Use strong, randomly generated JWT secret (≥ 32 chars)
- [ ] Change default Postgres password from `.env.example`
- [ ] Enable HTTPS-only (`UseHttpsRedirection` is already configured)
- [ ] Store all secrets via environment variables or a secrets manager (e.g., Azure Key Vault)
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Restrict Swagger UI to non-production environments (already done in `Program.cs`)
- [ ] Change default admin credentials after first login

---

## 🐛 Known Issues & Limitations

| Issue | Status | Notes |
|---|---|---|
| **CORS is `AllowAnyOrigin`** | 🟠 Open | Safe for development, **must be restricted for production** |
| **`AllowedHosts: "*"`** | 🟠 Open | Must be set to your domain in production |
| **RefreshToken stored in LocalStorage** | 🟡 Low risk | Consider migrating to `HttpOnly` cookies for XSS protection |
| **`appsettings.Development.json` was previously tracked** | ✅ Fixed | Removed from git in latest commit. Local file contains no real secrets. |
| **No email sending** | 🟡 Info | `ForgotPassword` / email confirmation endpoints exist but email transport is not wired up |
| **GeoIP may return Unknown** | 🟡 Info | Free GeoIP tier has rate limits; Unknown is the graceful fallback |

---

## 📝 License

This project was built as part of a CodeAlpha internship sprint. All rights reserved.

---

<div align="center">
  Built with ❤️ using Clean Architecture · CQRS · Redis · PostgreSQL · React
</div>
