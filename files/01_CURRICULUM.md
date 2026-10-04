# CURRICULUM — CareRoute: a hospital platform with .NET 10, Clean Architecture, Blazor & Azure

**34 lessons · ~2 hours each · 4 lessons a day · 9 days · 1 project that grows every lesson**
Stack: .NET 10 (LTS) · C# 14 · ASP.NET Core 10 (controllers) · EF Core 10 · SQL Server · **Clean Architecture** · MediatR & CQRS · FluentValidation · Blazor WebAssembly · MongoDB · RabbitMQ
Testing: xUnit · **FluentAssertions 7.x** · **NSubstitute** · **Bogus** · **Testcontainers** · bUnit
Observability: **Serilog** · structured logging · **correlation IDs** · **health checks** · OpenTelemetry · **Application Insights**
Azure: **App Service** · **Key Vault** · **Application Insights** · **Service Bus** · **Azure Functions & Durable Functions** · **Logic Apps** · **API Management**

---

## 0. How this course works

### Beginner-friendly, with depth
Every 🟢 core concept goes through the **depth stack**, automatically:

| Layer | What it answers | Marker |
|---|---|---|
| 1. Orientation | What is it? Where does it fit? | 🧭 |
| 2. Why | Which problem does it solve? (analogy) | 💡 |
| 3. Definition | Plain words, then technical | 🟦 |
| 4. The basics | How do I use it? | 📘 |
| 5. Under the hood | What happens at runtime? | ⚙️ |
| 6. Trade-offs | Gain, cost, when not, alternative | ⚖️ |
| 7. Failure modes | Symptom → cause → notice → fix | 💣 |

Then 🟩 example · 🟥 mistake · 🟪 say it out loud · 🟨 in short.

**Priority levels:** 🟢 **Core** (full stack, max 3–4 per lesson) · 🔵 **Important** (compact: definition, one trade-off, one failure mode) · 🟣 **Advanced** (one-line pointers — ask with `DEEPER`).

### Clean Architecture is the backbone
CareRoute has four layers from L03 on, one project each, with dependencies pointing **inward**:

```
        ┌──────────────────────────────────────────┐
        │ Api  (controllers, middleware, startup)  │
        │  ┌────────────────────────────────────┐  │
        │  │ Infrastructure (EF Core, Mongo,    │  │
        │  │ RabbitMQ / Service Bus, Serilog…)  │  │
        │  │  ┌──────────────────────────────┐  │  │
        │  │  │ Application (use cases,      │  │  │
        │  │  │ MediatR, validators, ports)  │  │  │
        │  │  │  ┌────────────────────────┐  │  │  │
        │  │  │  │ Domain (entities,      │  │  │  │
        │  │  │  │ value objects, rules)  │  │  │  │
        │  │  │  └────────────────────────┘  │  │  │
        │  │  └──────────────────────────────┘  │  │
        │  └────────────────────────────────────┘  │
        └──────────────────────────────────────────┘
        Every arrow points inward. Domain references nothing.
```

Each new file says which layer it lives in (🏛️). The pay-off arrives twice in Azure: **RabbitMQ → Service Bus (L30)** and **hosted process manager → Durable Functions (L31)** swap Infrastructure without touching Domain or Application.

### DDD — where it matters, not as separate lessons
There are no DDD lessons. A short 🧩 **DDD note** appears where a DDD idea explains a design choice (value object, aggregate, invariant, domain vs integration event, bounded context, anti-corruption layer…). Each lesson lists its notes. Want more? `DDD <term>`. Appendix E is the reference.

### Tests at the end of every lesson (no TDD)
Part 2 builds the feature and you check it by running it. **Part 3 — Test** then writes every test on the lesson's 🧪 list: one per business rule, every boundary, a regression test for every bug. Once per lesson you 💥 break a rule on purpose and watch its test go red — that's how you know the tests are honest.

### Every lesson has five parts (~2 hours)

| Part | Time | What you do |
|---|---|---|
| 1. Understand | 35 min | Concepts, 🎬 animations, 🧩/🏛️ notes. Explain each ⚙️ out loud. |
| 2. Build | 45 min | Add the feature in Visual Studio (or the Azure Portal). Type the code yourself. |
| 3. Test | 20 min | Write and run the lesson's tests; one 💥 break-it. |
| 4. Practice | 12 min | Quiz + explain-it questions, **out loud**. |
| 5. Review | 8 min | Summary, glossary, commit, update `02_CONTINUITY.md`. |

**Between lessons:** a 10-minute break away from the screen. **Lunch** after the second lesson of the day.

### A note on expectations
Four lessons a day is intense. If a day overruns, send `BEHIND` for a lighter version of the next lesson, or move the last lesson to the next morning — depth beats speed. Checkpoints (L16, L27) are built-in catch-up time.

---

## 1. The project: CareRoute

### 1.1 The story (fictional)
CareRoute is a **hospital platform** for the journey from a family doctor to a hospital appointment. **GPs** register their **patients** and **refer** them to a **hospital department** (Cardiology, Orthopaedics, Dermatology). The GP fills in the department's **intake form**. **Specialists** **triage** the referral and accept or reject it. An accepted referral becomes an **appointment** within a deadline that depends on urgency. Everyone is notified, and every access to patient data is audited. **All data is fictional** (Bogus-generated or hand-written).

### 1.2 Bounded contexts (introduced as a 🧩 note in L03)

| Context | What it owns | Where it lives |
|---|---|---|
| **Patient Registry** | Patients, national numbers, the assigned GP | `CareRoute.Api` (modular monolith), SQL Server |
| **Referrals** (core) | Referral workflow: draft → submit → triage → accept/reject | `CareRoute.Api`, SQL Server |
| **Scheduling** (core) | Doctors' schedules, appointments, booking rules | `CareRoute.Api`, SQL Server |
| **Hospital Directory** | Hospitals, departments, doctors | `CareRoute.Api`, SQL Server |
| **Clinical Intake** | Department-specific intake forms | `CareRoute.Intake.Api` (own service), MongoDB |
| **Notifications** | Telling people something happened | `CareRoute.Notifications.Worker` → Logic App (L32) |
| **Identity & Access** | Who you are, what you may do | Dev token issuer → identity provider (concept) |

Contexts are **folders inside each layer** (`Domain/Referrals`, `Application/Referrals`…), not separate projects — except Intake, which is a service.

### 1.3 How the project grows

| Lesson | What we add |
|---|---|
| **Part A — Foundations** | |
| L01 | Solution + first API with a `PatientsController` (hard-coded list) |
| L02 | Pipeline, DI, configuration, model validation, ProblemDetails, `ILogger` |
| L03 | Clean Architecture skeleton (4 projects) + test projects + `CareRoute.Testing` (Bogus) |
| **Part B — Core logic** | |
| L04 | Domain I: `Patient`, `NationalNumber`, `PersonName`, typed IDs |
| L05 | Domain II: `Referral` workflow, `DoctorSchedule` (no double booking), domain events, `TimeProvider` |
| L06 | Application layer: MediatR use cases, FluentValidation, behaviors, thin controllers |
| **Part C — Data** | |
| L07 | SQL Server basics: schema per context, keys, constraints |
| L08 | Indexes, plans, transactions, concurrency |
| L09 | EF Core 10 in Infrastructure: mapping the domain, repositories, migrations |
| L10 | EF Core in practice + integration tests with Testcontainers and Bogus |
| L11 | CQRS and domain-event dispatch: timeline, booking requests |
| L12 | **Observability:** Serilog, structured logs, correlation IDs, health checks, OpenTelemetry |
| **Part D — UI & security** | |
| L13 | Blazor WebAssembly portal |
| L14 | Blazor forms, routing, state, errors, booking screen |
| L15 | Security I: JWT, policies, resource-based authorisation, BOLA |
| L16 | ★ Checkpoint 1 |
| **Part E — Distributed system** | |
| L17 | Docker + MongoDB: the Clinical Intake service, queries, indexes, aggregation |
| L18 | RabbitMQ: integration events, Notifications worker, `IEventBus` port |
| L19 | Reliability: acks, retries, dead letters, outbox, inbox, idempotency keys |
| L20 | Distributed tracing: one trace and one correlation ID across services |
| L21 | Process manager: deadlines and escalation (behind a port) |
| **Part F — Quality & production** | |
| L22 | Testing strategy: pyramid, bUnit, architecture tests, CI |
| L23 | Production readiness: secrets, Docker images, liveness/readiness probes |
| L24 | Security II: audit, log redaction, headers, OAuth flows |
| L25 | Performance & caching |
| L26 | Resilience & service boundaries (anti-corruption layer) |
| L27 | ★ Checkpoint 2 |
| **Part G — Azure** | |
| L28 | Azure basics, App Service, Key Vault, managed identity, deploy from GitHub Actions |
| L29 | Application Insights: logs, traces, metrics, KQL, alerts, availability tests |
| L30 | Azure Service Bus replaces RabbitMQ |
| L31 | Azure Functions & Durable Functions: the booking-deadline orchestration |
| L32 | Logic Apps: the notification workflow |
| L33 | API Management: one front door, policies, products |
| L34 | ★ Final checkpoint + capstone |

### 1.4 Where we end up (Azure)

```
 Browser ─► Azure Static Web App / App Service (CareRoute.Portal, Blazor)
                │ HTTPS + JWT
                ▼
        ┌──────────────── API Management ────────────────┐
        │ policies: JWT check · rate limit · versioning  │
        └──────┬──────────────────────────────┬──────────┘
               ▼                              ▼
  App Service: CareRoute.Api        App Service: CareRoute.Intake.Api
  (Clean Architecture monolith)     (MongoDB / Cosmos DB for MongoDB — optional)
  Azure SQL Database                         │
       │ outbox → IEventBus                  │
       ▼                                     ▼
  ┌──────────────── Azure Service Bus (topic careroute-events) ─────────────┐
  └──────┬───────────────────────────────┬──────────────────────────────────┘
         ▼                               ▼
  Azure Functions                   Logic App
  (Durable: booking deadline)       (notifications: email / Teams)
  Key Vault (secrets, managed identity) · Application Insights (everything)
```

Locally, the same system runs with LocalDB/SQL Server, MongoDB and RabbitMQ in Docker Desktop.

### 1.5 Final solution structure

```
CareRoute (solution, .slnx)
├─ src
│  ├─ CareRoute.Domain                 # entities, value objects, domain events, repository interfaces (L03–L05)
│  ├─ CareRoute.Application            # use cases, MediatR handlers, validators, ports (L03, L06)
│  ├─ CareRoute.Infrastructure         # EF Core, repositories, outbox, messaging, Serilog setup, ACL (L03, L09)
│  ├─ CareRoute.Api                    # controllers, middleware, composition root (L01)
│  ├─ CareRoute.Contracts              # DTOs and integration-event contracts (L13)
│  ├─ CareRoute.Portal                 # Blazor WebAssembly (L13)
│  ├─ CareRoute.Intake.Api             # Clinical Intake service, MongoDB (L17)
│  ├─ CareRoute.Notifications.Worker   # RabbitMQ consumer (L18)
│  └─ CareRoute.Functions              # Azure Functions + Durable Functions (L31)
├─ tests
│  ├─ CareRoute.Testing                # shared Bogus fakers, builders, Testcontainers fixtures (L03, L10)
│  ├─ CareRoute.Domain.Tests           # unit tests (L03)
│  ├─ CareRoute.Application.Tests      # handler + validator tests with NSubstitute (L06)
│  ├─ CareRoute.Api.IntegrationTests   # WebApplicationFactory + Testcontainers (L10)
│  ├─ CareRoute.Portal.Tests           # bUnit (L22)
│  └─ CareRoute.ArchitectureTests      # dependency rule + context boundaries (L22)
├─ tools
│  ├─ CareRoute.NationalRegistry.Fake  # fake external system behind an ACL (L26)
│  └─ CareRoute.LoadTest               # small load generator (L25)
├─ .github/workflows                   # CI (L22), deploy to Azure (L28)
└─ docs                                # ubiquitous language, context map, ADRs, runbooks, SQL scripts
```

### 1.6 Business rules (fictional, kept simple)

**Patient Registry**
- A patient has a **Belgian-style national register number**: 11 digits = birth date (YYMMDD) + 3-digit serial + 2-digit check (97 − first 9 digits mod 97; for people born in 2000 or later, prefix `2` before calculating). Unique.
- Every patient has exactly one assigned GP.

**Referrals**
- Workflow: `Draft → Submitted → InTriage → Accepted | Rejected`; Draft or Submitted can be `Withdrawn`.
- Urgency `Routine` or `Urgent`. Urgent must be triaged within **48 hours**, otherwise escalated.
- To submit: clinical reason ≥ 20 characters, the department's **intake form complete**, draft not older than **30 days**.
- Only a **specialist of the target department** can triage, accept or reject; **rejecting needs a reason**.

**Scheduling**
- Appointments last **20 or 40 minutes**, start on a 20-minute boundary; **a doctor's appointments never overlap**.
- No booking in the past. Cancelling < **24 hours** before the start = late cancellation.
- An accepted referral must be booked within **7 days (urgent)** or **60 days (routine)**, otherwise the department is alerted.

**Access**
- A GP sees only their own patients and referrals. A specialist sees only their department's referrals and their own schedule. Every read of patient data is audited.

---

## 2. Schedule — 4 lessons a day

| Day | 09:00–11:00 | 11:15–13:15 | 14:15–16:15 | 16:30–18:30 |
|---|---|---|---|---|
| **1** | L01 First API | L02 ASP.NET Core fundamentals | L03 Clean Architecture + test toolkit | L04 Domain I: patients |
| **2** | L05 Domain II: referrals & scheduling | L06 Application layer | L07 SQL basics | L08 SQL for developers |
| **3** | L09 EF Core 10 | L10 EF Core + Testcontainers | L11 CQRS & events | L12 Observability |
| **4** | L13 Blazor basics | L14 Blazor forms & state | L15 Security I | L16 ★ Checkpoint 1 |
| **5** | L17 Docker & MongoDB | L18 RabbitMQ | L19 Reliability & outbox | L20 Distributed tracing |
| **6** | L21 Process manager | L22 Testing strategy & CI | L23 Production readiness | L24 Security II |
| **7** | L25 Performance | L26 Resilience & boundaries | L27 ★ Checkpoint 2 | L28 Azure App Service & Key Vault |
| **8** | L29 Application Insights | L30 Service Bus | L31 Functions & Durable | L32 Logic Apps |
| **9** | L33 API Management | L34 ★ Final checkpoint + capstone (double slot) | — | — |

**Evenings:** 20 minutes at most — today's handouts and weak spots. Nothing new.

### Why this order
- **Clean Architecture on day 1.** The four projects exist from L03, so every later file has an obvious home.
- **Domain before database.** Rules are modelled and tested in plain C# (L04–L06) before persistence (L07–L10). The model decides the tables.
- **Testcontainers early (L10).** Integration tests use real SQL Server from the first time we persist — no fake databases.
- **Observability before the UI (L12).** Every later lesson — Blazor errors, security, messaging, Azure — is debugged with structured logs and correlation IDs.
- **Ports before Azure.** `IEventBus` (L18) and the process-manager port (L21) are designed so Azure can replace RabbitMQ and the hosted timer without changing the core.
- **Local first, cloud last.** Everything runs on your machine before it costs money in Azure.

---

## 3. Before Lesson 1 — setup with installers (no terminal)

1. **Visual Studio Installer** → **Modify** Visual Studio 2026:
   - Workloads: **ASP.NET and web development** (includes the .NET 10 SDK) and **Azure development** (Functions tools, Azure publishing).
   - Individual components: **.NET 10.0 Runtime (LTS)**, **SQL Server Express LocalDB**, **Git for Windows**, **Container development tools** (names vary slightly).
2. If **.NET 10.0** isn't offered in L01, install the **.NET 10 SDK** from Microsoft's download page and restart Visual Studio.
3. **GitHub account** (free).
4. **Docker Desktop** for Windows (enables WSL 2). **Needed from L10** (Testcontainers) — install now.
5. **MongoDB Compass** — needed from L17.
6. Optional: **SQL Server Management Studio**.
7. **Azure account** — needed from **L28**. A free account or a pay-as-you-go subscription; you'll set a budget alert in L28 (verify current free-account offer).
8. In **Claude**: switch on **Web search**, **Artifacts** and **Code execution and file creation**.

---

## 4. What you'll be able to explain

| Area | The basics | Under the hood | Trade-offs & failure modes |
|---|---|---|---|
| ASP.NET Core 10 | Pipeline, controllers, DI, config, validation, errors | Scopes per request, the controller request flow | Captive dependencies, over-posting, sync-over-async |
| Clean Architecture | Four layers, dependency rule, ports & adapters | Project references as enforcement, composition root | Ceremony vs swappability; leaking EF into the domain |
| Testing | xUnit, FluentAssertions, NSubstitute, Bogus, Testcontainers, bUnit | Test discovery, dynamic proxies, container lifecycle | Over-mocking, flaky tests, tests that mirror code |
| Domain modelling (DDD notes) | Entities, value objects, aggregates, domain events | Consistency boundaries, event collection & dispatch | Aggregate size, anemic models, leaking models |
| SQL Server & EF Core | Tables, keys, indexes, DbContext, migrations | B-trees, plans, change tracking, LINQ → SQL | N+1, deadlocks, lost updates, migration drift |
| MediatR & CQRS | Handlers, behaviors, commands vs queries | Behavior nesting, dispatch timing | Indirection, lost events, stale reads |
| Observability | Serilog, structured logs, correlation IDs, health checks, traces | Enrichers, `Activity` and W3C trace context, probes | Log cost & PII, missing correlation, probes that kill apps |
| Blazor WebAssembly | Components, forms, routing, state | WebAssembly runtime, render tree | Download size, CORS, UI-only security |
| MongoDB | Documents, indexes, aggregation | Discriminators, write concern | Unbounded docs, lost writes on failover |
| Messaging | Exchanges/topics, acks, retries, DLQ, outbox/inbox | Publisher confirms, at-least-once, peek-lock | Duplicates, poison messages, dual writes |
| Security | AuthN vs AuthZ, JWT, resource checks, audit | Token validation, claims, policies | BOLA, token theft, personal data in logs |
| Azure | App Service, Key Vault, App Insights, Service Bus, Functions, Logic Apps, APIM | Managed identity, slots, durable replay, APIM policy pipeline | Cost, cold starts, vendor lock-in, misconfigured identity |

---

# PART A — FOUNDATIONS

## L01 — Start the project: Visual Studio 2026, .NET 10 & your first API
**Day 1 · Slot 1 · Project step P01**
**📍 Start:** nothing. **🏁 End:** a `CareRoute` solution with a running API whose `PatientsController` returns fictional patients, tested from a `.http` file, on GitHub.

### You'll be able to
- Explain .NET, C#, ASP.NET Core, SDK vs runtime, and why LTS.
- Create, run and debug a Web API; write a controller with two actions.
- Send requests from a `.http` file; commit and push with Git.

### Concepts
🟢 **Core**
- .NET vs C# vs ASP.NET Core; SDK vs runtime; `net10.0`; **LTS** (supported until November 2028). ⚙️ C# → IL in a DLL → JIT → the host starts **Kestrel**.
- Solution (`.slnx`) vs project; `Program.cs`, `appsettings.json`, `launchSettings.json`.
- **HTTP**: method, URL, status code, headers, JSON body.
- **Controllers & actions** (`[Route]`, `[HttpGet]`, `ControllerBase`). ⚙️ `AddControllers()` discovers controllers, `MapControllers()` creates endpoints; **a new controller instance per request**.

🔵 **Important**
- C# you need today: `class` vs `record`, properties, `List<T>`, `var`, attributes, `: ControllerBase`, `=>`, string interpolation.
- F5 vs Ctrl+F5, breakpoints, stepping; `.http` file and **Endpoints Explorer**; OpenAPI at `/openapi/v1.json` (no Swagger UI since .NET 9).
- Controllers vs minimal APIs — why CareRoute uses controllers.

🟣 **Going further:** C# 14 highlights · `dotnet run app.cs` `[.NET 10]` · Scalar UI.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Controllers (structure, filters, conventions) vs minimal APIs (less ceremony). ⚖️ LTS vs STS.
- 💣 Controller not found (missing `MapControllers`, non-public class) → 404 with empty body.
- 💣 Port in use / untrusted dev certificate. 💣 Different SDKs on different machines (`global.json`, L22).

### 🎬 Visuals
- 🎬 Pressing F5: build → Kestrel listens → request → routing → controller created → action → JSON.
- 🎬 One HTTP round trip with method, URL, status and body highlighted.

### 🛠️ Build
1. **File > New > Project > Blank Solution** → `CareRoute` (`.slnx`).
2. **Add > New Project > ASP.NET Core Web API** → `CareRoute.Api`: .NET 10.0 (LTS), Authentication None, HTTPS ✓, container ✗, OpenAPI ✓, top-level statements ✓, **Use controllers ✓**.
3. F5 → send the sample request from `CareRoute.Api.http`; breakpoint in the sample action; step through.
4. Delete the sample; **Add > Controller > API Controller - Empty** → `PatientsController`; a `Patient` record; `GET api/patients` (3 fictional patients) and `GET api/patients/{id}` (404 when missing).
5. **Git > Create Git Repository** → push to GitHub.

### 🧪 Tests (Part 3)
No test project yet (it arrives in L03). Part 3 today is a **manual test plan** in `CareRoute.Api.http`: list → 200 with 3 items; existing id → 200; unknown id → 404; malformed id → 404/400 (explain which and why). 💥 Remove `MapControllers()` → every request 404 → restore.

### 🏁 Checkpoint
- `/api/patients` returns 3 patients; `/api/patients/{id}` returns one or 404. Commit `P01: CareRoute API with PatientsController`.

### 🎤 Explain-it questions
1. What is .NET 10, and why does LTS matter?
2. SDK vs runtime?
3. What happens, step by step, when a request reaches your controller?
4. Class vs record?
5. Why controllers instead of minimal APIs, and what do we give up?

---

## L02 — ASP.NET Core fundamentals: pipeline, DI, configuration, validation, errors
**Day 1 · Slot 2 · Project step P02**
**📍 Start:** a controller with a hard-coded list. **🏁 End:** an injected patient store, `POST` with automatic validation, options with `ValidateOnStart`, ProblemDetails errors, `ILogger`, and a first `/health` endpoint.

### Concepts
🟢 **Core**
- **Middleware pipeline** (💡 airport security checkpoints, in order) — order is behaviour.
- **Controller request flow** ⚙️: routing → controller created (DI) → **model binding** → **validation** → filters → action → result.
- **Dependency injection & lifetimes** (singleton, scoped, transient). ⚙️ One scope per request, disposed at the end.
- **Model validation**: DataAnnotations on request DTOs; `[ApiController]` → automatic **400 ValidationProblemDetails**.

🔵 **Important**
- Configuration layers and the **options pattern** with `ValidateOnStart`.
- `ILogger` with message templates — IDs, never names or national numbers (Serilog replaces the provider in L12).
- `IExceptionHandler` + `AddProblemDetails`; status codes 200/201/400/404/409/500; `CreatedAtAction`.
- `async`/`await` (💡 a waiter taking other orders) and `CancellationToken`. ⚙️ No thread waits during I/O.
- Filters vs middleware; over-posting and why we bind DTOs.
- `app.MapHealthChecks("/health")` — one line today, the full story in L12.

🟣 **Going further:** keyed services · `TimeProvider` (L05) · CORS ordering (L13).

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Singleton store (shared, must be thread-safe) vs scoped. ⚖️ DataAnnotations vs FluentValidation (L06) vs domain rules (L04).
- 💣 Captive dependency (singleton holding scoped). 💣 Middleware in the wrong order. 💣 Over-posting. 💣 Sync-over-async (`.Result`) → thread-pool starvation. 💣 A shared `List<T>` corrupted by concurrent requests.

### 🎬 Visuals
- 🎬 A request through the pipeline, with the exception handler catching on the way back.
- 🎬 Inside a controller request, with validation short-circuiting to 400.
- 🎬 Singleton vs scoped vs transient across two requests.

### 🛠️ Build
1. `Patients` folder: `Patient`, `RegisterPatientRequest` (DataAnnotations), `PatientResponse`.
2. `IPatientStore` + thread-safe `InMemoryPatientStore` (singleton) injected into the controller.
3. `POST api/patients` → 201 `CreatedAtAction`; invalid → automatic 400.
4. `Referrals` options section (`MaxDraftAgeDays: 30`, `UrgentTriageHours: 48`) with `ValidateOnStart`.
5. `ILogger` logging the patient ID on registration.
6. `AddProblemDetails` + an `IExceptionHandler`; `/health`.

### 🧪 Tests (Part 3)
Still manual (unit tests start in L03): extend the `.http` file — valid POST → 201 + `Location`; missing name → 400 with field errors; extra `id` in body ignored (over-posting); thrown exception → 500 ProblemDetails without stack trace; `/health` → Healthy; bad config value → app fails at startup. 💥 Change the store to a plain `List<T>` singleton and register concurrently with a loop in the `.http` file (or explain the race) → restore.

### 🏁 Checkpoint
- Commit `P02: DI, configuration, validation, ProblemDetails, health`.

### 🎤 Explain-it questions
1. What is middleware, and why does order matter?
2. What happens between routing and your action? What does `[ApiController]` do?
3. Singleton vs scoped vs transient — and what is a captive dependency?
4. What is over-posting, and how do DTOs prevent it?
5. Does `await` create a new thread?

---

## L03 — Clean Architecture & the testing toolkit
**Day 1 · Slot 3 · Project step P03**
**📍 Start:** one API project. **🏁 End:** the four Clean Architecture projects with correct references, the patient store moved behind an interface in the right layer, a `CareRoute.Testing` project with Bogus fakers, and the first unit tests written with xUnit + FluentAssertions + NSubstitute.

### Concepts
🟢 **Core**
- **Clean Architecture** (💡 an onion: the core knows nothing about the layers around it). Domain · Application · Infrastructure · Api; **the dependency rule**. ⚙️ Enforced by project references — the compiler refuses an outward reference.
- **Ports & adapters:** Application defines an interface (port), Infrastructure implements it (adapter), Api wires them in the **composition root** (`Program.cs`).
- **Unit testing with xUnit:** `[Fact]`, `[Theory]` + `[InlineData]`, Arrange–Act–Assert, Test Explorer. ⚙️ The runner finds public methods with test attributes by reflection.
- **The toolkit:** **FluentAssertions 7.x** (`x.Should().Be(…)`, readable failure messages), **NSubstitute** (`Substitute.For<IPort>()`; ⚙️ a runtime-generated class that records calls), **Bogus** (`Faker<T>` for realistic fictional data; ⚙️ a seed makes data repeatable).

🔵 **Important**
- What makes a good test (fast, independent, repeatable, named after the rule).
- Test doubles: dummy, stub, mock, fake — **substitute ports, never the domain**.
- Licences: FluentAssertions v8+ is commercial → pin **7.x** (AwesomeAssertions = Apache fork); Bogus, NSubstitute, xUnit are open source (verify).
- TDD exists as an alternative workflow; this course writes tests at the end of each lesson.

### 🧩 DDD notes
- **Bounded contexts** (5 lines): CareRoute's contexts from §1.2 become **folders inside each layer** (`Domain/PatientRegistry`, `Domain/Referrals`…). "Patient" can mean different things in different contexts — share IDs, not classes.
- **Ubiquitous language**: create `docs/ubiquitous-language.md`; class and method names come from it.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Clean Architecture (testable core, swappable infrastructure) vs ceremony for simple CRUD. ⚖️ Layers-as-projects (compiler-enforced) vs folders (lighter, leaks silently).
- ⚖️ Mocks isolate but couple tests to implementation. ⚖️ Random Bogus data finds surprises vs flaky tests → **fixed seed**.
- 💣 Domain referencing EF Core "just for an attribute" → the core now depends on a database library.
- 💣 Over-mocking: tests that pass while the real wiring is broken. 💣 Tests that assert nothing useful.

### 🎬 Visuals
- 🎬 The onion: rings appear one by one; an arrow from Api to Domain is allowed, Domain → Infrastructure turns red.
- Cards: dummy, stub, spy, mock, fake.

### 🛠️ Build
1. **Add > New Project > Class Library** ×3: `CareRoute.Domain`, `CareRoute.Application`, `CareRoute.Infrastructure` (.NET 10). Solution folders `src` and `tests`.
2. References: Application → Domain; Infrastructure → Application; Api → Application + Infrastructure.
3. Move `IPatientStore` to Application (port); `InMemoryPatientStore` to Infrastructure (adapter); a `DependencyInjection.cs` extension per layer (`AddApplication()`, `AddInfrastructure()`) called from `Program.cs`.
4. **Add > New Project > xUnit Test Project** → `CareRoute.Domain.Tests`, `CareRoute.Application.Tests`; class library `CareRoute.Testing`.
5. NuGet: `FluentAssertions` (**7.x — pick the latest 7.x in the Version dropdown**), `NSubstitute`, `Bogus`.
6. `CareRoute.Testing/Fakers/PatientFaker.cs` (Bogus, fixed seed, fictional Belgian names).
7. `docs/ubiquitous-language.md`, `docs/adr/ADR-001-clean-architecture.md`.

### 🧪 Tests (Part 3)
- `InMemoryPatientStore`: add then get returns the same patient; unknown ID returns null; 100 Bogus patients added concurrently are all stored (thread safety).
- A tiny `RegisterPatientService` in Application using `IPatientStore`: with an NSubstitute store, registering calls `AddAsync` exactly once (`Received(1)`); returns the new ID.
- `PatientFaker` with the same seed produces the same first patient (repeatable data).
- 💥 Add a reference Domain → Infrastructure → build still works → explain why that's the danger (an architecture test catches it in L22) → remove it.

### 🏁 Checkpoint
- Four layers build; API behaves as in L02; tests green. Commit `P03: Clean Architecture skeleton and testing toolkit`.

### 🎤 Explain-it questions
1. What is Clean Architecture, and what is the dependency rule?
2. What is a port, an adapter and the composition root?
3. What does FluentAssertions add over `Assert.Equal`?
4. When do you use NSubstitute — and what should you never substitute?
5. Why give Bogus a fixed seed?

---

# PART B — CORE LOGIC

## L04 — Domain layer I: patients, value objects & invariants
**Day 1 · Slot 4 · Project step P04**
**📍 Start:** a `Patient` record in the API. **🏁 End:** a rich `Patient` entity in `Domain/PatientRegistry` with `NationalNumber`, `PersonName`, typed IDs and a `DomainException`; the API uses it; a Bogus rule that generates **valid** national numbers.

### Concepts
🟢 **Core**
- **Entity vs value object** (💡 a person vs a €10 note). ⚙️ A `record` gets compiler-generated value equality.
- **Invariants & factory methods:** private constructor + `Create(...)` → an object can't exist in an invalid state ("parse, don't validate").
- **`NationalNumber`**: 11 digits, mod-97 check, the 2000+ rule, birth date. ⚙️ The check on `850730033` → `28`.
- **Strongly typed IDs** (`readonly record struct PatientId(Guid Value)`).

🔵 **Important**
- Exceptions (`DomainException` with a stable code) vs a `Result` type. Primitive obsession. Private setters and the C# 14 `field` keyword `[C# 14]`.

### 🧩 DDD notes
- **Value object** — `NationalNumber` and `PersonName` are defined by their values and immutable.
- **Entity** — `Patient` keeps its identity while its name or GP changes.

### 🏛️ Layer notes
Domain references nothing; the API maps `RegisterPatientRequest` → `Patient.Register(...)` and `DomainException` → 409 ProblemDetails.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Rich model vs anemic model + services. ⚖️ Typed IDs: safety vs mapping work.
- 💣 A public setter bypasses every invariant with all tests green. 💣 Swapped `Guid`s compile fine. 💣 `default(PatientId)` sneaks in an empty ID.

### 🎬 Visuals
- 🎬 The mod-97 check on a real example, then the 2000+ variant.
- Comparison: primitive obsession vs value objects.

### 🛠️ Build
1. `SharedKernel`: `Entity<TId>`, `DomainException`, typed-ID pattern.
2. `PatientRegistry`: `NationalNumber`, `PersonName`, `PatientId`, `DoctorId`, `Patient` (`Register`, `ReassignGp`).
3. API: map DTO → domain; `DomainExceptionHandler` → 409 (registered before the general handler).
4. `CareRoute.Testing`: a Bogus extension `f.CareRoute().NationalNumber()` that builds valid numbers; update `PatientFaker`.

### 🧪 Tests (Part 3)
- `NationalNumber` (`[Theory]`): valid 1985 `85073003328`, valid 2000 `00010100105`; 10/12 digits; letters; wrong check; dots and dashes accepted; birth date derived; equality by value.
- `PersonName`: empty/whitespace fails; trimmed.
- `Patient`: `Register` sets GP and raises nothing else; `ReassignGp` changes GP; empty `DoctorId` fails.
- Bogus: 1,000 generated national numbers all pass `NationalNumber.Create` (the faker itself is tested).
- Reflection test: no public setters on domain types.
- 💥 Change the mod-97 formula → the theory goes red → restore.

### 🏁 Checkpoint
- Invalid national number → 400/409 from the API; tests green. Commit `P04: Patient, NationalNumber, typed IDs`.

### 🎤 Explain-it questions
1. Entity vs value object — CareRoute examples?
2. What is an invariant, and where is it enforced?
3. Why a private constructor and a `Create` method?
4. Why strongly typed IDs, and what's their weak spot?
5. Exceptions or Result for broken rules?

---

## L05 — Domain layer II: referral workflow, scheduling & domain events
**Day 2 · Slot 1 · Project step P05**
**📍 Start:** a `Patient` entity. **🏁 End:** a `Referral` state machine, a `DoctorSchedule` that refuses overlapping appointments, domain events recorded by both, and `TimeProvider` for all domain time.

### Concepts
🟢 **Core**
- **A state machine in a class:** `Draft → Submitted → InTriage → Accepted | Rejected`, `Withdrawn`; guard first, change last.
- **Testable time:** `TimeProvider` (UTC `DateTimeOffset`); `FakeTimeProvider` in tests.
- **`TimeSlot` & no double booking:** half-open intervals `[start, end)`.
- **Domain events:** past-tense facts recorded on the entity (`ReferralSubmitted`, `ReferralAccepted`, `AppointmentBooked`). ⚙️ Added to an internal list; dispatched later (L11).

🔵 **Important**
- A stateless domain service (`BookingEligibilityService`) for a rule across two objects.
- One transaction changes one aggregate.

### 🧩 DDD notes
- **Aggregate & root** — `Referral` and `DoctorSchedule` are consistency boundaries; change them only through their root, reference others by ID.
- **Aggregate size** — a schedule per *doctor per day*, not the whole calendar: small enough to load, big enough to protect "no overlap".
- **Domain event** — a fact inside one context (integration events come in L18).

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Big aggregate (strong consistency, contention) vs small (parallelism, eventual consistency).
- 💣 `DateTime.Now` in the domain → untestable and time-zone bugs. 💣 Two copies of one day both accept 10:00 (fixed with a version check, L10). 💣 Events raised but never dispatched (L11).

### 🎬 Visuals
- 🎬 The referral state machine: valid transitions green, invalid red with the blocking rule.
- 🎬 Two bookings racing for one slot.

### 🛠️ Build
1. `Referrals`: `ClinicalReason`, `Urgency`, `ReferralStatus`, `Referral` (`Draft`, `MarkIntakeComplete`, `Submit`, `StartTriage`, `Accept`, `Reject`, `Withdraw`).
2. `Scheduling`: `TimeSlot`, `Appointment`, `DoctorSchedule` (`Book`, `Cancel`), `BookingEligibilityService`.
3. `IDomainEvent` + event list on `AggregateRoot`.
4. Temporary in-memory repositories and a `ReferralsController` / `SchedulesController` (thin versions come in L06).
5. Register `TimeProvider.System`; update the ubiquitous language.
6. `CareRoute.Testing`: `ReferralBuilder` (Bogus for reason text, explicit state helpers like `.Submitted()`).

### 🧪 Tests (Part 3)
- Referral: new is Draft; `Submit` → Submitted + `ReferralSubmitted`; submit twice fails; reason < 20 chars fails (boundary 19/20); intake incomplete fails; draft 30 days old OK, 30 days + 1 tick fails (`FakeTimeProvider`); triage only from Submitted; accept/reject only from InTriage; reject needs reason; withdraw only from Draft/Submitted.
- TimeSlot: overlap, touching (`10:00–10:20` and `10:20–10:40` OK), contained, identical; 20/40 minutes only; 20-minute grid.
- DoctorSchedule: book OK + `AppointmentBooked`; overlap fails; past fails; cancel < 24 h → late; cancelled slot can be rebooked.
- BookingEligibilityService: accepted + right department → eligible; otherwise not.
- 💥 Change `<` to `<=` in the overlap check → the touching test goes red → restore.

### 🏁 Checkpoint
- Create/submit referrals and book appointments via `.http`; rule violations → 409. Commit `P05: Referral workflow, scheduling, domain events`.

### 🎤 Explain-it questions
1. How do you decide where an aggregate's boundary is?
2. Why reference other aggregates by ID?
3. What is a domain event, and when should it be dispatched?
4. How does `TimeProvider` make time rules testable?
5. Why half-open intervals for time slots?

---

## L06 — The Application layer: MediatR use cases, FluentValidation & behaviors
**Day 2 · Slot 2 · Project step P06**
**📍 Start:** controllers contain orchestration. **🏁 End:** one MediatR handler per use case in `CareRoute.Application`, organised by context and feature; FluentValidation; logging and validation behaviors; thin controllers.

### Concepts
🟢 **Core**
- **Use case = application service:** load → call domain method → save → return. Rules stay in the domain.
- **MediatR** (💡 an order ticket): `IRequest<T>`, `IRequestHandler`, `ISender.Send`. Pinned **12.5.0** (last Apache 2.0).
- **Pipeline behaviors** (💡 onion layers around every request). ⚙️ Resolved from DI and nested in registration order.
- **Thin controllers:** HTTP → command/query → `Send` → HTTP.

🔵 **Important**
- **FluentValidation** for commands; the three validation layers (input · command · invariants).
- Feature folders (`Application/Referrals/SubmitReferral/…`). Commands as an allow-list. A hand-rolled dispatcher as alternative.

### 🏛️ Layer notes
Repository interfaces live in Domain (or Application ports); handlers depend only on interfaces; Api registers everything.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ MediatR (behaviors, decoupling) vs indirection. ⚖️ Strict layers vs CRUD ceremony.
- 💣 Fat controllers. 💣 Business rules leaking into handlers (anemic model). 💣 Behaviors in the wrong order. 💣 A handler calling another handler.

### 🎬 Visuals
- 🎬 Request → Logging → Validation → Handler; an invalid request stopped by Validation.

### 🛠️ Build
1. NuGet on Application: `MediatR` **12.5.0**, `FluentValidation.DependencyInjectionExtensions`.
2. Use cases: `RegisterPatient`, `CreateReferral`, `SubmitReferral`, `GetReferralById`, `BookAppointment`.
3. `ValidationBehavior` (→ 400 ProblemDetails via a `ValidationException` handler) and `LoggingBehavior`.
4. Rewrite controllers as thin.
5. `docs/adr/ADR-002-mediatr.md`.

### 🧪 Tests (Part 3)
- Validators: each rule has a pass and a fail case (FluentValidation's `TestValidate` + FluentAssertions).
- Handlers with **NSubstitute** repositories and **Bogus** data: `SubmitReferral` loads, calls `Submit`, saves once; unknown ID → `NotFound` result and no save (`DidNotReceive`); domain rule broken → exception propagates and nothing saved.
- `ValidationBehavior`: invalid command → handler never called.
- 💥 Swap behavior registration order → the "rejected requests are logged" test fails → restore.

### 🏁 Checkpoint
- Behaviour unchanged through handlers; invalid commands → 400. Commit `P06: Application layer with MediatR and FluentValidation`.

### 🎤 Explain-it questions
1. What belongs in a handler, and what in the domain?
2. What does MediatR add, and what does it cost? Why version 12.5.0?
3. What is a pipeline behavior?
4. Where does each kind of validation live?
5. What belongs in a controller?

---

# PART C — DATA

## L07 — SQL Server basics
**Day 2 · Slot 3 · Project step P07**
**📍 Start:** data in memory. **🏁 End:** a `CareRouteDb` database (LocalDB) with one schema per bounded context, tables, keys, constraints, fictional sample data and queries.

### Concepts
🟢 **Core**
- Tables, rows, columns, data types (`nvarchar`, `datetime2`, `uniqueidentifier`).
- **Primary & foreign keys** ⚙️ (FK checked on insert and delete; no automatic index on the FK column).
- Constraints: NOT NULL, UNIQUE, CHECK — the database's back-door lock.
- SELECT/WHERE/ORDER BY/INSERT/UPDATE/DELETE, JOIN, GROUP BY.

🔵 **Important**
- **Schemas** per context (`registry`, `referrals`, `scheduling`, `directory`). NULL behaviour. **SQL injection** & parameters. How an aggregate maps to root + child tables.

### 🧩 DDD notes
- **Context boundaries in the database:** foreign keys inside a context, plain IDs across contexts.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ GUID vs int keys. ⚖️ FKs across contexts: integrity vs coupling.
- 💣 SQL injection. 💣 `= NULL` returns nothing. 💣 No UNIQUE on national number → duplicates. 💣 Local time instead of UTC.

### 🎬 Visuals
- 🎬 INNER vs LEFT JOIN. 🎬 Injection: concatenated vs parameterised.

### 🛠️ Build
1. **SQL Server Object Explorer** → `(localdb)\MSSQLLocalDB` → **Add New Database** `CareRouteDb`.
2. **New Query**: schemas + 7 tables (scripts in `docs/database`).
3. Sample data; queries: referrals per department, by status, patients without referrals, appointments per doctor per day.

### 🧪 Tests (Part 3)
SQL "tests" as scripts in `docs/database/004_checks.sql`, each with the expected result: duplicate national number refused (UQ); rejected without reason refused (CHECK); appointment of 30 minutes refused; delete a schedule cascades its appointments; injection demo returns all rows vs 0 rows with `sp_executesql`. 💥 Drop the UNIQUE constraint, insert a duplicate, try to re-add the constraint → error → clean up.

### 🏁 Checkpoint
- Commit `P07: database schema and SQL scripts`.

### 🎤 Explain-it questions
1. Primary vs foreign key?
2. INNER vs LEFT JOIN?
3. How do you prevent SQL injection?
4. Why doesn't `WHERE x = NULL` work?
5. Why one schema per context, and FKs only inside a context?

---

## L08 — SQL Server for developers: indexes, plans, transactions, concurrency
**Day 2 · Slot 4 · Project step P08**
**📍 Start:** a small schema. **🏁 End:** ~100,000 Bogus-style rows, an index with before/after numbers, a transaction demo, `rowversion` columns, and a unique index guarding identical start times.

### Concepts
🟢 **Core**
- **Index** (💡 a book index). ⚙️ 8 KB pages, B-tree root → leaf. Clustered vs nonclustered.
- **Execution plans:** seek vs scan; `SET STATISTICS IO ON`.
- **Transactions & ACID**; COMMIT/ROLLBACK.
- **Optimistic concurrency** with `rowversion`.

🔵 **Important**
- Covering indexes, SARGable queries. Isolation levels, RCSI, blocking, deadlocks. Parameter sniffing.
- Defence in depth: domain rule + filtered unique index (DoctorId, StartsAtUtc) — and what the index can't catch (overlaps).

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Indexes speed reads, slow writes. ⚖️ Optimistic vs pessimistic. ⚖️ Isolation vs blocking.
- 💣 Lost update. 💣 Deadlock victim. 💣 Function around a column → scan. 💣 Long transactions → timeouts.

### 🎬 Visuals
- 🎬 Seek vs scan. 🎬 Blocking then deadlock. 🎬 Two users, one rowversion.

### 🛠️ Build
1. Data-generation script; measure "appointments of one doctor this month" with the actual plan.
2. Add the index; record before/after in `docs/performance-notes.md`.
3. `BEGIN TRAN` → update → `ROLLBACK`.
4. `RowVersion` on Referrals and DoctorSchedules; filtered unique index.
5. Two query windows: blocking.

### 🧪 Tests (Part 3)
Scripted checks: second appointment same doctor/start refused; overlapping 09:00/40 min + 09:20 **accepted by SQL** (proves the domain rule is still needed); rowversion changes on every update; update with an old rowversion affects 0 rows. 💥 Wrap the column in `CONVERT(date, …)` → the plan turns into a scan → restore.

### 🏁 Checkpoint
- Commit `P08: indexes, transactions, rowversion, double-booking guard`.

### 🎤 Explain-it questions
1. What is an index, and what does it cost?
2. What is a transaction? ACID?
3. How does `rowversion` give optimistic concurrency?
4. What is a deadlock?
5. Why enforce "no double booking" in both domain and database?

---

## L09 — EF Core 10 in the Infrastructure layer
**Day 3 · Slot 1 · Project step P09**
**📍 Start:** in-memory repositories. **🏁 End:** `CareRouteDbContext` in Infrastructure mapping the domain (typed IDs, value objects, private fields), one repository per aggregate, migrations; the API persists to LocalDB.

### Concepts
🟢 **Core**
- **ORM, `DbContext`, `DbSet<T>`** — registered **scoped** (⚙️ not thread-safe).
- **Mapping a clean domain** with `IEntityTypeConfiguration<T>` only — no EF attributes in Domain: value conversions (typed IDs, `NationalNumber`), **complex types** (`PersonName`) `[EF10]`, backing fields, private constructors.
- **Migrations:** `Add-Migration`, `Update-Database` in the Package Manager Console (every word explained).
- **Repository per aggregate:** interface in Domain, implementation in Infrastructure. ⚙️ Dependency inversion.

🔵 **Important**
- ⚙️ Change tracking and `DetectChanges`; `AsNoTracking`; `IQueryable` vs `IEnumerable`; enums as strings; seeing generated SQL in Output.

### 🏛️ Layer notes
Infrastructure is the only project that references EF Core. If Domain needs `using Microsoft.EntityFrameworkCore`, something is wrong.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ ORM productivity vs SQL control. ⚖️ Repositories vs `DbContext` in handlers.
- 💣 A `DbContext` shared by two threads. 💣 Migration drift. 💣 `.ToList()` too early. 💣 EF's private constructor path skipping invariants.

### 🎬 Visuals
- 🎬 LINQ → SQL → rows → objects. 🎬 Change tracker states.

### 🛠️ Build
1. NuGet on Infrastructure: `Microsoft.EntityFrameworkCore.SqlServer`, `.Design`; on Api: `.Tools` (**10.0.x**).
2. `CareRouteDbContext`, configurations per aggregate with schemas.
3. Connection string `CareRouteApp` in `appsettings.Development.json`.
4. PMC: `Add-Migration InitialCreate` → read → `Update-Database`.
5. `PatientRepository`, `ReferralRepository`; swap registrations in `AddInfrastructure()`.

### 🧪 Tests (Part 3)
Quick sanity tests now (the full container suite is L10): a test that builds the EF model and asserts every aggregate is mapped and every typed ID has a converter (`context.Model.FindEntityType(...)`); a test that `PersonName` maps to two columns. 💥 Remove a value converter → the model test fails with EF's error → restore.

### 🏁 Checkpoint
- Register a patient, restart, still there. Commit `P09: EF Core persistence and migrations`.

### 🎤 Explain-it questions
1. What does an ORM hide from you?
2. Why is `DbContext` scoped?
3. How do you persist a value object and a typed ID without polluting the domain?
4. Why one repository per aggregate?
5. When `AsNoTracking`?

---

## L10 — EF Core in practice + integration tests with Testcontainers & Bogus
**Day 3 · Slot 2 · Project step P10**
**📍 Start:** repositories work. **🏁 End:** `DoctorSchedule` persisted, projections (no N+1), concurrency → 409, unit of work, and an integration-test project running the real API against **SQL Server in a container**, seeded with **Bogus**.

### Concepts
🟢 **Core**
- **N+1** (💡 a shop trip per item) vs `Include` vs **projections**.
- **Concurrency token** on aggregate roots → `DbUpdateConcurrencyException` → 409.
- **Unit of work**: one save per use case.
- **Integration tests:** `WebApplicationFactory` + **Testcontainers** (`MsSqlContainer`). ⚙️ Container starts once per fixture, migrations applied, tests run, container disposed.

🔵 **Important**
- Resetting data between tests (transactions or a reset library such as Respawn — verify). Why never the InMemory provider. Split queries. Named query filters `[EF10]`.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Real containers (realistic) vs speed → share one container per test collection.
- 💣 N+1 invisible in dev. 💣 Unhandled concurrency → 500. 💣 Tests sharing state → order-dependent failures. 💣 Docker Desktop not running → tests fail at fixture start (clear message).

### 🎬 Visuals
- 🎬 N+1 vs projection with a query counter. 🎬 Testcontainers lifecycle. 🎬 Two saves, one conflict.

### 🛠️ Build
1. Map `DoctorSchedule` + appointments; `Add-Migration AddScheduling`.
2. `GET api/referrals` list: N+1 version → watch Output → projection fix.
3. Concurrency tokens; map exception → 409.
4. `IUnitOfWork`.
5. **xUnit Test Project** `CareRoute.Api.IntegrationTests`; NuGet `Microsoft.AspNetCore.Mvc.Testing` (10.0.x), `Testcontainers.MsSql`; in `CareRoute.Testing`: `SqlServerFixture`, `CareRouteApiFactory`, `DatabaseSeeder` using the Bogus fakers.

### 🧪 Tests (Part 3)
- Register patient → GET returns it (round trip through real SQL).
- Duplicate national number → 409 (database unique index).
- Create → submit referral → 200; second submit → 409.
- Overlapping booking → 409 and database unchanged.
- Stale concurrency version → 409.
- Referral list with 50 Bogus referrals runs **one** SQL query (count via an EF interceptor or logged commands).
- 💥 Remove the projection, re-introduce `Include` in a loop → the one-query test fails → restore.

### 🏁 Checkpoint
- Integration tests green with Docker Desktop running. Commit `P10: projections, concurrency, Testcontainers integration tests`.

### 🎤 Explain-it questions
1. What is N+1, and how do you fix it?
2. How do you handle two users changing the same aggregate?
3. Unit vs integration test — what does each prove?
4. Why Testcontainers instead of LocalDB or the InMemory provider?
5. How do you keep integration tests independent?

---

## L11 — CQRS & dispatching domain events
**Day 3 · Slot 3 · Project step P11**
**📍 Start:** events recorded, never dispatched. **🏁 End:** commands vs queries; a `UnitOfWorkBehavior` that saves then dispatches; a referral timeline; `ReferralAccepted` creating a `BookingRequest` in Scheduling.

### Concepts
🟢 **Core**
- **CQS vs CQRS** (💡 returns desk vs reading room); commands by intent; queries shaped for screens.
- **Dispatch after save** ⚙️: collect events from tracked aggregates → save → publish as MediatR notifications → clear.
- **Timeline** (`ReferralHistory`) as audit trail.

🔵 **Important**
- CQRS levels ladder. Eventual consistency in the UI. Idempotent event handlers. When CQRS is overkill.

### 🧩 DDD notes
- **Policy** — "when a referral is accepted, open a booking request": two contexts cooperate through an event, not a direct call.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Dispatch before save (atomic, longer transaction) vs after (only real events, crash gap → outbox L19).
- 💣 Handler fails after save → missing timeline entry. 💣 Event loops. 💣 Stale reads.

### 🎬 Visuals
- 🎬 Command path vs query path. 🎬 `ReferralAccepted` fanning out.

### 🛠️ Build
1. `ICommand`/`IQuery` markers; `UnitOfWorkBehavior` for commands only.
2. `ReferralHistory` + migration + notification handler.
3. `GetReferralTimeline` projection query.
4. Scheduling handler creates `BookingRequest`.
5. `AcceptReferral` and `StartTriage` commands (authorisation comes in L15).

### 🧪 Tests (Part 3)
- Unit (NSubstitute): `UnitOfWorkBehavior` saves once, then publishes each event, then clears; if save throws, nothing is published.
- Integration (Testcontainers): submit + accept → timeline has 2 entries in order; accept creates exactly one `BookingRequest`; a query leaves the database unchanged (row counts before/after).
- 💥 Move publish before save and make save fail → the "nothing published" test goes red → restore.

### 🏁 Checkpoint
- Commit `P11: CQRS, event dispatch, timeline, booking requests`.

### 🎤 Explain-it questions
1. CQRS vs CQS?
2. When are domain events dispatched, and what if a handler fails?
3. How do two contexts cooperate without calling each other?
4. When is CQRS overkill?
5. How do you explain eventual consistency to a user?

---

## L12 — Observability: Serilog, structured logging, correlation IDs & health checks
**Day 3 · Slot 4 · Project step P12**
**📍 Start:** default console logging and one `/health`. **🏁 End:** Serilog configured from `appsettings.json` with enrichers and sinks (console + rolling file + **Seq** in Docker), request logging, a correlation ID on every log line and response, PII-free logs, health checks for SQL Server with `/health/live` and `/health/ready`, and OpenTelemetry traces/metrics viewable locally.

### Concepts
🟢 **Core**
- **Structured logging** (💡 a spreadsheet instead of a diary): message templates → properties you can query. ⚙️ `{PatientId}` becomes a named property, not just text.
- **Serilog**: sinks, enrichers, minimum levels and overrides, `UseSerilog`, `UseSerilogRequestLogging` (one summary line per request), configuration from `appsettings.json`, two-stage bootstrap logger.
- **Correlation IDs** (💡 a parcel tracking number): ⚙️ ASP.NET Core creates an `Activity` per request with a **W3C `traceparent`** trace ID; we enrich every log with `TraceId`, return it in a `X-Correlation-Id` response header and in ProblemDetails, and accept an incoming one.
- **Health checks**: liveness ("am I alive?") vs readiness ("can I serve? — database reachable?"). ⚙️ `IHealthCheck` per dependency, tags, separate endpoints, JSON response writer.

🔵 **Important**
- Log levels and what goes where; `LogContext.PushProperty` (scoped properties, e.g. `ReferralId`) and MediatR `LoggingBehavior` pushing the request name.
- **PII rules**: IDs only; a destructuring policy/enricher that masks `NationalNumber`.
- OpenTelemetry basics (traces, metrics) exported via OTLP to a local dashboard (the standalone **Aspire dashboard** container or Jaeger — verify image).
- Serilog and OpenTelemetry together: Serilog for logs, OTel for traces/metrics — both carry the same TraceId.

🟣 **Going further:** sampling · Serilog `Expressions` filters · health-check UI.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ More logs (visibility) vs cost, noise and privacy. ⚖️ Synchronous sinks (simple) vs async/batching (fast, may lose the last logs on crash → `Log.CloseAndFlush`).
- ⚖️ Readiness that checks every dependency (safe) vs one that takes the app out of rotation for a minor hiccup.
- 💣 String interpolation `$"…{id}"` in a log call → no structured property, unqueryable logs.
- 💣 A national number in a log line → personal data copied into every log store.
- 💣 Correlation ID lost on background work → logs you can't connect.
- 💣 Liveness that checks the database → orchestrator restarts healthy apps during a DB blip.

### 🎬 Visuals
- 🎬 One request: correlation ID generated → appears on 5 log lines (controller, behavior, handler, EF Core, response) → returned in the header.
- 🎬 Liveness vs readiness during a database outage.

### 🛠️ Build
1. NuGet on Api: `Serilog.AspNetCore`, `Serilog.Sinks.Seq`, `Serilog.Enrichers.Environment` (verify names); bootstrap logger in `Program.cs`; config in `appsettings.json`.
2. **Docker Desktop**: run `datalust/seq` (accept the licence env var — verify) on port 5341/80; open Seq.
3. `CorrelationIdMiddleware` (reads/returns the header, pushes `CorrelationId` into `LogContext`); add `traceId` to ProblemDetails.
4. A `NationalNumber` masking policy; review every existing log call for templates.
5. NuGet `AspNetCore.HealthChecks.SqlServer` (verify) — `/health/live` (no dependencies) and `/health/ready` (SQL) with JSON output.
6. OpenTelemetry: `OpenTelemetry.Extensions.Hosting`, ASP.NET Core/Http/EF instrumentation, OTLP exporter → local dashboard.

### 🧪 Tests (Part 3)
- Integration: every response has `X-Correlation-Id`; an incoming header is echoed back; ProblemDetails contains `traceId`.
- Log capture test (Serilog `InMemory`/test sink — verify package, or a custom `ILogEventSink`): registering a patient logs `PatientId` as a **property**; no log event contains the raw national number.
- Health: `/health/live` → 200 with the DB container stopped; `/health/ready` → 503 with it stopped, 200 with it running (Testcontainers `StopAsync`).
- 💥 Replace a template with `$"…"` → the "property exists" test goes red → restore.

### 🏁 Checkpoint
- In Seq, filter `CorrelationId = '…'` and see one request end to end. Commit `P12: Serilog, correlation IDs, health checks, OpenTelemetry`.

### 🎤 Explain-it questions
1. Structured vs plain-text logging — why does it matter?
2. How does a correlation ID get onto every log line?
3. Liveness vs readiness — and how can a bad probe hurt you?
4. What must never be logged in CareRoute, and how do you enforce it?
5. Logs vs traces vs metrics?

---

# PART D — USER INTERFACE & SECURITY

## L13 — Blazor WebAssembly basics
**Day 4 · Slot 1 · Project step P13**
**📍 Start:** no UI. **🏁 End:** `CareRoute.Portal` started with the API, showing patients; `CareRoute.Contracts` shared DTOs; CORS configured.

### Concepts
🟢 **Core**
- Blazor WebAssembly (💡 the browser runs a small .NET). ⚙️ Runtime in WebAssembly loads your DLLs; render tree diff; JS updates the DOM.
- Razor components: markup + `@code`, parameters, `@onclick`, `@bind`, `OnInitializedAsync`.
- `HttpClient`, loading/error states; **CORS** (💡 the browser's bouncer).
- `CareRoute.Contracts`: DTOs shared by API and portal — never domain types.

🔵 **Important**
- Server vs WebAssembly vs Auto. Scoped = singleton in WASM. `EventCallback`. Multiple startup projects. The browser's console and Network tab show the `X-Correlation-Id` from L12.

### 🧩 DDD notes
- The portal is outside every bounded context: it speaks the **published contracts**, never the domain model.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ WASM (static hosting, offline-capable, big download, code is public) vs Server (thin client, constant connection).
- 💣 CORS works from `.http`, fails in the browser. 💣 Secrets in `wwwroot`. 💣 Unhandled exception breaks the page.

### 🎬 Visuals
- 🎬 How a WASM app loads. 🎬 Component lifecycle.

### 🛠️ Build
1. **Blazor WebAssembly Standalone App** → `CareRoute.Portal`.
2. **Configure Startup Projects** → API + Portal.
3. CORS policy for the portal origin.
4. `CareRoute.Contracts` (move response DTOs there).
5. Patients page with loading/error states.

### 🧪 Tests (Part 3)
bUnit arrives in L22; today: an API integration test proving the CORS preflight from the portal origin returns `Access-Control-Allow-Origin` and that another origin doesn't. Manual check list for the page (loading, data, API stopped → friendly error). 💥 Remove the CORS policy → see the browser console error → restore.

### 🏁 Checkpoint
- Commit `P13: Blazor portal listing patients`.

### 🎤 Explain-it questions
1. How does Blazor WebAssembly run?
2. Server vs WASM vs Auto?
3. Where do you load data, and why?
4. What is CORS?
5. Why share DTOs, not domain classes?

---

## L14 — Blazor forms, routing, state & errors
**Day 4 · Slot 2 · Project step P14**
**📍 Start:** a read-only list. **🏁 End:** register-patient form with server errors, referral detail with Submit and timeline, booking page, department filter state, QuickGrid paging, error boundaries.

### Concepts
🟢 **Core**
- Routing (`@page`, parameters, `NavigationManager`); forms (`EditForm`, inputs, `DataAnnotationsValidator`).
- **Showing server errors**: 400 → `ValidationMessageStore`; 409 → friendly domain message ("This slot was just booked").
- A state service with a change event. ⚙️ Subscribe in `OnInitialized`, unsubscribe in `Dispose`.

🔵 **Important**
- Typed API clients reading ProblemDetails (and logging the `traceId` for support). `ErrorBoundary`. QuickGrid. `NavigationLock`.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Client validation (fast) vs server (trusted) — both. ⚖️ State service vs a library.
- 💣 Forgotten unsubscribe → leaks. 💣 Double submit (button disabled now; idempotency keys L19). 💣 Stale screen → 409.

### 🎬 Visuals
- 🎬 400 errors landing under fields. 🎬 Two users, one slot, friendly 409.

### 🛠️ Build
1. Typed clients. 2. `/patients/new`. 3. `/referrals/{id}` with Submit + timeline. 4. `/bookings/{referralId}`. 5. `DepartmentFilterState`. 6. QuickGrid + `ErrorBoundary`.

### 🧪 Tests (Part 3)
Unit tests for the typed clients with a fake `HttpMessageHandler` (NSubstitute-free — a small stub class): 400 ProblemDetails → field errors dictionary; 409 → `ConflictResult` with the domain code; 500 → generic error with `traceId`. `DepartmentFilterState`: notifies subscribers; unsubscribed handler not called. 💥 Remove the unsubscribe in `Dispose` and show the "not called after dispose" test failing → restore.

### 🏁 Checkpoint
- Commit `P14: forms, routing, state, booking UI`.

### 🎤 Explain-it questions
1. How does Blazor routing work?
2. How do you show API validation errors?
3. How do you share state, and what can leak?
4. How do you prevent a double submit?
5. What should the UI do on 409?

---

## L15 — Security I: authentication, authorisation & BOLA
**Day 4 · Slot 3 · Project step P15**
**📍 Start:** anyone sees everything. **🏁 End:** JWT validation, a dev-only token issuer, `GP`/`Specialist` policies, a fallback policy, resource-based checks, BOLA tests, portal login, specialist triage page.

### Concepts
🟢 **Core**
- **AuthN vs AuthZ** (💡 badge at the entrance vs ward doors). ⚙️ Authentication middleware → `ClaimsPrincipal`; authorization evaluates policies.
- **JWT** ⚙️: base64url ≠ encryption; signature → issuer → audience → lifetime.
- **Policies, fallback policy, `[Authorize]`, 401 vs 403.**
- **Resource-based authorisation & BOLA.**

🔵 **Important**
- Filter lists by user + check single resources. 403 vs 404. Portal: `AuthenticationStateProvider`, `DelegatingHandler`, `AuthorizeView` (UI hint, not security). Log the user ID (never the token) as a Serilog property.

### 🧩 DDD notes
- Access control lives in Application; true business rules ("only the target department's specialist may accept") also live in the Domain.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ JWT (no lookup) vs revocation → short lifetimes. ⚖️ 404 vs 403.
- 💣 BOLA. 💣 `UseAuthorization` before `UseAuthentication`. 💣 New controller without `[Authorize]` (fallback policy prevents). 💣 Clock skew. 💣 Trusting a user ID from the body.

### 🎬 Visuals
- 🎬 Request through authN → authZ → resource check (200/401/404). 🎬 A BOLA attack with and without the check.

### 🛠️ Build
1. `Microsoft.AspNetCore.Authentication.JwtBearer` (10.0.x).
2. `DevTokenController` guarded by `[DevelopmentOnly]`.
3. Policies, fallback, resource handlers, filtered queries; `ICurrentUser` port in Application.
4. Portal login, bearer handler, `AuthorizeView`.
5. Triage page.

### 🧪 Tests (Part 3)
`CareRoute.Testing` gets a `TestTokens` helper (signed JWTs in code). Integration: no token → 401; expired → 401; GP A reads GP B's patient → 404; GP list never contains other GPs' patients (Bogus: 3 GPs × 20 patients); GP cannot accept → 403; Dermatology specialist cannot triage Cardiology → 403; `/api/dev/token` → 404 outside Development; every controller has authorisation (reflection test: no action reachable anonymously except allow-listed). Unit: resource handlers with NSubstitute `ICurrentUser`. 💥 Remove the resource check in `GetPatient` → the BOLA test goes red → restore.

### 🏁 Checkpoint
- Commit `P15: JWT, policies, resource-based authorisation, BOLA tests`.

### 🎤 Explain-it questions
1. AuthN vs AuthZ in ASP.NET Core?
2. What's in a JWT, and how is it validated?
3. Role vs policy vs resource-based?
4. What is BOLA, and how do you test it?
5. Is hiding a button enough?

---

## L16 — ★ Checkpoint 1: review, self-test & catch-up
**Day 4 · Slot 4**

| Part | Time | What Claude delivers |
|---|---|---|
| A. Recap | 25 min | 🎬 Concept map of L01–L15 (layers, domain, data, observability, UI, security); one card per lesson: 3 ideas + 1 trade-off + 1 failure mode. |
| B. Self-test | 35 min | 20 questions (basics, under the hood, trade-offs, failure modes) → ⛔ answers + scoring sheet. |
| C. Code review | 25 min | ~50 lines with ~8 issues (logic in a controller, EF attribute in Domain, `DateTime.Now`, `.Result`, N+1, `$"…"` in a log call, national number logged, missing resource check, events before save) → ⛔ answers. |
| D. Explain your project | 15 min | A 3-minute walkthrough: layers → one aggregate → one test → one log trace with a correlation ID → one security detail. |
| E. Catch-up | rest | `CATCHUP Lxx` for anything missing. |

---

# PART E — DISTRIBUTED SYSTEM

## L17 — Docker & MongoDB: the Clinical Intake service
**Day 5 · Slot 1 · Project step P17**
**📍 Start:** one API, SQL Server. **🏁 End:** MongoDB in Docker Desktop (volume), `CareRoute.Intake.Api` (its own small Clean Architecture: Domain/Application/Infrastructure folders inside one project) with polymorphic intake forms, filtered queries, indexes created at start-up, an aggregation summary, version-based concurrency, Serilog + correlation + health checks reused.

### Concepts
🟢 **Core**
- **Docker**: image, container, ports, **volumes**.
- **Documents & collections**; C# driver (`MongoClient` once per app); polymorphic documents ⚙️ (`_t` discriminator).
- **Queries & indexes**: `Builders<T>`, `explain` (COLLSCAN vs IXSCAN), the ESR rule.
- **Aggregation pipeline** (💡 a conveyor belt: `$match` → `$group` → `$sort`).

🔵 **Important**
- MongoDB vs SQL for intake forms; embed vs reference; schemaless ≠ no schema; version field → 409; replica sets and write concern `majority` (concept).

### 🧩 DDD notes
- **Bounded context as a service** — Clinical Intake owns its model (`IntakeForm`, one per referral) and its database; Referrals only knows a `ReferralId`.
- **One aggregate = one document** — why MongoDB transactions are rarely needed.

### 🏛️ Layer notes
A small service can keep the layers as folders in one project; we accept that trade-off here and protect it with an architecture test in L22.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Document vs relational. ⚖️ Separate service vs module. ⚖️ `w:majority` vs `w:1`.
- 💣 Container without volume. 💣 Unbounded documents (16 MB). 💣 Mixed document shapes. 💣 `MongoClient` per request. 💣 Missing index fine at 100 docs, slow at a million.

### 🎬 Visuals
- 🎬 Image → container → port → volume. 🎬 Documents shrinking through an aggregation. 🎬 COLLSCAN vs IXSCAN.

### 🛠️ Build
1. Docker Desktop: `mongo:8`, `careroute-mongo`, 27017, volume `careroute-mongo-data`.
2. Compass connection.
3. **ASP.NET Core Web API** `CareRoute.Intake.Api` (Use controllers ✓); `MongoDB.Driver` 3.x; reuse Serilog setup (a shared `CareRoute.ServiceDefaults`-style extension class library — 🏛️ cross-cutting, referenced by both APIs).
4. Model + repository + `IntakeFormsController` (`PUT/GET api/intake/{referralId}`, `GET api/intake?department=&complete=`, `GET api/intake/summary`).
5. Index-creation hosted service; version field → 409; Mongo health check; same JWT validation.
6. Portal: intake section + summary card.

### 🧪 Tests (Part 3)
Unit: intake validation per department (Bogus-generated answers). Integration with **`Testcontainers.MongoDb`**: save + load each subtype (discriminator round trip); filter by department returns only matching; summary counts correct for 30 Bogus forms; conflicting update with old version → 409; indexes exist after start-up. 💥 Drop the version check → the lost-update test goes red → restore.

### 🏁 Checkpoint
- Restart the container, data survives. Commit `P17: Clinical Intake service with MongoDB`.

### 🎤 Explain-it questions
1. Container, image, volume?
2. When MongoDB, and when not?
3. Why one `MongoClient` per app?
4. How do you prevent lost updates in MongoDB?
5. Why is Clinical Intake its own service?

---

## L18 — RabbitMQ & integration events behind an `IEventBus` port
**Day 5 · Slot 2 · Project step P18**
**📍 Start:** services don't talk. **🏁 End:** RabbitMQ in Docker; an **`IEventBus` port in Application** with a RabbitMQ adapter in Infrastructure; `ReferralSubmitted` published; `CareRoute.Notifications.Worker` consumes and logs, with the correlation ID carried in message headers.

### Concepts
🟢 **Core**
- Why a broker (💡 a post office). Producer, consumer, queue, **exchange, binding, routing key**; direct/fanout/topic.
- `RabbitMQ.Client` 7 (async-first) ⚙️: one connection, several channels. Worker Service + `BackgroundService`.
- **The `IEventBus` port**: Application publishes `IIntegrationEvent`s without knowing the broker — the Azure Service Bus adapter in L30 will plug into the same port.

🔵 **Important**
- Durable queues, persistent messages, acks, prefetch. Data minimisation in messages. Correlation ID and message ID in headers; Serilog `LogContext` in the consumer. RabbitMQ vs Kafka.

### 🧩 DDD notes
- **Domain event vs integration event** — a domain event stays inside the context and may use rich types; an integration event is a stable, versioned contract (**published language**) with IDs and simple values, in `CareRoute.Contracts`.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Async messaging (decoupled) vs complexity and eventual consistency. ⚖️ A port abstraction (swappable) vs hiding broker features you might need.
- 💣 Publishing the domain-event class. 💣 Auto-ack + crash → lost. 💣 Connection per message. 💣 Personal data in messages. 💣 Correlation ID not copied to headers → disconnected logs.

### 🎬 Visuals
- 🎬 One message through direct, fanout, topic. 🎬 Publish → queue → deliver → ack.

### 🛠️ Build
1. Docker: `rabbitmq:4-management`, ports 5672/15672, user/pass `careroute`.
2. `CareRoute.Contracts/Events/ReferralSubmittedV1`.
3. `IEventBus` in Application; `RabbitMqEventBus` in Infrastructure (one connection, topic exchange `careroute.events`).
4. Domain-event handler translates → integration event → `IEventBus.PublishAsync`.
5. **Worker Service** `CareRoute.Notifications.Worker`: consumer, Serilog, correlation from headers, RabbitMQ health check.

### 🧪 Tests (Part 3)
Unit: the translator maps domain → integration event with IDs only (no names); the handler calls `IEventBus.PublishAsync` once (NSubstitute). Integration with **`Testcontainers.RabbitMq`**: publish via `RabbitMqEventBus` → a test consumer receives it with `correlation-id` and `message-id` headers; the message body contains no national number. 💥 Publish the domain event object instead of the contract → the "contract only" test fails → restore.

### 🏁 Checkpoint
- Submit a referral → Worker logs "department notified" with the same CorrelationId in Seq. Commit `P18: integration events over RabbitMQ`.

### 🎤 Explain-it questions
1. Why a message broker?
2. Domain event vs integration event?
3. Exchange vs queue; direct/fanout/topic?
4. Why put the broker behind an `IEventBus` port?
5. What should — and shouldn't — go in a message?

---

## L19 — Reliable messaging: retries, dead letters, outbox, inbox & idempotency
**Day 5 · Slot 3 · Project step P19**
**📍 Start:** happy path; save-then-publish can lose events. **🏁 End:** publisher confirms, manual acks, delayed retry, parking-lot queue; a transactional outbox with a relay; an inbox in the Worker; `Idempotency-Key` for POSTs.

### Concepts
🟢 **Core**
- What can go wrong: broker restart, consumer crash, poison message. **Publisher confirms, manual ack/nack, dead-letter exchange, delayed retry.**
- **At-least-once** → duplicates happen.
- **Dual-write problem** (💡 diary and letter) → **outbox**: events saved in the same transaction; a relay publishes via `IEventBus`.
- **Inbox / idempotent consumer** and **idempotency keys** for HTTP (an action filter).

🔵 **Important**
- Quorum queues and delivery limits (RabbitMQ 4). Several relay instances. Outbox cleanup. Contract versioning. Queue-depth monitoring (a health check that warns on depth).

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Confirms (safe) vs throughput. ⚖️ Outbox: guaranteed vs latency + table. ⚖️ Inbox: exactly-once effect vs a lookup per message.
- 💣 Poison message looping. 💣 Ack before work. 💣 Relay publishes then crashes → duplicate. 💣 Outbox grows forever. 💣 Renamed event field breaks consumers.

### 🎬 Visuals
- 🎬 The no-loss chain with failures injected. 🎬 Retry → wait queue → parking lot. 🎬 Outbox → relay → broker → inbox, duplicate ignored.

### 🛠️ Build
1. Confirms + persistent; manual ack; retry topology; parking lot after 3.
2. `OutboxMessages` (migration); `UnitOfWorkBehavior` writes integration events into the outbox; `OutboxRelay` background service.
3. Worker: `ProcessedMessages` table (its own small DB).
4. `[Idempotent]` action filter on create referral and booking; portal sends the key.
5. `docs/runbooks/dead-letter-queue.md`.

### 🧪 Tests (Part 3)
Integration (SQL + RabbitMQ containers): submitting stores exactly one outbox row in the same transaction (rollback → zero rows); relay publishes and marks sent; same message delivered twice → one notification; poison message ends in the parking lot after 3 attempts; same POST + same key twice → one referral, same response. 💥 Stop the RabbitMQ container mid-test, submit, start it → event still arrives (and explain what would happen without the outbox).

### 🏁 Checkpoint
- Commit `P19: reliable messaging, outbox, inbox, idempotency keys`.

### 🎤 Explain-it questions
1. How do you make sure messages aren't lost?
2. What is the dual-write problem, and how does the outbox fix it?
3. What happens if the relay publishes twice?
4. What does a dead-letter queue need around it?
5. How do you make a POST safe to retry?

---

## L20 — Distributed tracing: one trace, one correlation ID across services
**Day 5 · Slot 4 · Project step P20**
**📍 Start:** each service logs and traces alone. **🏁 End:** one trace from portal → API → SQL → outbox relay → RabbitMQ → Worker → and API → Intake (HTTP); one correlation ID across all logs in Seq; a shared observability setup used by every service.

### Concepts
🟢 **Core**
- **Trace, span, parent/child** ⚙️; W3C `traceparent` propagated automatically over HTTP by `HttpClient`, **manually over the broker** (inject on publish, extract on consume).
- **The outbox gap**: the relay runs later — store the `traceparent` in the outbox row and continue the trace (or use a span link).
- Logs ↔ traces: Serilog `TraceId`/`SpanId` on every event → jump from a log to its trace.

🔵 **Important**
- Correlation vs causation IDs. Messaging semantic conventions. Sampling (head vs tail). A shared `AddCareRouteObservability()` extension so all services behave the same.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ More spans (visibility) vs cost/noise/PII in attributes.
- 💣 Lost trace context at the broker → two unrelated traces. 💣 Personal data as span attributes. 💣 100% sampling in production → bills and noise.

### 🎬 Visuals
- 🎬 A trace waterfall across 4 services, then the same view with context lost at the broker.

### 🛠️ Build
1. Shared observability extension (Serilog + OTel) used by Api, Intake, Worker.
2. Inject/extract `traceparent` in `RabbitMqEventBus` and the consumer; store it in the outbox.
3. API → Intake typed `HttpClient` (intake completeness); trace flows automatically.
4. Worker writes notification documents to MongoDB (instrumented).

### 🧪 Tests (Part 3)
Integration with an in-memory `ActivityListener`/OTel in-memory exporter (verify package): publishing creates a producer span with the current trace ID; consuming creates a consumer span whose parent is the producer; the outbox row stores `traceparent`; the consumer's log events carry the same `TraceId` (test sink). 💥 Stop copying `traceparent` into headers → the "same trace" test fails, and the dashboard shows two traces → restore.

### 🏁 Checkpoint
- One trace shows the full journey. Commit `P20: distributed tracing across HTTP and RabbitMQ`.

### 🎤 Explain-it questions
1. How does trace context cross HTTP? A broker? An outbox?
2. Correlation ID vs trace ID — same thing?
3. What must never be a span attribute?
4. Why sample traces?
5. How do you go from a log line to the trace?

---

## L21 — Process manager: deadlines & escalation (behind a port)
**Day 6 · Slot 1 · Project step P21**
**📍 Start:** nobody follows up accepted referrals. **🏁 End:** a `BookingProcess` process manager (state in SQL) opened by `ReferralAccepted`, closed by `AppointmentBooked`, escalating at the 7/60-day deadline; urgent-triage escalation at 48 h — implemented behind an **`IBookingDeadlineScheduler` port** so Durable Functions can replace the hosted timer in L31.

### Concepts
🟢 **Core**
- **Process manager / saga** (💡 a case worker with a checklist): state, events in, commands out, timeouts.
- **Timers with `TimeProvider`** and a polling hosted service; testing with `FakeTimeProvider`.
- **Orchestration vs choreography.**

🔵 **Important**
- Compensation. Idempotent timers. MassTransit/sagas and their licences (v9+ commercial) vs hand-rolled. Why Durable Functions is a natural cloud replacement (preview of L31).

### 🧩 DDD notes
- **Policy vs process manager** — a policy reacts once ("when accepted, open a booking request"); a process manager remembers state across several events and time.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Orchestration (one visible place) vs choreography (loose, invisible process). ⚖️ Polling (simple) vs durable timers (precise, platform-specific).
- 💣 Process stuck because an event never arrived (timeouts). 💣 Timer fires twice after restart → duplicate escalation.

### 🎬 Visuals
- 🎬 The booking process: happy path, then a missed deadline and escalation.

### 🛠️ Build
1. `BookingProcess` table + entity; `IBookingDeadlineScheduler` port (Application); `HostedDeadlineScheduler` adapter (Infrastructure).
2. Handlers: `ReferralAccepted` opens, `AppointmentBooked` closes; due processes publish `BookingDeadlineMissedV1`.
3. Urgent-triage check (48 h).
4. Notifications Worker handles escalations.
5. `docs/adr/ADR-003-process-manager.md`.

### 🧪 Tests (Part 3)
Unit (`FakeTimeProvider`): urgent accepted → not escalated at 7 days − 1 tick, escalated at 7 days; routine at 60 days; booked before deadline → closed, never escalated; escalation runs once even if the scheduler ticks twice; urgent not triaged after 48 h → escalated. Integration: accept → process row exists; book → closed. 💥 Remove the "already escalated" guard → the "runs once" test goes red → restore.

### 🏁 Checkpoint
- Commit `P21: booking process manager and escalations`.

### 🎤 Explain-it questions
1. What is a process manager?
2. Orchestration vs choreography?
3. What if an expected event never arrives?
4. How do you test a 60-day deadline in milliseconds?
5. Why put the scheduler behind a port?

---

# PART F — QUALITY & PRODUCTION

## L22 — Testing strategy: bUnit, architecture tests & CI
**Day 6 · Slot 2 · Project step P22**
**📍 Start:** many tests, no strategy, no CI, nothing guarding the layers. **🏁 End:** a written test strategy, bUnit component tests, architecture tests enforcing Clean Architecture and context boundaries, `global.json`, and a GitHub Actions workflow running everything (including Testcontainers) on every push.

### Concepts
🟢 **Core**
- **Test pyramid vs honeycomb** — where CareRoute's tests sit and why.
- **bUnit** ⚙️: renders a component in a test renderer, clicks, asserts markup — no browser.
- **Architecture tests**: "Domain references nothing", "Application doesn't reference Infrastructure", "Referrals doesn't use Scheduling internals", "no EF Core in Domain".
- **CI** with GitHub Actions: workflow, job, steps; Docker is available on Ubuntu runners for Testcontainers.

🔵 **Important**
- Flaky tests: time, order, shared data, random data without seed. Test data builders vs Bogus. Contract tests for integration events (snapshot of the JSON shape).

🟣 **Going further:** mutation testing (Stryker.NET) · Playwright · coverage gates.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Pyramid vs honeycomb. ⚖️ Container tests in CI: realistic vs slower builds.
- 💣 Flaky tests teaching the team to ignore red. 💣 Green locally, red in CI (SDK, Docker, time zone). 💣 Architecture eroding silently.

### 🎬 Visuals
- 🎬 A CI pipeline stopping a change. Diagram: pyramid vs honeycomb with CareRoute's counts.

### 🛠️ Build
1. `docs/testing-strategy.md`.
2. `CareRoute.Portal.Tests` (bUnit).
3. `CareRoute.ArchitectureTests` (`NetArchTest.Rules` or `TngTech.ArchUnitNET.xUnit` — verify which is maintained).
4. `global.json` pinning the .NET 10 SDK.
5. GitHub → **Actions** → .NET starter workflow → .NET 10; build + test.

### 🧪 Tests (Part 3)
bUnit: `StatusBadge` shows the right label per status; specialist buttons hidden for a GP; register form shows server field errors (fake API client). Architecture: the four layer rules, context-boundary rule, "no `DateTime.Now` in Domain", "controllers end in Controller and are thin (no repository injection)". Contract: `ReferralSubmittedV1` serialises to the approved JSON. 💥 Add Domain → Infrastructure reference again (from L03) → the architecture test is red locally **and** in CI → remove.

### 🏁 Checkpoint
- All green in Test Explorer and on GitHub Actions. Commit `P22: bUnit, architecture tests, CI`.

### 🎤 Explain-it questions
1. Pyramid vs honeycomb — which fits CareRoute?
2. How do you test a Blazor component?
3. What does an architecture test protect?
4. How do you kill flaky tests?
5. Why can Testcontainers run in CI?

---

## L23 — Production readiness: secrets, containers & probes
**Day 6 · Slot 3 · Project step P23**
**📍 Start:** runs on your machine. **🏁 End:** secrets in User Secrets, Docker images for the APIs and Worker, health endpoints wired as container probes, Serilog production config (JSON to console), migrations as a pipeline step, Central Package Management, runbooks.

### Concepts
🟢 **Core**
- Environments; **User Secrets** (step 2 of 3: appsettings → user secrets → Key Vault in L28).
- **Dockerfile** ⚙️: multi-stage; port 8080; non-root user.
- **Probes**: liveness and readiness from L12 used by a container orchestrator; graceful shutdown.
- **Production logging**: compact JSON to stdout, levels, no PII, flush on shutdown.

🔵 **Important**
- `Directory.Build.props`, Central Package Management. Migrations from the pipeline (script/bundle). Kubernetes basics (concept). Staying current with patches.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Migrations at start-up vs from the pipeline. ⚖️ Strict readiness vs cascading outages.
- 💣 Readiness checking too much → cascading restarts. 💣 Secrets in Git history. 💣 Wrong container port.

### 🎬 Visuals
- 🎬 Rolling update with readiness and graceful shutdown. Diagram: multi-stage Dockerfile.

### 🛠️ Build
1. **Manage User Secrets** for connection strings, JWT key, RabbitMQ password.
2. **Add > Docker Support** for API, Intake, Worker; Container launch profile.
3. Serilog `appsettings.Production.json` (CompactJsonFormatter).
4. `Directory.Build.props` + CPM.
5. Migration bundle step in CI (artifact).
6. Runbooks: incident and release checklists.

### 🧪 Tests (Part 3)
Integration: in Production environment, `/api/dev/token` → 404 and ProblemDetails have no exception details; options validation fails fast when the JWT key is missing; readiness returns JSON with each dependency's status. CI: a test that scans the repo for secret-looking values in `appsettings*.json` (simple regex). 💥 Make liveness check the database, stop SQL, watch the container restart loop → restore.

### 🏁 Checkpoint
- Commit `P23: production readiness`.

### 🎤 Explain-it questions
1. How do you manage configuration and secrets — and what's next after User Secrets?
2. What does a multi-stage Dockerfile do?
3. How can a readiness check hurt you?
4. How should migrations reach production?
5. What should production logs look like?

---

## L24 — Security II: audit, privacy & token handling
**Day 6 · Slot 4 · Project step P24**
**📍 Start:** authN/authZ since L15. **🏁 End:** audit of every patient read, log redaction enforced, security headers, tight CORS, mass-assignment tests, ADR on OAuth flows and token storage.

### Concepts
🟢 **Core**
- **OAuth 2.0 vs OpenID Connect**; **auth code + PKCE** (portal), **client credentials** (services) — and managed identity in Azure (L28).
- Token storage: memory vs session storage vs **BFF**.
- **Audit of reads** ⚙️: an `AuditAccessBehavior` for `ISensitiveQuery` → `AccessLog` row (who, what, when, correlation ID).
- **Redaction**: `Microsoft.Extensions.Compliance.Redaction` (verify) or the Serilog masking policy from L12 — one consistent approach.

🔵 **Important**
- GDPR special-category data (concepts; verify before quoting periods). Security headers. OWASP API Top 10 mapped to CareRoute.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ BFF vs tokens in the browser. ⚖️ Audit everything vs selectively.
- 💣 The audit log leaking data. 💣 `AllowAnyOrigin` + credentials. 💣 Tokens in URLs. 💣 Over-fetching. 💣 Mass assignment.

### 🎬 Visuals
- 🎬 Auth code + PKCE. 🎬 Opening a patient → audit row.

### 🛠️ Build
1. `AccessLog` + `AuditAccessBehavior`.
2. Redaction unified; security headers middleware; CORS tightened.
3. `docs/adr/ADR-004-token-handling.md`; `docs/security/data-protection.md`.

### 🧪 Tests (Part 3)
Opening a patient writes exactly one audit row with user and correlation ID; a list query writes one row, not one per patient; the body cannot set `Status` or `ReferringDoctorId`; responses carry the security headers; CORS refuses unknown origins; no log event (test sink, 200 Bogus patients registered) contains a national number or full name. 💥 Mark the patient query as non-sensitive → the audit test fails → restore.

### 🏁 Checkpoint
- Commit `P24: audit, redaction, security headers, token ADR`.

### 🎤 Explain-it questions
1. OAuth 2.0 vs OpenID Connect?
2. Which flow for the portal? For a background service?
3. Where should tokens live? What is a BFF?
4. Why isn't authorisation enough for health data?
5. How do you keep personal data out of logs?

---

## L25 — Performance & caching
**Day 7 · Slot 1 · Project step P25**
**📍 Start:** how fast is it? **🏁 End:** a load-test tool, profiler measurements, a slow spot fixed, HybridCache and output caching, per-doctor rate limiting — with numbers.

### Concepts
🟢 **Core**
- Latency vs throughput; **p95/p99**; measure → change one thing → measure.
- **HybridCache** `[.NET 9+]` and output caching; ⚙️ stampede protection.
- **Thread pool** and why `.Result` hurts.
- **Rate limiting** (429, `Retry-After`).

🔵 **Important**
- Caching and privacy (never cache per-user data without the user in the key). Connection-pool limits. Accept-then-queue for spikes. Using Serilog request-duration logs and OTel metrics to find slow endpoints.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Freshness vs speed. ⚖️ Rate limits vs legitimate peaks.
- 💣 Premature optimisation. 💣 Stampede. 💣 Privacy leak via cache. 💣 Pool exhaustion.

### 🎬 Visuals
- 🎬 Stampede and coalescing. 🎬 Blocked vs free threads. Chart: p50/p95/p99 before/after.

### 🛠️ Build
1. `CareRoute.LoadTest` console app.
2. Profiler during load; a deliberate `.Result` → fix → numbers in `docs/performance-notes.md`.
3. HybridCache for directory data; `[OutputCache]` on reference data.
4. Per-doctor rate limiting via the `sub` claim.

### 🧪 Tests (Part 3)
Integration: the 101st request in a window → 429 with `Retry-After`; two doctors have separate limits; directory query hits the database once for 50 concurrent requests (stampede test with a counting repository); a per-user endpoint is never output-cached (two GPs get different data). 💥 Cache the patient list without the user key → the privacy test goes red → restore.

### 🏁 Checkpoint
- Commit `P25: measurements, caching, rate limiting`.

### 🎤 Explain-it questions
1. How do you approach a performance problem?
2. Why p95/p99?
3. What is a cache stampede?
4. How can caching leak data?
5. Why is `.Result` dangerous?

---

## L26 — Resilience & service boundaries
**Day 7 · Slot 2 · Project step P26**
**📍 Start:** HTTP calls between services, an external registry to integrate. **🏁 End:** `AddStandardResilienceHandler` on every outbound `HttpClient`; a fake national registry behind an **anti-corruption layer**; `IntakeFormCompleted` published by Intake so Submit no longer calls Intake synchronously; an optional YARP gateway; ADR on service boundaries.

### Concepts
🟢 **Core**
- The network is unreliable: **timeouts, retries (safe operations only), circuit breaker** ⚙️ (closed → open → half-open).
- `Microsoft.Extensions.Http.Resilience` standard handler and its logs.
- Monolith vs modular monolith vs microservices; sync vs async; the **distributed monolith**.

🔵 **Important**
- Retry storms, timeout budgets (inner < outer). Service-to-service auth. Strangler fig. Gateway (YARP now, API Management in L33).

### 🧩 DDD notes
- **Anti-corruption layer** — translate the foreign registry's fields and codes into our language before anything reaches the domain.
- **Context map** — update `docs/context-map.md` with the relationship types (customer–supplier, published language, ACL).

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Microservices vs operational cost. ⚖️ Retries vs load. ⚖️ ACL vs conformist.
- 💣 Cascading failure. 💣 Retry storm. 💣 Inner timeout longer than outer. 💣 Foreign codes spreading into the database.

### 🎬 Visuals
- 🎬 Cascading failure vs contained with a circuit breaker. 🎬 A foreign response through the ACL.

### 🛠️ Build
1. Resilience handler on API → Intake and API → registry.
2. `CareRoute.NationalRegistry.Fake` (controllers, foreign model, configurable delay/failure).
3. ACL: `NationalRegistryClient` + `NationalRegistryTranslator` behind an Application port.
4. Intake publishes `IntakeFormCompletedV1`; Referrals consumer calls `MarkIntakeComplete()`.
5. Optional YARP `CareRoute.Gateway`.
6. `docs/architecture.md`, `docs/adr/ADR-005-service-boundaries.md`.

### 🧪 Tests (Part 3)
Unit: translator maps every foreign code (theory) and rejects unknown codes. Integration (fake registry in-process via `WebApplicationFactory`): slow registry → request fails within the timeout budget; after N failures the circuit opens and calls fail fast (count calls on the fake); `IntakeFormCompletedV1` marks the referral complete; submit works with Intake stopped. 💥 Set the inner timeout above the outer → the timeout-budget test fails → restore.

### 🏁 Checkpoint
- Commit `P26: resilience, anti-corruption layer, intake-completed event`.

### 🎤 Explain-it questions
1. How do you handle a slow downstream service?
2. Circuit breaker states?
3. When are retries dangerous?
4. What is an anti-corruption layer, and when is it worth it?
5. Modular monolith or microservices for a new system?

---

## L27 — ★ Checkpoint 2: review before the cloud
**Day 7 · Slot 3**

| Part | Time | What Claude delivers |
|---|---|---|
| A. Recap | 20 min | 🎬 Concept map L17–L26; one card per lesson. |
| B. Self-test | 35 min | 20 mixed questions → ⛔ answers + scores. |
| C. Failure drills | 30 min | "A GP got the same notification twice"; "two appointments in one slot"; "a trace stops at RabbitMQ"; "readiness flaps every minute" — investigate with logs, traces, tests. → ⛔ answers. |
| D. Azure prep | 15 min | Map every local piece to its Azure counterpart (LocalDB → Azure SQL, RabbitMQ → Service Bus, user secrets → Key Vault, Seq/dashboard → App Insights, hosted timer → Durable Functions, YARP → APIM, Worker notifications → Logic App). Check your Azure account and Portal access. |
| E. Catch-up | rest | `CATCHUP Lxx`. |

---

# PART G — AZURE

> ☁️ **Every Azure lesson** starts with the cost of what you'll create (cheapest tier that works — verify current prices) and ends with a 🧹 **cleanup** step: stop or delete what you don't need. Everything lives in **one resource group `rg-careroute-dev`** so it can be deleted in one click. Region: one close to you (e.g., West Europe), used for everything.

## L28 — Azure basics, App Service, Key Vault & deploying from GitHub
**Day 7 · Slot 4 · Project step P28**
**📍 Start:** CareRoute runs locally and in CI. **🏁 End:** a resource group with a budget alert; Azure SQL Database; `CareRoute.Api` on **App Service** (Linux, .NET 10) with a staging **deployment slot**; secrets in **Key Vault** read through a **managed identity**; GitHub Actions building, testing, running the migration bundle and deploying to the slot, then swapping.

### Concepts
🟢 **Core**
- **Azure building blocks**: subscription, resource group, region, resource, pricing tier; the Portal (💡 a building: subscription = the building, resource groups = rooms, resources = furniture).
- **App Service** ⚙️: an App Service **plan** (the VMs you pay for) hosts one or more **web apps**; deployment slots; app settings override `appsettings.json` (same configuration layers as L02); health-check path uses `/health/ready`.
- **Managed identity** (💡 a staff badge issued by the hospital — no password to steal) ⚙️: the app gets an Entra ID identity; Azure issues tokens to it; `DefaultAzureCredential` picks it up.
- **Key Vault**: secrets, access through Azure RBAC role "Key Vault Secrets User"; `AddAzureKeyVault` configuration provider or Key Vault references in app settings.

🔵 **Important**
- Azure SQL Database (serverless/basic tier) and Entra authentication with managed identity (no SQL password).
- GitHub Actions deploy with **OpenID Connect federation** (no publish profile secret) — verify current recommended setup.
- Slot swap with warm-up; app settings marked "slot setting".
- Cost: budget + alert in **Cost Management**.

🟣 **Going further:** Bicep for the same resources · Azure Container Apps as an alternative host · Static Web Apps for the portal.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ App Service (managed, simple) vs Container Apps/AKS (flexible, more to run). ⚖️ Key Vault references (no code) vs config provider (reload, all secrets at once).
- 💣 Managed identity enabled but no role assigned → 403 from Key Vault at start-up (app fails to start: read the **Log stream**).
- 💣 Swapping without warm-up → first users hit a cold app. 💣 Connection string left in `appsettings.json` "for now". 💣 Forgotten resources → surprise bill.
- 💣 Wrong `ASPNETCORE_ENVIRONMENT` → dev token endpoint exposed (L15's guard + test protects you).

### 🎬 Visuals
- 🎬 App start: managed identity → token → Key Vault → secrets into configuration → SQL with Entra auth.
- 🎬 Slot swap: staging warms up → swap → production; rollback by swapping back.

### 🛠️ Build (☁️ Azure Portal + Visual Studio)
1. **Cost Management > Budgets** → monthly budget + 80% alert.
2. **Create a resource > Resource group** `rg-careroute-dev`.
3. **Azure SQL Database** (cheapest dev tier, verify), Entra admin = you; firewall: allow Azure services.
4. **Key Vault** (RBAC mode); add secrets (JWT signing key, RabbitMQ placeholder).
5. **Web App**: .NET 10, Linux, plan B1 (verify); enable **system-assigned identity**; grant "Key Vault Secrets User"; add the identity as a SQL user (one SQL script, explained).
6. NuGet on Api: `Azure.Extensions.AspNetCore.Configuration.Secrets`, `Azure.Identity`; load Key Vault when not in Development.
7. **Deployment slot** `staging`; health check path `/health/ready`.
8. GitHub: federated credential → workflow `deploy.yml`: build → test → migration bundle → deploy to staging → swap.
9. 🧹 Stop the web app when you finish the day (or scale the plan down — explain the difference).

### 🧪 Tests (Part 3)
- Unit: configuration builder adds Key Vault only outside Development (inspect providers).
- **Smoke tests** (`CareRoute.SmokeTests`, run by the workflow after deploying to staging, against the slot URL): `/health/ready` → 200; `/api/patients` without token → 401; `/api/dev/token` → 404; response has `X-Correlation-Id` and security headers. The swap only happens if they pass.
- 💥 Remove the Key Vault role assignment → app fails to start → find the cause in **Log stream** → restore.

### 🏁 Checkpoint
- CareRoute.Api answers on its `azurewebsites.net` URL; no secrets in app settings. Commit `P28: App Service, Key Vault, CI/CD`.

### 🎤 Explain-it questions
1. Subscription, resource group, App Service plan, web app — how do they relate?
2. What is a managed identity, and why is it safer than a connection-string password?
3. How do secrets get from Key Vault into `IConfiguration`?
4. Why deploy to a slot and swap?
5. What did you do to avoid surprise costs?

---

## L29 — Application Insights: logs, traces, metrics & alerts in Azure
**Day 8 · Slot 1 · Project step P29**
**📍 Start:** Serilog + OpenTelemetry go to Seq and a local dashboard. **🏁 End:** a workspace-based **Application Insights** resource receiving traces, metrics and Serilog logs from the API (and later Functions), the correlation ID from L12 searchable in **Transaction search**, KQL queries saved in a workbook, an **availability test** on `/health/ready`, and two **alerts**.

### Concepts
🟢 **Core**
- **Application Insights** on a **Log Analytics workspace** ⚙️: tables `requests`, `dependencies`, `traces`, `exceptions`, `customMetrics`; `operation_Id` = our W3C trace ID.
- **Azure Monitor OpenTelemetry Distro** (`UseAzureMonitor()`) ⚙️: the same OTel pipeline from L12/L20 with a new exporter — no code changes in Domain/Application.
- **Serilog → App Insights**: logs flow through OpenTelemetry (or a dedicated sink — verify the current recommendation) with properties as `customDimensions`.
- **KQL basics**: `where`, `project`, `summarize`, `join` on `operation_Id`.

🔵 **Important**
- Application map, end-to-end transaction view, Live Metrics, Failures blade.
- Availability tests and **alert rules** (failed requests, p95 latency); action groups (email).
- Sampling and ingestion cost; daily cap. PII: what our redaction already protects.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Everything in App Insights (one place) vs cost per GB → sampling + log levels. ⚖️ Vendor-neutral OTel vs App Insights SDK features.
- 💣 Two telemetry pipelines (old SDK + distro) → duplicate telemetry. 💣 Connection string missing → silent: no data, no error. 💣 Alert without an action group → nobody is told. 💣 Personal data in `customDimensions`.

### 🎬 Visuals
- 🎬 One request: Serilog event + OTel spans → exporter → App Insights tables → one end-to-end transaction.

### 🛠️ Build (☁️ + Visual Studio)
1. **Create a resource > Application Insights** (workspace-based) in `rg-careroute-dev`.
2. NuGet `Azure.Monitor.OpenTelemetry.AspNetCore` on Api; `UseAzureMonitor()` when a connection string is configured (stored as an app setting — it's not a secret, explain why).
3. Serilog wiring to App Insights (verify path); remove nothing from the local Seq setup.
4. Deploy; generate traffic; **Transaction search** by `X-Correlation-Id`.
5. KQL: failed requests by operation; p95 per endpoint; logs by `CorrelationId`; save to a **workbook**.
6. **Availability test** on `/health/ready`; **alerts**: failed requests > N, availability < 100%.
7. 🧹 Set a **daily cap** on the workspace.

### 🧪 Tests (Part 3)
- Unit: telemetry setup adds the Azure Monitor exporter only when the connection string exists.
- Smoke (added to the deploy workflow): a request with a known `X-Correlation-Id` → a KQL query through the Azure Monitor Query client (verify package) finds it in `requests` within a few minutes (marked as a slow, optional test).
- 💥 Break the connection string → no data, no error → find it via the missing availability data / alert → restore.

### 🏁 Checkpoint
- One user action visible end to end in App Insights. Commit `P29: Application Insights`.

### 🎤 Explain-it questions
1. How does a trace ID become `operation_Id`?
2. Why use the OpenTelemetry distro instead of the classic SDK?
3. How do Serilog properties show up in App Insights?
4. Write a KQL query for p95 latency per endpoint.
5. How do you control App Insights cost?

---

## L30 — Azure Service Bus replaces RabbitMQ
**Day 8 · Slot 2 · Project step P30**
**📍 Start:** `IEventBus` → RabbitMQ. **🏁 End:** a Service Bus namespace with topic `careroute-events` and subscriptions; `ServiceBusEventBus` adapter in Infrastructure (selected by configuration); the outbox relay publishes to Service Bus; the Worker (or a Function in L31) consumes with peek-lock; dead-letter handling; managed identity access.

### Concepts
🟢 **Core**
- **Queues vs topics/subscriptions** (💡 a pigeonhole vs a newsletter with subscriber filters); subscription filters (SQL/correlation).
- **Peek-lock** ⚙️: receive → lock → complete/abandon/dead-letter; lock duration and renewal; delivery count → automatic **dead-letter sub-queue** after max deliveries.
- **The swap**: only Infrastructure changes — prove it with `git diff` and the architecture tests.
- `Azure.Messaging.ServiceBus` with `DefaultAzureCredential` ("Azure Service Bus Data Sender/Receiver" roles).

🔵 **Important**
- RabbitMQ ↔ Service Bus concept map (exchange/topic, binding/subscription, DLX/DLQ, ack/complete, prefetch).
- Sessions for ordering; duplicate detection (message ID window) vs our inbox; scheduled messages; `ApplicationProperties` for `traceparent`/correlation.
- Tiers (Basic has no topics — Standard needed; verify).

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Managed broker (no ops, SLA) vs cost and portability. ⚖️ Service Bus duplicate detection vs our inbox (keep the inbox: it also covers redelivery after lock loss).
- 💣 Processing longer than the lock → message redelivered while still running → duplicates. 💣 Basic tier chosen → no topics. 💣 Nobody watching the DLQ. 💣 Trace context not copied into `ApplicationProperties`.

### 🎬 Visuals
- 🎬 Peek-lock: receive → lock → complete; then a crash → lock expires → redelivery → max deliveries → DLQ.
- Diagram: RabbitMQ vs Service Bus concepts side by side.

### 🛠️ Build (☁️ + Visual Studio)
1. **Service Bus namespace** (Standard); topic `careroute-events`; subscriptions `notifications`, `scheduling`, with filters on `EventType`.
2. Roles for the web app identity and your own user (local development via Visual Studio's Azure sign-in).
3. `ServiceBusEventBus` (Infrastructure) + `MessagingOptions.Provider = RabbitMq | ServiceBus`.
4. Consumer side: `ServiceBusProcessor` in the Worker; inbox unchanged.
5. Service Bus health check; DLQ runbook update.
6. 🧹 Note the Standard tier base cost; delete the namespace at the end of the course.

### 🧪 Tests (Part 3)
- Contract: both adapters produce the same body and the same `traceparent`/correlation properties (shared test cases run against both — RabbitMQ in a container, Service Bus with a fake `ServiceBusSender` via NSubstitute, since the client classes are mockable — verify).
- Architecture: Domain and Application unchanged since L29 (no new references; `IEventBus` signature identical).
- Integration (optional, real namespace, tagged `Category=Azure`): publish → receive on `notifications`.
- 💥 Throw in the consumer every time → watch the delivery count rise and the message land in the DLQ in **Service Bus Explorer** (Portal) → restore.

### 🏁 Checkpoint
- Submitting a referral in Azure reaches the Worker through Service Bus, same correlation ID in App Insights. Commit `P30: Service Bus adapter`.

### 🎤 Explain-it questions
1. Queue vs topic/subscription?
2. How does peek-lock work, and what happens when processing outlasts the lock?
3. What changed in the code when we swapped brokers — and what didn't?
4. Duplicate detection vs inbox?
5. How do you carry trace context through Service Bus?

---

## L31 — Azure Functions & Durable Functions: the booking-deadline orchestration
**Day 8 · Slot 3 · Project step P31**
**📍 Start:** the hosted `BookingProcess` polls the database. **🏁 End:** `CareRoute.Functions` (isolated worker, .NET 10 — verify support) with a Service Bus–triggered function starting a **Durable orchestration** per accepted referral; a **durable timer** for the 7/60-day deadline racing an external event `AppointmentBooked`; escalation via an activity; the `IBookingDeadlineScheduler` port now has a Durable adapter.

### Concepts
🟢 **Core**
- **Azure Functions** (💡 a light switch: code runs when something happens): triggers & bindings, isolated worker model, Consumption/Flex Consumption vs Premium plans (verify current options), cold starts.
- **Durable Functions** ⚙️: orchestrator (deterministic, **replayed** from history), activities (do the work), **durable timers**, **external events**, instance IDs (= `ReferralId` → natural idempotency).
- **The deadline pattern**: `Task.WhenAny(timer, WaitForExternalEvent("AppointmentBooked"))` → booked: cancel timer, complete; timeout: call `EscalateActivity`.

🔵 **Important**
- **Orchestrator rules**: no `DateTime.Now`, no I/O, no random — use `context.CurrentUtcDateTime`; why (replay).
- Storage (Azure Storage / Durable Task Scheduler — verify current default) and monitoring in App Insights.
- Hosted polling (L21) vs Durable: precision, visibility, cost, lock-in.

### 🏛️ Layer notes
Functions is a new **outer** project (like Api): it references Application (to reuse handlers/ports) and Infrastructure. The orchestrator is infrastructure; the *rule* (7 vs 60 days) still comes from the Domain.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Durable (exact timers, history, scale-to-zero) vs a hosted service (portable, simple). ⚖️ Consumption (cheap, cold starts) vs Premium (warm, costs more).
- 💣 Non-deterministic orchestrator code (`DateTime.UtcNow`, `Guid.NewGuid()`) → replay errors or wrong branches.
- 💣 Starting a second instance with the same ID → handle "already exists". 💣 Event raised before the orchestration started → lost unless you check state. 💣 Long-running activity hitting the function timeout.

### 🎬 Visuals
- 🎬 Orchestration replay: start → schedule timer → sleep → event arrives → replay from history → complete.
- 🎬 Timeout branch → escalation activity.

### 🛠️ Build (Visual Studio + ☁️)
1. **Add > New Project > Azure Functions** → `CareRoute.Functions` (isolated worker, .NET 10, verify template options).
2. Service Bus trigger on subscription `scheduling` for `ReferralAcceptedV1` → start orchestration with instance ID = ReferralId.
3. Orchestrator `BookingDeadlineOrchestrator`; activities `EscalateBookingDeadline` (publishes via `IEventBus`) and `GetDeadlinePolicy` (domain rule).
4. `AppointmentBookedV1` trigger → `RaiseEventAsync`.
5. `DurableBookingDeadlineScheduler` adapter; disable the hosted one in Azure via configuration.
6. Publish to a **Function App** (Flex Consumption/Consumption, verify) with managed identity and App Insights.
7. 🧹 Consumption cost is near zero at idle — explain the storage account cost.

### 🧪 Tests (Part 3)
- Unit: orchestrator with a mocked `TaskOrchestrationContext` (NSubstitute — verify it's an abstract class you can substitute): booked before timer → no escalation activity called; timer wins → `EscalateBookingDeadline` called once; urgent uses 7 days, routine 60 (deadline passed to `CreateTimer`).
- Unit: the trigger uses `ReferralId` as instance ID and ignores "already exists".
- Architecture: Functions references Application/Infrastructure; nothing references Functions.
- 💥 Use `DateTime.UtcNow` in the orchestrator → explain/observe the replay problem (a test asserting the timer deadline comes from `context.CurrentUtcDateTime` goes red) → restore.

### 🏁 Checkpoint
- In the Portal, an orchestration instance per accepted referral; a test deadline (minutes, via config) escalates. Commit `P31: Durable Functions booking deadline`.

### 🎤 Explain-it questions
1. What is a trigger and a binding?
2. Why must orchestrator code be deterministic?
3. How does a durable timer differ from `Task.Delay`?
4. How did the port from L21 make this swap easy?
5. When would you *not* use Durable Functions?

---

## L32 — Logic Apps: the notification workflow
**Day 8 · Slot 4 · Project step P32**
**📍 Start:** the Worker logs "notified". **🏁 End:** a **Logic App (Consumption)** triggered by the Service Bus subscription `notifications` that parses the integration event, looks up the recipient through a CareRoute API endpoint, and sends an email (Office 365 Outlook or another connector — fictional recipients/test mailbox), with retry policy, run history and failures dead-lettered.

### Concepts
🟢 **Core**
- **Logic Apps** (💡 a flowchart that runs itself): triggers, actions, connectors, designer; Consumption vs Standard.
- **Workflow definition** ⚙️ (JSON under the designer); run history; each action's inputs/outputs; retry policies.
- **Calling CareRoute securely**: HTTP action with **managed identity** to an API endpoint protected by an app role — only IDs travel in the message, the Logic App fetches what it needs.

🔵 **Important**
- Logic Apps vs Azure Functions vs code in the Worker: who owns the workflow (ops/business vs developers). Parse JSON with the event's schema; handling `EventType` with a Switch. Connector authentication and data leaving your boundary (email content = personal data → minimal content: "You have a new referral update — open CareRoute").

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Low-code (fast to change, visual history) vs code (testable, versioned, reviewable). ⚖️ Consumption (pay per action) vs Standard (VNet, local dev).
- 💣 Patient details put in the email body. 💣 Connector token expired → silent failures until someone checks run history (alert on failed runs). 💣 Peek-lock completed before the email is sent → lost notification. 💣 Changing the event contract breaks the Parse JSON step.

### 🎬 Visuals
- 🎬 Service Bus message → trigger → Parse JSON → HTTP (managed identity) → Send email → complete; failure branch → retry → dead-letter.

### 🛠️ Build (☁️ Azure Portal)
1. **Create a resource > Logic App (Consumption)** in `rg-careroute-dev`; system-assigned identity.
2. Trigger: Service Bus "When a message is received in a topic subscription (peek-lock)".
3. Parse JSON with the `ReferralSubmittedV1` schema (from `CareRoute.Contracts` snapshot test, L22); Switch on `EventType`.
4. API: `GET api/notifications/recipients/{referralId}` protected by an app role; grant the Logic App identity.
5. Send email (test mailbox); complete or dead-letter the message.
6. Export the workflow JSON into `infra/logicapps/notifications.json` and commit it.
7. Alert on failed runs. Disable the Worker's email path (it keeps the inbox/timeline work).

### 🧪 Tests (Part 3)
- API integration: the recipients endpoint returns only the needed fields; anonymous → 401; a GP token → 403; Logic App role → 200.
- Contract: the JSON schema used by the Logic App validates against serialized `ReferralSubmittedV1` and `BookingDeadlineMissedV1` (schema test in code — a renamed field fails here before it fails in Azure).
- Manual Azure test plan: submit → email arrives; break the HTTP action URL → run fails → message dead-lettered → alert fires.
- 💥 Rename a field in the event → the schema test goes red → restore.

### 🏁 Checkpoint
- A submitted referral sends a test email; run history shows every step. Commit `P32: Logic App notifications`.

### 🎤 Explain-it questions
1. When would you choose Logic Apps over a Function or Worker code?
2. How does the Logic App call CareRoute without a secret?
3. What data may go into a notification email, and why so little?
4. How do you notice a failing Logic App?
5. How do you keep a low-code workflow versioned?

---

## L33 — API Management: one front door
**Day 9 · Slot 1 · Project step P33**
**📍 Start:** two APIs on separate URLs. **🏁 End:** an **API Management** instance (Developer or Consumption tier — verify) importing both OpenAPI documents, with policies for **JWT validation**, **rate limiting**, correlation-ID forwarding, CORS for the portal, versioning (`v1`), a product with a subscription key for the "partner" use case; backends locked so only APIM can reach them.

### Concepts
🟢 **Core**
- **API gateway** (💡 the hospital reception desk) vs YARP from L26: managed, policies, developer portal.
- **APIM structure**: APIs, operations, products, subscriptions, backends, named values (from Key Vault).
- **Policy pipeline** ⚙️: `inbound` → `backend` → `outbound` → `on-error`; scopes (global → product → API → operation).
- Key policies: `validate-jwt`, `rate-limit-by-key`, `set-header`, `cors`, `retry`.

🔵 **Important**
- Defence in depth: APIM validates the JWT **and** the API still does (never trust the gateway alone). Versions vs revisions. Locking backends (access restrictions / private endpoints concept). APIM tiers and cost; Consumption cold starts. Correlation: APIM's own request ID vs our `X-Correlation-Id`.

### ⚖️ Trade-offs & 💣 failure modes
- ⚖️ Managed gateway (policies, portal, analytics) vs cost and another hop. ⚖️ Rate limits at the gateway vs in the app (L25 — keep both, different jobs).
- 💣 Backend still publicly reachable → gateway bypassed. 💣 Removing JWT checks from the API "because APIM does it". 💣 Policy XML error → every call 500. 💣 Developer tier used for production (no SLA).

### 🎬 Visuals
- 🎬 A request through the policy pipeline: JWT invalid → 401 at inbound; rate limit → 429; OK → backend → outbound header added.

### 🛠️ Build (☁️ Azure Portal)
1. **Create a resource > API Management** (cheapest tier that supports what we use — verify; Developer can take ~30+ minutes to create — start it at the beginning of the lesson).
2. Import CareRoute.Api and Intake.Api from `/openapi/v1.json`.
3. Global policy: forward/generate `X-Correlation-Id`; CORS for the portal.
4. API policies: `validate-jwt` (same issuer/audience as the API), `rate-limit-by-key` on the `sub` claim.
5. Product `partners` with subscription key on a read-only reference-data API.
6. Restrict App Service access to APIM; point the portal at the APIM URL.
7. Store policy XML in `infra/apim/` and commit.
8. 🧹 APIM is the most expensive piece of the course — delete it after L34 or immediately if on Developer tier and not needed.

### 🧪 Tests (Part 3)
Smoke tests against the APIM URL (added to the workflow, optional stage): no token → 401 from APIM (response header shows it came from the gateway); valid token → 200 with `X-Correlation-Id`; the direct App Service URL → 403; partner API without key → 401, with key → 200; burst over the limit → 429. 💥 Remove `validate-jwt` from APIM → requests still fail with 401 **from the API** → defence in depth proven → restore.

### 🏁 Checkpoint
- The portal talks only to APIM. Commit `P33: API Management`.

### 🎤 Explain-it questions
1. What does APIM add over YARP?
2. Explain the policy pipeline and policy scopes.
3. Why validate the JWT in both APIM and the API?
4. How do you stop clients bypassing the gateway?
5. Products and subscription keys — who are they for?

---

# FINISH

## L34 — ★ Final checkpoint + capstone
**Day 9 · Slots 2–3 (double lesson, ~4 hours) · Project step P34**

| Part | Time | What Claude delivers |
|---|---|---|
| A. Explain CareRoute | 20 min | 3-minute presentation (layers → an aggregate → an event through outbox → Service Bus → Durable Function → Logic App → one App Insights trace), then 8 follow-up questions. |
| B. Mixed questions | 45 min | 40 questions across all lessons → ⛔ answers + scoring sheet. List your **top 5 gaps**. |
| C. Failure drills | 35 min | "A GP got the same email twice"; "the deadline escalated although the appointment was booked"; "App Service won't start after a deploy"; "p95 doubled on Monday morning" — investigate with App Insights KQL, Durable history, Service Bus DLQ, tests. |
| D. Remediation | 45 min | Short re-teach of your top 5 gaps with a visual each. |
| E. README & demo | 40 min | README (architecture diagram, local + Azure setup, ADR index, known limitations); a 5-minute demo script with one failure handled gracefully. |
| F. Cheat sheet & cleanup | 30 min | One printable page of key ideas; ☁️ **Azure cleanup checklist** (APIM, Service Bus, Function App, Logic App, App Service plan, SQL, Key Vault soft-delete, workspace) or a plan to keep a minimal setup; tag `v1.0`. |

- Commit `P34: documentation, demo script, v1.0`.

---

# APPENDICES

## Appendix A — Visual Studio cheat sheet

| Action | Where / shortcut |
|---|---|
| Run with / without debugger | **F5** / **Ctrl+F5**; stop **Shift+F5** |
| Breakpoint; step over; step into | **F9**; **F10**; **F11** |
| Build solution | **Ctrl+Shift+B** |
| Run all tests | **Test > Run All Tests** (Ctrl+R, A) — **Test Explorer** |
| Run tests by trait (e.g., skip `Category=Azure`) | Test Explorer → group by **Traits** |
| Quick fix / add `using` | **Ctrl+.** |
| Go to definition; find references | **F12**; **Shift+F12** |
| Add a NuGet package / pick a version | Right-click project → **Manage NuGet Packages** → **Browse** → **Version** dropdown |
| Add a project reference | Right-click **Dependencies** → **Add Project Reference** |
| Several projects on F5 | Right-click solution → **Configure Startup Projects** → **Multiple** |
| HTTP requests | `.http` file → **Send request**; **View > Other Windows > Endpoints Explorer** |
| Databases | **View > SQL Server Object Explorer** |
| Package Manager Console | **Tools > NuGet Package Manager > Package Manager Console** |
| Git | **View > Git Changes** |
| User secrets | Right-click project → **Manage User Secrets** |
| Docker support | Right-click project → **Add > Docker Support** |
| Azure sign-in (for `DefaultAzureCredential` locally) | **Tools > Options > Azure Service Authentication** (verify) |
| Connected services (Key Vault, App Insights…) | Right-click project → **Add > Connected Service** |
| Publish | Right-click project → **Publish** |
| Profiling | **Debug > Performance Profiler** (Alt+F2) |

## Appendix B — Docker Desktop recipes (no terminal)

| Service | Image : tag | Ports (host → container) | Volume | Environment |
|---|---|---|---|---|
| MongoDB | `mongo` : `8` | 27017 → 27017 | `careroute-mongo-data` → `/data/db` | — |
| RabbitMQ | `rabbitmq` : `4-management` | 5672, 15672 | optional | `RABBITMQ_DEFAULT_USER=careroute`, `RABBITMQ_DEFAULT_PASS=careroute` |
| Seq | `datalust/seq` : latest (verify) | 5341 → 80 (verify) | `careroute-seq-data` → `/data` | licence acceptance variable (verify) |
| OTel dashboard | Aspire dashboard or Jaeger (verify image) | UI + OTLP 4317 | — | — |

Testcontainers starts its own SQL Server, MongoDB and RabbitMQ containers for tests — **Docker Desktop must be running** from L10 on.

## Appendix C — NuGet packages

| When | Package | Version |
|---|---|---|
| L03 | xUnit (template), `FluentAssertions` | **7.x (pinned — v8+ is commercial)** |
| L03 | `NSubstitute`, `Bogus` | latest (verify licence) |
| L05 | `Microsoft.Extensions.TimeProvider.Testing` | 10.x |
| L06 | `MediatR` | **12.5.0 (pinned — last Apache 2.0)** |
| L06 | `FluentValidation.DependencyInjectionExtensions` | latest |
| L09 | `Microsoft.EntityFrameworkCore.SqlServer`, `.Design`, `.Tools` | **10.0.x** |
| L10 | `Microsoft.AspNetCore.Mvc.Testing` | **10.0.x** |
| L10 | `Testcontainers.MsSql` (+ a DB reset library, verify) | latest |
| L12 | `Serilog.AspNetCore`, `Serilog.Sinks.Seq`, enrichers, a test sink | latest (verify names) |
| L12 | `AspNetCore.HealthChecks.SqlServer` (+ `.MongoDb`, `.Rabbitmq`, `.AzureServiceBus` later) | latest (verify) |
| L12 | `OpenTelemetry.Extensions.Hosting`, `.Exporter.OpenTelemetryProtocol`, `.Instrumentation.AspNetCore`, `.Http`, EF Core instrumentation | latest stable |
| L14 | `Microsoft.AspNetCore.Components.QuickGrid` | 10.0.x |
| L15 | `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.Components.Authorization` | 10.0.x |
| L17 | `MongoDB.Driver`, `Testcontainers.MongoDb` | 3.x / latest |
| L18 | `RabbitMQ.Client`, `Testcontainers.RabbitMq` | 7.x / latest |
| L22 | `bunit`; `NetArchTest.Rules` or `TngTech.ArchUnitNET.xUnit` | latest (verify) |
| L24 | `Microsoft.Extensions.Compliance.Redaction` | 10.x (verify) |
| L25 | `Microsoft.Extensions.Caching.Hybrid` | 10.x |
| L26 | `Microsoft.Extensions.Http.Resilience`; optional `Yarp.ReverseProxy` | 10.x / latest |
| L28 | `Azure.Identity`, `Azure.Extensions.AspNetCore.Configuration.Secrets` | latest |
| L29 | `Azure.Monitor.OpenTelemetry.AspNetCore` | latest (verify) |
| L30 | `Azure.Messaging.ServiceBus` | latest |
| L31 | `Microsoft.Azure.Functions.Worker`, `.Extensions.DurableTask`, `.Extensions.ServiceBus` | latest (verify .NET 10 support) |

Not used on purpose (licensing): MediatR v13+, FluentAssertions v8+, MassTransit v9+, AutoMapper v15+. Know them; don't depend on them here. If FluentAssertions 7 ever causes trouble, **AwesomeAssertions** (Apache 2.0 fork, same API) is the drop-in alternative.

## Appendix D — Healthcare vocabulary

| Term | Meaning |
|---|---|
| **GP** | General practitioner — the family doctor who refers patients. |
| **Specialist** | A hospital doctor in one department who triages and treats. |
| **Referral** | A GP's request that a department sees a patient (`ServiceRequest` in HL7 FHIR). |
| **Triage** | Sorting referrals by urgency and suitability before accepting them. |
| **Intake form** | Department-specific clinical questions answered before the first visit. |
| **Appointment / slot** | A reserved time with a doctor. |
| **National register number** | 11-digit personal ID (birth date + serial + mod-97 check). Personal data. |
| **HL7 FHIR** | The international standard for exchanging health data as REST resources. |
| **Special-category data** | GDPR: health data needs stronger protection. |
| **Deadline / SLA** | Triage within 48 h; appointment within 7 or 60 days. |
| **Audit trail** | Who created, changed or **viewed** which record, and when. |

## Appendix E — DDD quick reference (used in 🧩 notes)

| Term | One-line meaning | In CareRoute | First 🧩 note |
|---|---|---|---|
| Ubiquitous language | Shared words of experts and developers, used in code | `Submit()`, `StartTriage()` | L03 |
| Bounded context | A boundary where a model's words have one meaning | Referrals, Scheduling… | L03 |
| Value object | Defined by values, immutable | `NationalNumber`, `TimeSlot` | L04 |
| Entity | Identified by ID, changes over time | `Patient` | L04 |
| Aggregate / root | Consistency boundary changed only through its root | `Referral`, `DoctorSchedule` | L05 |
| Invariant | A rule that must always hold | "Appointments never overlap" | L04 |
| Domain event | Something that happened, inside a context | `ReferralAccepted` | L05 |
| Domain service | Domain logic spanning aggregates | `BookingEligibilityService` | L05 |
| Policy | "When X happens, then do Y" | Accepted → booking request | L11 |
| Integration event / published language | A stable contract between contexts | `ReferralSubmittedV1` | L18 |
| Process manager | Long-running process with state | `BookingProcess` / Durable orchestration | L21 |
| Anti-corruption layer | Translates a foreign model into yours | National-registry ACL | L26 |
| Context map | How contexts relate | `docs/context-map.md` | L26 |

## Appendix F — Clean Architecture quick reference

| Layer | Contains | References | Never contains |
|---|---|---|---|
| Domain | Entities, value objects, domain events, repository interfaces, domain services | nothing | EF Core, HTTP, logging frameworks, `DateTime.Now` |
| Application | Commands/queries, handlers, validators, behaviors, ports (`IEventBus`, `ICurrentUser`, `IBookingDeadlineScheduler`) | Domain | DbContext, broker clients, controllers |
| Infrastructure | EF Core, repositories, outbox, RabbitMQ/Service Bus adapters, Key Vault, ACL clients | Application (+ Domain) | Business rules |
| Api / Functions / Worker | Controllers, middleware, triggers, composition root | Application + Infrastructure | Business rules, SQL |

Swaps proven in the course: in-memory store → EF Core (L09) · RabbitMQ → Service Bus (L30) · hosted timer → Durable Functions (L31).

## Appendix G — Local ↔ Azure map

| Local (L01–L27) | Azure (L28–L33) |
|---|---|
| LocalDB / SQL Server container | Azure SQL Database (Entra auth, managed identity) |
| User Secrets | Key Vault (managed identity) |
| Seq + OTel dashboard | Application Insights + Log Analytics |
| RabbitMQ | Service Bus (topic + subscriptions) |
| Hosted `BookingProcess` timer | Durable Functions orchestration |
| Notifications Worker email path | Logic App |
| YARP gateway | API Management |
| `docker run` / F5 | App Service + deployment slots, deployed by GitHub Actions |

## Appendix H — .NET 10 & C# 14 features used

| Feature | Tag | Lesson |
|---|---|---|
| `.slnx` solution format | VS 2026 | L01 |
| OpenAPI 3.1 document (no Swagger UI since .NET 9) | `[.NET 10]` | L01, L33 (APIM import) |
| `field` keyword | `[C# 14]` | L04 |
| Complex types; named query filters; `LeftJoin` | `[EF10]` | L09, L10 |
| HybridCache | `[.NET 9+]` | L25 |

.NET 10 is LTS (November 2025 → November 2028). Never present preview features as available. CareRoute uses controllers; the .NET 10 `AddValidation()` is for minimal APIs and isn't used.
