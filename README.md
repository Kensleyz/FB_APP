# PageBoost AI 🚀

**AI-Powered Facebook Business Page Optimizer for South African Small Businesses**

A SaaS product helping township businesses (spaza shops, salons, churches, etc.) manage and optimize their Facebook Business Pages using AI-generated content, automated scheduling, and performance analytics.

**Target:** R50,000/month revenue by Month 6 | 1,000 paying users | R30–R150/month subscriptions

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Running the App](#running-the-app)
- [Database Migrations](#database-migrations)
- [Testing](#testing)
- [API Documentation](#api-documentation)
- [Deployment](#deployment)
- [External Services Setup](#external-services-setup)
- [Subscription Tiers](#subscription-tiers)

---

## Tech Stack

| Layer            | Technology                          |
| ---------------- | ----------------------------------- |
| **Backend**      | .NET 9 Web API (C#)                |
| **Frontend**     | React 19 + TypeScript + Vite        |
| **Styling**      | Tailwind CSS v4                     |
| **Database**     | PostgreSQL 15+                      |
| **Caching**      | Redis 7                             |
| **Background**   | Hangfire (PostgreSQL storage)       |
| **Auth**         | JWT (access + refresh tokens)       |
| **Payments**     | PayFast (South African gateway)     |
| **AI**           | Anthropic Claude API (Sonnet 4.5)   |
| **State Mgmt**   | Zustand                            |
| **HTTP Client**  | Axios                               |
| **Icons**        | Lucide React                        |
| **Routing**      | React Router v7                     |
| **Containerization** | Docker + Docker Compose         |

---

## Architecture

Clean Architecture with DDD bounded contexts:

```
┌─────────────────────────────────────────────┐
│                  API Layer                  │
│  Controllers · Middleware · Filters · JWT   │
├─────────────────────────────────────────────┤
│              Application Layer              │
│  CQRS Handlers · Validators · DTOs · MediatR│
├─────────────────────────────────────────────┤
│                Domain Layer                 │
│  Entities · Value Objects · Interfaces      │
├─────────────────────────────────────────────┤
│            Infrastructure Layer             │
│  EF Core · Redis · Hangfire · External APIs │
└─────────────────────────────────────────────┘
```

**Bounded Contexts:** Auth, Content, Scheduling, Billing, Facebook Integration

---

## Project Structure

```
PageBoostAI/
├── src/
│   ├── PageBoostAI.Domain/            # Entities, value objects, domain interfaces
│   ├── PageBoostAI.Application/       # Use cases, CQRS, validators (MediatR + FluentValidation)
│   ├── PageBoostAI.Infrastructure/    # EF Core, external services, caching, jobs
│   ├── PageBoostAI.Api/               # Controllers, middleware, DI config, Program.cs
│   └── PageBoostAI.Tests/             # xUnit tests (NSubstitute, FluentAssertions, AutoFixture)
├── frontend/                          # React + TypeScript + Vite
│   ├── src/
│   ├── package.json
│   ├── vite.config.ts
│   ├── nginx.conf                     # Production SPA + API proxy
│   └── Dockerfile
├── docker-compose.yml                 # Full stack: API, frontend, PostgreSQL, Redis
├── Dockerfile                         # Backend multi-stage build
├── PageBoostAI.sln
├── NuGet.Config
└── BRS.md                             # Business Requirements Specification
```

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/) (LTS)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for containerised dev)
- [PostgreSQL 15+](https://www.postgresql.org/) (or use Docker)
- [Redis 7+](https://redis.io/) (or use Docker)
- Git

---

## Getting Started

### 1. Clone the repo

```bash
git clone <your-repo-url>
cd FB_APP
```

### 2. Set up environment variables

Create a `.env` file in the project root (used by `docker-compose.yml`):

```env
# Database
DATABASE_URL=Host=postgres;Port=5432;Database=pageboost_db;Username=pageboost;Password=pageboost_dev
POSTGRES_USER=pageboost
POSTGRES_PASSWORD=pageboost_dev
POSTGRES_DB=pageboost_db

# JWT
JWT_SECRET_KEY=your-super-secret-key-at-least-32-characters-long!!
JWT_ISSUER=PageBoostAI
JWT_AUDIENCE=PageBoostAI

# Anthropic Claude
ANTHROPIC_API_KEY=sk-ant-your-key-here
ANTHROPIC_MODEL=claude-sonnet-4-5-20250929

# Facebook OAuth
FACEBOOK_APP_ID=your-facebook-app-id
FACEBOOK_APP_SECRET=your-facebook-app-secret
FACEBOOK_REDIRECT_URI=http://localhost:5000/api/v1/facebook/callback

# PayFast (sandbox credentials for dev)
PAYFAST_MERCHANT_ID=10000100
PAYFAST_MERCHANT_KEY=46f0cd694581a
PAYFAST_PASSPHRASE=jt7NOE43FZPn
PAYFAST_MODE=sandbox

# Unsplash
UNSPLASH_ACCESS_KEY=your-unsplash-access-key

# Redis
REDIS_URL=redis://redis:6379

# URLs
FRONTEND_URL=http://localhost:3000
API_URL=http://localhost:5000
```

---

## Running the App

### Option A: Docker Compose (recommended)

Spins up everything — API, frontend, PostgreSQL, and Redis:

```bash
docker-compose up --build
```

| Service      | URL                          |
| ------------ | ---------------------------- |
| Frontend     | http://localhost:3000         |
| API          | http://localhost:5000         |
| Swagger      | http://localhost:5000/swagger |
| Hangfire      | http://localhost:5000/hangfire|
| PostgreSQL   | localhost:5432                |
| Redis        | localhost:6379                |
| Health Check | http://localhost:5000/health  |

### Option B: Run locally (without Docker)

#### Backend

```bash
# Ensure PostgreSQL and Redis are running locally

# Restore packages
dotnet restore

# Run the API (uses appsettings.Development.json)
cd src/PageBoostAI.Api
dotnet run
```

#### Frontend

```bash
cd frontend

# Install dependencies
npm install

# Start dev server (hot reload on http://localhost:5173)
npm run dev
```

> **Note:** When running locally, the Vite dev server runs on port 5173. The API CORS config already allows this origin.

---

## Configuration

### appsettings.json (Backend)

Key configuration sections in `src/PageBoostAI.Api/appsettings.json`:

| Section              | Purpose                                |
| -------------------- | -------------------------------------- |
| `ConnectionStrings`  | PostgreSQL + Redis connection strings  |
| `JwtSettings`        | Token signing key, issuer, audience, expiry |
| `AnthropicSettings`  | Claude API key + model                 |
| `FacebookSettings`   | OAuth app ID/secret, Graph API version |
| `PayFastSettings`    | Merchant credentials, sandbox toggle   |
| `UnsplashSettings`   | API key for stock photos               |
| `CorsSettings`       | Allowed frontend origins               |
| `Serilog`            | Structured logging config              |

### Dev overrides

`appsettings.Development.json` provides sandbox/dev defaults (PayFast sandbox creds, debug logging, local DB).

---

## Database Migrations

Uses EF Core with PostgreSQL. Run from the solution root:

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> \
  --project src/PageBoostAI.Infrastructure \
  --startup-project src/PageBoostAI.Api

# Apply migrations
dotnet ef database update \
  --project src/PageBoostAI.Infrastructure \
  --startup-project src/PageBoostAI.Api
```

> Hangfire automatically creates its own schema tables on first run.

---

## Testing

Test stack: **xUnit** + **NSubstitute** (mocking) + **FluentAssertions** + **AutoFixture**

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run a specific test project
dotnet test src/PageBoostAI.Tests
```

**Testing philosophy:** TDD — write tests before implementation. Tests live in `src/PageBoostAI.Tests/`.

---

## API Documentation

Swagger UI is available in development at:

```
http://localhost:5000/swagger
```

### Key API routes

| Area         | Endpoint                         | Auth |
| ------------ | -------------------------------- | ---- |
| Register     | `POST /api/v1/auth/register`     | No   |
| Login        | `POST /api/v1/auth/login`        | No   |
| Current User | `GET /api/v1/auth/me`            | Yes  |
| FB Connect   | `POST /api/v1/facebook/connect`  | Yes  |
| Generate AI  | `POST /api/v1/content/generate`  | Yes  |
| Schedule     | `POST /api/v1/schedule`          | Yes  |
| Subscribe    | `POST /api/v1/billing/subscribe` | Yes  |
| Dashboard    | `GET /api/v1/dashboard/overview` | Yes  |
| PayFast Hook | `POST /api/v1/webhooks/payfast`  | No*  |

*PayFast webhook uses signature validation instead of JWT.

---

## Deployment

### Docker Production Build

```bash
# Build and tag
docker build -t pageboost-api .
docker build -t pageboost-frontend ./frontend

# Or use compose
docker-compose -f docker-compose.yml up -d
```

### CI/CD

GitHub Actions pipeline (to be configured):
1. Run tests on PR
2. Build Docker images
3. Deploy to Render.com / Railway on push to `main`
4. Blue-green deployment strategy

### Hosting targets

- **Compute:** Docker containers on Render.com or Railway
- **Database:** Managed PostgreSQL (~R300/month)
- **Redis:** Managed or Docker sidecar
- **Domain:** `.co.za` (~R150/year)
- **SSL:** Let's Encrypt (free)

---

## External Services Setup

### 1. Facebook App

1. Go to [Facebook Developers](https://developers.facebook.com/)
2. Create a new app → Business type
3. Add **Facebook Login** product
4. Set redirect URI: `https://yourdomain.com/api/v1/facebook/callback`
5. Request permissions: `pages_manage_posts`, `business_management`
6. Copy App ID + App Secret → env vars

### 2. Anthropic Claude

1. Sign up at [console.anthropic.com](https://console.anthropic.com/)
2. Create an API key
3. Budget: ~R500/month (~R0.06/post)
4. Model: `claude-sonnet-4-5-20250929`

### 3. PayFast

1. Register at [payfast.co.za](https://www.payfast.co.za/)
2. Get Merchant ID, Merchant Key, Passphrase
3. Use [sandbox](https://sandbox.payfast.co.za/) for development
4. Configure ITN (webhook) URL: `https://yourdomain.com/api/v1/webhooks/payfast`

### 4. Unsplash

1. Register at [unsplash.com/developers](https://unsplash.com/developers)
2. Create an app → get Access Key
3. Free tier: 50 requests/hour

---

## Subscription Tiers

| Tier                | Price       | Posts/mo | Pages | Images/mo |
| ------------------- | ----------- | -------- | ----- | --------- |
| **Free**            | R0          | 5        | 1     | 2         |
| **Starter**         | R30/month   | 10       | 1     | 5         |
| **Growth**          | R50/month   | 30       | 3     | 15        |
| **Pro**             | R150/month  | 100      | 10    | 50        |

Growth tier includes a 7-day free trial.

---

## Background Jobs (Hangfire)

| Job               | Schedule       | Purpose                              |
| ------------------ | -------------- | ------------------------------------ |
| Post Publishing    | Every minute   | Check & publish scheduled posts      |
| Token Refresh      | Every 6 hours  | Refresh Facebook long-lived tokens   |

Dashboard: `http://localhost:5000/hangfire` (requires authentication)

---

## Security Highlights

- **Passwords:** BCrypt (cost factor 12)
- **Tokens:** JWT access (60 min) + refresh (7 days) in HttpOnly cookies
- **Rate Limiting:** 100 requests/hour per IP (AspNetCoreRateLimit)
- **Facebook tokens:** AES-256 encrypted at rest
- **CORS:** Whitelisted origins only
- **Secrets:** Environment variables, never committed

---

## Monitoring

- **Logging:** Serilog (structured, console sink)
- **Error Tracking:** Sentry (free tier)
- **Uptime:** UptimeRobot
- **Performance:** < 200ms p95 API response, < 5s AI generation

---

## License

Private — All rights reserved.
