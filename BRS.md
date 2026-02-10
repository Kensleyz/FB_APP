# PROJECT: PAGEBOOST AI - AI-POWERED FACEBOOK BUSINESS PAGE OPTIMIZER

## CONTEXT
You are building a SaaS product for South African small businesses to manage and optimize their Facebook Business Pages using AI. This is a revenue-generating product targeting township businesses (spaza shops, salons, churches, etc.) with subscription pricing from R30-R150/month.

## BUSINESS GOALS
- Launch MVP in 8 weeks
- Acquire 50 beta users
- Target R50,000/month revenue by Month 6
- 1,000 paying users within 6 months

## TECHNICAL REQUIREMENTS

### ARCHITECTURE
- **Backend:** .NET 8 Web API (C#)
- **Frontend:** React 18+ with TypeScript
- **Database:** PostgreSQL 15+
- **Caching:** Redis
- **Background Jobs:** Hangfire
- **Authentication:** JWT (access + refresh tokens)
- **Payment Gateway:** PayFast (South African)
- **AI Provider:** Anthropic Claude API (Sonnet 4.5)
- **Image Processing:** ImageSharp + Unsplash API
- **Deployment:** Docker containers on Render.com or Railway
- **Version Control:** Git with trunk-based development

### DESIGN PRINCIPLES - MANDATORY
1. **Domain-Driven Design (DDD):**
   - Bounded contexts: Auth, Content, Scheduling, Billing, Facebook Integration
   - Aggregate roots: User, FacebookPage, ContentSchedule, Subscription
   - Value objects for immutable data
   - Domain events for cross-context communication
   - Repository pattern for data access
   - CQRS where beneficial

2. **Test-Driven Development (TDD):**
   - Write tests BEFORE implementation
   - Minimum 80% code coverage
   - Unit tests for all business logic
   - Integration tests for APIs
   - E2E tests for critical user flows
   - Use xUnit for .NET, Jest/React Testing Library for frontend

3. **Clean Architecture:**
   - Domain layer (entities, value objects, domain events)
   - Application layer (use cases, DTOs, interfaces)
   - Infrastructure layer (EF Core, external APIs, file storage)
   - Presentation layer (API controllers, React components)

## CORE FEATURES

### 1. USER MANAGEMENT
- Email/password registration with verification
- JWT-based authentication (access + refresh tokens)
- Password reset flow
- User profile management
- Subscription tier tracking (Free, Starter R30, Growth R50, Pro R150)
- Usage limits enforcement per tier

### 2. FACEBOOK INTEGRATION
- OAuth 2.0 connection to Facebook
- Multi-page support (1-10 pages depending on tier)
- Long-lived page access token management
- Auto-refresh token mechanism
- Fetch page insights (followers, engagement)
- Publish posts with text and images
- Schedule posts for future publishing

### 3. AI CONTENT GENERATION
**Prompt Engineering Requirements:**
- Generate Facebook posts (max 280 chars)
- 3 variations per generation
- Context-aware prompts for South African businesses
- Support business types:
  - Spaza shops (township stores)
  - Hair salons
  - Churches
  - Restaurants
  - Gyms
  - Funeral parlors
  - Taxi associations
  - General businesses
- Tone options: Professional, Casual, Friendly, Energetic, Respectful
- Post types: Promotional, Educational, Engagement, Event, Product Showcase, Community
- Language: English with optional Afrikaans phrases (code-switching)
- Include emojis naturally in text
- Generate 2-4 hashtags per post
- Include clear call-to-action
- Cultural context: township slang, local references, SA holidays

**Image Generation:**
- Integrate Unsplash API for stock photos (free tier: 50/hour)
- Use ImageSharp to add text overlays
- Add business logo/branding
- Optimize for Facebook (1200x630px)
- Support cover photos and post images

### 4. CONTENT SCHEDULER
- Calendar view for scheduled posts
- Drag-and-drop scheduling
- Edit scheduled posts before publishing
- Automatic publishing via Hangfire background jobs
- Retry logic (3 attempts on failure)
- Post history and status tracking
- Timezone support (SAST)
- Posting rules: max 4/day, no posts 10pm-6am

### 5. SUBSCRIPTION & BILLING (PAYFAST)
**Subscription Tiers:**
- **Free:** 5 posts/month, 1 page, 2 images/month
- **Starter (R30/month):** 10 posts/month, 1 page, 5 images/month
- **Growth (R50/month):** 30 posts/month, 3 pages, 15 images/month
- **Pro (R150/month):** 100 posts/month, 10 pages, 50 images/month

**PayFast Integration:**
- Subscription creation and management
- Recurring billing (monthly)
- Webhook handling for payment events
- Payment success/failure handling
- Subscription cancellation
- Upgrade/downgrade flows
- 7-day free trial for Growth tier
- Invoice/receipt generation

**Usage Tracking:**
- Track posts generated, images created, posts published
- Enforce limits per tier
- Show usage meter in dashboard
- Prompt upgrade when limits reached

### 6. DASHBOARD & ANALYTICS
- Overview: connected pages, usage stats, upcoming posts
- Calendar view of scheduled content
- Recent posts with engagement metrics
- Top-performing posts
- Usage breakdown by month
- Quick actions: generate post, schedule post, connect page

## DATABASE SCHEMA

### Tables Required:
1. **Users**
   - Id (UUID, PK)
   - Email (unique, indexed)
   - PasswordHash
   - FirstName, LastName, PhoneNumber
   - SubscriptionTier (enum: Free/Starter/Growth/Pro)
   - SubscriptionExpiresAt
   - IsEmailVerified, EmailVerificationToken
   - PasswordResetToken, PasswordResetExpiry
   - CreatedAt, UpdatedAt, LastLoginAt

2. **FacebookPages**
   - Id (UUID, PK)
   - UserId (FK to Users)
   - FacebookPageId (indexed)
   - PageName, PageCategory
   - PageAccessToken (encrypted)
   - AccessTokenExpiresAt
   - ProfilePictureUrl, FollowerCount
   - IsActive, ConnectedAt, LastSyncedAt

3. **ContentSchedule**
   - Id (UUID, PK)
   - PageId (FK to FacebookPages)
   - Content (text)
   - ImageUrl
   - Hashtags (array)
   - CallToAction
   - ScheduledFor (indexed)
   - Status (Scheduled/Published/Failed/Cancelled)
   - FacebookPostId
   - ErrorMessage, RetryCount
   - CreatedAt, PublishedAt

4. **Subscriptions**
   - Id (UUID, PK)
   - UserId (FK to Users)
   - PayFastSubscriptionToken
   - Tier, Amount, Currency
   - Status (Active/Cancelled/Suspended/PastDue)
   - StartDate, NextBillingDate, CancelledAt

5. **PaymentTransactions**
   - Id (UUID, PK)
   - UserId, SubscriptionId (FKs)
   - PayFastPaymentId
   - Amount, Currency
   - Status (Pending/Complete/Failed/Refunded)
   - PaymentMethod, TransactionType
   - Metadata (JSONB for PayFast response)
   - CreatedAt, CompletedAt

6. **UsageMetrics**
   - Id (UUID, PK)
   - UserId (FK)
   - Period (YYYY-MM format, indexed)
   - PostsGenerated, ImagesCreated, PostsPublished, ApiCallsCount

Add appropriate indexes for performance:
- Users: Email, SubscriptionTier+ExpiresAt
- FacebookPages: UserId, IsActive
- ContentSchedule: PageId, ScheduledFor+Status
- Subscriptions: UserId, Status
- Transactions: UserId, SubscriptionId, Status
- UsageMetrics: UserId+Period

## API ENDPOINTS

### Authentication
- POST /api/v1/auth/register
- POST /api/v1/auth/verify-email
- POST /api/v1/auth/login
- POST /api/v1/auth/refresh
- POST /api/v1/auth/forgot-password
- POST /api/v1/auth/reset-password
- GET /api/v1/auth/me
- POST /api/v1/auth/logout

### Facebook Integration
- POST /api/v1/facebook/connect (initiate OAuth)
- GET /api/v1/facebook/callback (OAuth callback)
- GET /api/v1/facebook/pages (list connected pages)
- DELETE /api/v1/facebook/pages/{pageId}/disconnect
- GET /api/v1/facebook/pages/{pageId}/insights

### Content Generation
- POST /api/v1/content/generate (generate AI posts)
- POST /api/v1/content/images/generate (generate images)
- GET /api/v1/content/templates (business type templates)

### Content Scheduling
- POST /api/v1/schedule (create scheduled post)
- GET /api/v1/schedule (list scheduled posts)
- GET /api/v1/schedule/{id}
- PUT /api/v1/schedule/{id}
- DELETE /api/v1/schedule/{id}
- POST /api/v1/schedule/{id}/publish-now
- GET /api/v1/schedule/calendar?month=2025-03

### Billing & Subscriptions
- GET /api/v1/billing/plans (list available plans)
- POST /api/v1/billing/subscribe (create subscription)
- GET /api/v1/billing/subscription (current subscription)
- POST /api/v1/billing/subscription/cancel
- POST /api/v1/billing/subscription/upgrade
- POST /api/v1/billing/subscription/downgrade
- GET /api/v1/billing/usage (current usage)
- GET /api/v1/billing/transactions (payment history)
- POST /api/v1/webhooks/payfast (PayFast webhook)

### Dashboard
- GET /api/v1/dashboard/overview
- GET /api/v1/dashboard/analytics?pageId={id}&period=30d

## FRONTEND PAGES

### Public Pages
1. **Landing Page** (/)
   - Hero section with value proposition
   - Features showcase
   - Pricing table (3 tiers)
   - Testimonials
   - CTA: Start Free Trial

2. **Login** (/login)
3. **Register** (/register)
4. **Forgot Password** (/forgot-password)
5. **Reset Password** (/reset-password/:token)

### Protected Pages (require auth)
6. **Dashboard** (/dashboard)
   - Overview cards: pages, posts this month, engagement
   - Quick actions
   - Recent posts
   - Usage meter

7. **Connect Facebook** (/connect-facebook)
   - OAuth flow
   - Page selection
   - Success confirmation

8. **Content Creator** (/create)
   - Business info form
   - Post type selector
   - Tone selector
   - Language options
   - Generate button
   - Show 3 AI variations
   - Edit before saving
   - Schedule or publish now

9. **Scheduler** (/scheduler)
   - Calendar view (month/week)
   - Drag & drop
   - Post preview
   - Edit/delete actions

10. **Billing** (/billing)
    - Current plan display
    - Usage breakdown
    - Upgrade/downgrade options
    - Payment history
    - Cancel subscription

11. **Settings** (/settings)
    - Profile management
    - Connected pages
    - Notification preferences

## SECURITY REQUIREMENTS

1. **Authentication:**
   - BCrypt password hashing (cost factor 12)
   - JWT with short-lived access tokens (60 min)
   - Refresh tokens (7 days)
   - HTTPS only in production
   - Secure, HttpOnly cookies for refresh tokens

2. **API Security:**
   - Rate limiting: 100 requests/hour per IP
   - CORS: whitelist frontend domains only
   - Input validation on all endpoints
   - SQL injection prevention (parameterized queries)
   - XSS prevention (sanitize user input)
   - CSRF protection

3. **Data Protection:**
   - Encrypt Facebook access tokens at rest (AES-256)
   - Encrypt PayFast credentials
   - No sensitive data in logs
   - GDPR-compliant data handling

4. **Secret Management:**
   - Environment variables for all secrets
   - Never commit secrets to Git
   - Use .NET User Secrets for local dev
   - Separate configs for dev/staging/prod

## EXTERNAL APIS

### Anthropic Claude API
- Endpoint: https://api.anthropic.com/v1/messages
- Model: claude-sonnet-4-5-20250929
- Max tokens: 1000
- Temperature: 0.7
- Estimated cost: R0.06/post, R500/month budget

### Facebook Graph API
- Permissions needed:
  - pages_manage_posts
  - pages_read_engagement
  - pages_manage_metadata
  - business_management
- Endpoints:
  - GET /{page-id}
  - POST /{page-id}/feed
  - GET /{page-id}/insights
  - GET /{page-id}/posts

### PayFast API
- Merchant credentials: ID, Key, Passphrase
- Subscription API for recurring billing
- ITN (Instant Transaction Notification) webhook
- Sandbox for testing

### Unsplash API
- Free tier: 50 requests/hour
- Search photos by keyword
- Download and attribute correctly

## PERFORMANCE REQUIREMENTS

- API response time: < 200ms (p95)
- AI generation: < 5 seconds
- Database queries: < 50ms average
- Frontend page load: < 2 seconds
- Lighthouse score: > 90
- Support 1,000 concurrent users
- 99.5% uptime SLA

## MONITORING & LOGGING

- Serilog for structured logging
- Log levels: Debug (dev only), Info, Warning, Error, Fatal
- Error tracking: Sentry (free tier)
- Uptime monitoring: UptimeRobot
- Performance monitoring: Application Insights or similar
- Log retention: 30 days

## DEPLOYMENT

### Infrastructure
- Docker containers
- PostgreSQL (managed service: R300/month)
- Redis (managed or Docker)
- File storage: Local or Azure Blob/S3
- Domain: .co.za (R150/year)
- SSL: Let's Encrypt (free)

### CI/CD
- GitHub Actions
- Auto-deploy on push to main
- Run tests before deploy
- Blue-green deployment

### Environment Variables
```
# Database
DATABASE_URL=postgresql://...

# JWT
JWT_SECRET_KEY=...
JWT_ISSUER=PageBoostAI
JWT_AUDIENCE=PageBoostAI-Users

# Anthropic
ANTHROPIC_API_KEY=sk-ant-...
ANTHROPIC_MODEL=claude-sonnet-4-5-20250929

# Facebook
FACEBOOK_APP_ID=...
FACEBOOK_APP_SECRET=...
FACEBOOK_REDIRECT_URI=https://yourdomain.com/api/v1/facebook/callback

# PayFast
PAYFAST_MERCHANT_ID=...
PAYFAST_MERCHANT_KEY=...
PAYFAST_PASSPHRASE=...
PAYFAST_MODE=live # or sandbox

# Unsplash
UNSPLASH_ACCESS_KEY=...

# Redis
REDIS_URL=redis://...

# App
FRONTEND_URL=https://yourdomain.com
API_URL=https://api.yourdomain.com
```

## PROJECT STRUCTURE
```
PageBoostAI/
├── src/
│   ├── PageBoostAI.Domain/              # Domain layer (DDD)
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── FacebookPage.cs
│   │   │   ├── ContentSchedule.cs
│   │   │   └── Subscription.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── Money.cs
│   │   │   └── PostContent.cs
│   │   ├── Aggregates/
│   │   ├── Events/
│   │   ├── Exceptions/
│   │   └── Interfaces/
│   ├── PageBoostAI.Application/         # Application layer (Use Cases)
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   └── DTOs/
│   │   ├── Content/
│   │   ├── Facebook/
│   │   ├── Billing/
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   ├── Behaviors/
│   │   │   └── Mappings/
│   │   └── Services/
│   ├── PageBoostAI.Infrastructure/      # Infrastructure layer
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/
│   │   │   └── Repositories/
│   │   ├── Identity/
│   │   ├── ExternalServices/
│   │   │   ├── Anthropic/
│   │   │   ├── Facebook/
│   │   │   ├── PayFast/
│   │   │   └── Unsplash/
│   │   ├── BackgroundJobs/
│   │   └── Caching/
│   ├── PageBoostAI.Api/                 # Presentation layer
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Filters/
│   │   ├── Extensions/
│   │   └── Program.cs
│   └── PageBoostAI.Tests/
│       ├── Unit/
│       ├── Integration/
│       └── E2E/
├── frontend/                            # React app
│   ├── src/
│   │   ├── components/
│   │   │   ├── auth/
│   │   │   ├── dashboard/
│   │   │   ├── content/
│   │   │   ├── scheduler/
│   │   │   └── billing/
│   │   ├── pages/
│   │   ├── hooks/
│   │   ├── services/
│   │   ├── store/ (Redux or Zustand)
│   │   ├── types/
│   │   ├── utils/
│   │   └── App.tsx
│   ├── public/
│   └── package.json
├── docker-compose.yml
├── Dockerfile
└── README.md
```

## DEVELOPMENT WORKFLOW (TDD)

For EVERY feature:
1. **Red:** Write failing test first
2. **Green:** Write minimal code to pass
3. **Refactor:** Improve code quality
4. **Commit:** Small, atomic commits

Example TDD cycle:
```csharp
// 1. RED: Write test first
[Fact]
public async Task GeneratePost_WithValidRequest_ShouldReturnThreePosts()
{
    // Arrange
    var request = new PostGenerationRequest { ... };
    
    // Act
    var result = await _service.GeneratePostsAsync(request);
    
    // Assert
    Assert.Equal(3, result.Posts.Count);
    Assert.All(result.Posts, p => Assert.True(p.Content.Length <= 280));
}

// 2. GREEN: Implement feature
public async Task<PostGenerationResponse> GeneratePostsAsync(...)
{
    // Implementation
}

// 3. REFACTOR: Clean up, extract methods
// 4. COMMIT: "feat: add AI post generation with 3 variations"
```

## TEAM ALLOCATION

Create a large team with specialized roles. Use Claude Code teams feature to parallelize work:

**Team Structure:**
1. **Backend Team Lead** - DDD architecture, core API
2. **Backend Dev 1** - Auth & user management
3. **Backend Dev 2** - Facebook integration
4. **Backend Dev 3** - AI content generation
5. **Backend Dev 4** - Billing & PayFast
6. **Frontend Team Lead** - React architecture, state management
7. **Frontend Dev 1** - Dashboard & analytics
8. **Frontend Dev 2** - Content creator & scheduler
9. **Frontend Dev 3** - Auth pages & billing
10. **DevOps Engineer** - Docker, CI/CD, deployment
11. **QA Engineer** - Test strategy, E2E tests
12. **Prompt Engineer** - AI prompts, content quality

## SUCCESS METRICS

### Technical KPIs
- All tests passing
- 80%+ code coverage
- Zero critical security vulnerabilities
- API response time < 200ms (p95)
- 99.5% uptime

### Business KPIs
- 50 beta users by Week 8
- 70% user activation (create at least 1 post)
- 20% free-to-paid conversion
- R50,000 MRR by Month 6
- <5% monthly churn

## DELIVERABLES

### Week 2
- [ ] DDD domain model complete
- [ ] Database schema finalized
- [ ] Auth system with tests (100% coverage)
- [ ] CI/CD pipeline working

### Week 4
- [ ] Facebook OAuth integration complete
- [ ] AI content generation working
- [ ] Basic frontend (login, dashboard)
- [ ] All features have unit tests

### Week 6
- [ ] PayFast integration complete
- [ ] Scheduler with Hangfire working
- [ ] Full frontend UI complete
- [ ] Integration tests passing

### Week 8
- [ ] Production deployment ready
- [ ] E2E tests passing
- [ ] Security audit complete
- [ ] 50 beta users onboarded

## INSTRUCTIONS FOR CLAUDE CODE TEAMS

1. **Create a large team** (12 members) to parallelize work
2. **Assign roles** based on specialization
3. **Follow TDD strictly** - write tests first
4. **Apply DDD patterns** - bounded contexts, aggregates, value objects
5. **Use clean architecture** - separate concerns
6. **Implement all features** from this spec
7. **Write production-ready code** - error handling, logging, validation
8. **Focus on South African context** - culture, language, payment methods
9. **Optimize for performance** - caching, indexes, async operations
10. **Security first** - encrypt secrets, validate input, prevent injection

## START COMMAND

Begin by:
1. Creating the DDD domain model (entities, value objects, aggregates)
2. Setting up the database schema with migrations
3. Implementing auth system with full test coverage
4. Building the React app structure

**Ready to make money! Let's build PageBoost AI! 🚀**