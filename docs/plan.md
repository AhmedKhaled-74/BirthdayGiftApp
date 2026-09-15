# Age Calculator Implementation Plan

Build a self-service age calculator with Angular 20, ASP.NET Core, SQL Server, and ASP.NET Core Identity. Implement one complete vertical slice at a time.

## Product decisions

- A registered User owns and can edit only their own Birth Date.
- The application has Login, Register, Welcome, and Profile Management pages.
- The API calculates the authoritative Age Summary: completed years, months, days, days until the next Birthday, and whether today is the Birthday.
- The browser sends its IANA time zone with the Welcome request. A Birthday Celebration uses the User's browser-local calendar day.
- February 29 Birth Dates celebrate on February 28 during non-leap years.
- The regular experience is a dark theme. The Birthday Celebration is a gold, luxury theme.
- Version one excludes administration, email confirmation, password reset, and social login.
- Run locally with Docker Compose; cloud deployment is deferred.

## Architecture

- `AgeCalculator.Api`: ASP.NET Core Web API, Identity, Entity Framework Core, and SQL Server.
- `age-calculator-web`: Angular 20 single-page application.
- Angular owns presentation. The API owns authentication, authorization, persistence, and age rules.

Use backend feature folders rather than broad technical layers:

```text
Features/
  Authentication/
    Register/
    Login/
    Logout/
    CurrentUser/
  Profile/
    GetProfile/
    UpdateBirthDate/
  Welcome/
    GetWelcomeSummary/
```

Each feature contains its endpoint, request and response models, validation, persistence work, and tests.

## Phase 1: Foundation

- Create the ASP.NET Core API and Angular 20 applications.
- Add SQL Server to Docker Compose.
- Configure Entity Framework Core, ASP.NET Core Identity, migrations, CORS, and environment settings.
- Add Angular routing and an authenticated HTTP client/interceptor.

**Done when:** the API connects to SQL Server and the Angular app loads locally.

## Phase 2: Authentication

- Add API endpoints for registration, login, logout, and the current authenticated User.
- Add Angular Login and Register pages with validation.
- Protect authenticated routes and add logout.

**Done when:** a new User can register, log in, log out, and cannot visit protected pages while logged out.

## Phase 3: Profile

- Add a nullable Birth Date to the User Profile.
- Add endpoints to get the current User's Profile and update their Birth Date.
- Reject missing and future dates.
- Add the Angular Profile Management page with edit and save states.

**Done when:** a User can add or update their Birth Date and sees the saved value after logging in again.

## Phase 4: Age Summary

- Add `GetWelcomeSummary`, accepting the browser IANA time zone.
- Return years, months, days, days to the next Birthday, and `isBirthday`.
- Direct Users without a Birth Date to Profile Management.
- Add unit tests for ordinary dates, birthday dates, future dates, and February 29 rules.

**Done when:** the Welcome page shows a correct Age Summary for all tested date cases.

## Phase 5: Birthday Celebration UI

- Build the default dark design with CSS variables.
- Apply the gold luxury theme when `isBirthday` is true.
- Add subtle celebratory visuals and copy such as “King of the Day.”
- Ensure the layout works on mobile and desktop.

**Done when:** only a User whose local calendar day is their Birthday sees the gold experience.

## Phase 6: Quality and handoff

- Add loading, empty, and error states.
- Improve form accessibility and responsive layout.
- Add integration tests for authentication and Profile ownership.
- Document local startup in `README.md`.

**Done when:** Docker Compose supports the full flow: register, log in, save a Birth Date, view the age summary, edit the date, and see the birthday theme.

## Phase 7: Design Foundation

Elevate the existing dark + gold luxury identity onto a token-driven design system.
Presentational work only: the API, routes, flows, and v1 feature scope stay frozen,
and the Phase 6 accessibility bar (contrast, focus-visible, `prefers-reduced-motion`,
`role="alert"`/`status` states) is preserved with no new npm dependencies.

- Build the token system in `styles.css`: type scale, spacing, radius, shadows, and motion rules.
- Rebuild buttons, inputs, cards, and shared state blocks on tokens; remove hardcoded
  colors and spacing from component styles.
- Make no layout or page-structure changes; add no new pages.
- Follow the `frontend-design` skill for design decisions during implementation.

**Done when:** every page renders from tokens, `ng build` budgets are green, the keyboard
and screen-reader pass is unchanged, and the layout is verified on mobile and desktop.

## Phase 8: Signature Moments

Spend the uniqueness budget on two demoable moments on the Welcome page, keeping the
`Birthday Celebration` identity (`CONTEXT.md` glossary, gold luxury concept,
“King of the Day” copy) exactly as is.

- Add an animated age reveal: Years, Months, and Days count up on load.
- Replace the static theme swap with a choreographed dark-to-gold Birthday Celebration
  transformation when `isBirthday` is true.
- Keep all motion subtle and honor `prefers-reduced-motion`; hold contrast ratios in both themes.
- Follow the `frontend-design` skill for design decisions during implementation.

**Done when:** both signature moments are demoable on demand, all motion respects
`prefers-reduced-motion`, contrast holds in both themes, and only a User whose local
calendar day is their Birthday sees the gold transformation.

## Phase 9: Free hosting (Render + Neon)

Host the app publicly for $0/month with no credit card: one Render Free Web Service
serving the API plus the Angular static build, backed by Neon Free Postgres.
Single provider everywhere: local Docker and hosted both run Postgres; SQL Server
is removed.

- Switch EF Core provider from `Microsoft.EntityFrameworkCore.SqlServer` to
  `Npgsql.EntityFrameworkCore.PostgreSQL` (match EF `10.0.12`); replace the single
  `UseSqlServer` call in `Program.cs` with `UseNpgsql` using the pooled Neon
  connection string from `ConnectionStrings__DefaultConnection`.
- Delete and regenerate the EF migrations under Npgsql (`datetime2` → `timestamptz`,
  `nvarchar` → `varchar`/`text`, T-SQL filters → Postgres quoting); keep
  `MigrateAsync()` on startup.
- Replace `docker-compose.yml` `mssql/server:2022` service with `postgres:16`
  (database `AgeCalculator`, persisted volume); update `appsettings.Development.json`
  and `README.md` local startup accordingly.
- Serve the Angular production build from the API: `AgeCalculator.Api.csproj`
  target runs `npm ci + ng build` on `dotnet publish` into `wwwroot`;
  `Program.cs` adds `UseStaticFiles + MapFallbackToFile("index.html")`;
  CORS stays for `http://localhost:4200` dev only. No runtime Node in prod.
- Add multi-stage Ubuntu-based `Dockerfile` (`.NET 10` SDK → `aspnet:10.0` Noble;
  no Debian tags exist) and a `render.yaml` blueprint: Docker web service
  (512MB/0.1 CPU, 750h/mo, sleeps after 15min idle), `ConnectionStrings__DefaultConnection`
  and `ASPNETCORE_URLS` from Render env vars, `/healthz` health check.
- Provision Neon Free project (0.5GB, 100 CU-h/mo, scales to zero after 5min idle,
  pooled + direct connection strings); run one manual `dotnet ef database update`
  or let the first boot migrate, then smoke-test register → Profile → Welcome.

Explicitly out of scope: Render Postgres (deleted after 30d), custom domain,
always-on, backups/PITR, email confirmation/password reset (still v1-excluded).

**Done when:** a cold Render URL serves the Angular app and API from one origin,
a new User can register, save a Birth Date, see the Age Summary and Birthday
Celebration, and data survives service sleep (Neon, not ephemeral disk).
