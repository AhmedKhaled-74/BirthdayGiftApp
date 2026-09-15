# Age Calculator

A self-service age calculator with an Angular 20 single-page app, an ASP.NET Core Web API
backed by Postgres and ASP.NET Core Identity. A registered User owns exactly their own
Birth Date; the API calculates the authoritative Age Summary (completed years, months, days,
days until the next Birthday, and whether today is the Birthday). On the User's Birthday —
by the browser-local calendar day — the Welcome page switches to a gold Birthday Celebration.
In production the API also serves the Angular static build, so one service hosts everything.

Version one excludes administration, email confirmation, password reset, and social login.

## Prerequisites

- .NET 10 SDK
- Node.js 20+ with npm
- Docker Desktop (for local Postgres)

## Local startup

1. Start Postgres:

   ```powershell
   docker compose up -d
   ```

   This starts Postgres 16 on `localhost:5432` (database `AgeCalculator`,
   user `postgres`, password `AgeCalc!2026Dev`). The API applies EF Core migrations on startup.

2. Start the API (new terminal, Development environment):

   ```powershell
   dotnet run --project AgeCalculator.Api --launch-profile http
   ```

   The API listens on `http://localhost:5039`.

3. Start the web app (new terminal):

   ```powershell
   npm --prefix age-calculator-web install
   npm --prefix age-calculator-web start
   ```

   The app is served at `http://localhost:4200`.

## Full flow

With Postgres running via Docker Compose:

1. Open `http://localhost:4200` and create an account on the Register page
   (passwords need at least 8 characters).
2. Log in on the Login page.
3. Without a Birth Date, the Welcome page directs you to Profile Management.
4. On the Profile page, add a Birth Date (future dates are rejected) and save.
5. Return to the Welcome page (`/`) to see the Age Summary: years, months, days,
   and days until the next Birthday.
6. Edit the Birth Date on the Profile page and confirm the summary updates.
7. To see the Birthday Celebration, set the Birth Date to today's date
   (a February 29 Birth Date celebrates on February 28 in non-leap years):
   the page switches to the gold "King of the Day" theme. Only a User whose
   local calendar day is their Birthday sees it.

## Tests

Backend (xUnit: 37 age-calculation unit tests plus integration tests for
authentication, Profile ownership, and the Welcome summary; integration tests
run against SQLite in-memory, no Docker needed):

```powershell
dotnet test
```

Frontend production build:

```powershell
npm --prefix age-calculator-web run build
```

## Project structure

```text
AgeCalculator.Api/            ASP.NET Core API, Identity, EF Core, Postgres
  Features/
    Authentication/           Register, Login, Logout, CurrentUser
    Profile/                  GetProfile, UpdateBirthDate
    Welcome/                  GetWelcomeSummary, AgeCalculation
AgeCalculator.Api.Tests/      Unit + integration tests
age-calculator-web/           Angular 20 SPA (Welcome, Login, Register, Profile)
docs/plan.md                  Phased implementation plan
docker-compose.yml            Local Postgres
Dockerfile                    Production image (Angular build + API, serves wwwroot)
render.yaml                   Render Free web service blueprint (/healthz check)
```

## Free hosting (Render + Neon, $0, no credit card)

The production deployment is one Render Free Web Service (the API serving the
Angular `wwwroot` build) backed by a Neon Free Postgres database.

1. Create a free [Neon](https://neon.com) project and copy its **pooled**
   connection string.
2. Push this repo to GitHub. In [Render](https://render.com), New → Blueprint,
   select the repo (`render.yaml` defines the Docker web service, plan `free`,
   health check `/healthz`).
3. Set the `ConnectionStrings__DefaultConnection` env var to the Neon pooled
   connection string and deploy. The API applies EF Core migrations on startup.

Notes:

- The Render Free service sleeps after 15 minutes without traffic; the first
  request after sleep takes ~1 minute to wake. Neon scales to zero after
  5 minutes idle and wakes in milliseconds.
- The production Angular build uses a same-origin API (`environment.ts`
  `apiUrl` is empty), so no CORS configuration is needed in production;
  `AllowedOrigins` (`http://localhost:4200`) applies to local development only.

## Troubleshooting

- `AgeCalculator.Api.exe` locked during build/test: a previous `dotnet run` is still
  holding it. Stop that process (or close the terminal) and retry.
- API cannot reach Postgres: confirm `docker compose ps` shows a healthy
  `age-calculator-db` container and port 5432 is free.
- Web app cannot reach the API: the API must run on `http://localhost:5039`
  (see `age-calculator-web/src/environments/`), and the browser must run the app
  from `http://localhost:4200` (allowed CORS origin in development).
