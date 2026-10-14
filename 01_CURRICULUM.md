# CURRICULUM — CareRoute: secure .NET 10, identity platforms, DevOps & Azure — from zero to interview-ready

**50 lessons (L50 is a double) · ~2 hours each · 4 lessons a day · 13 days · 1 project that grows every lesson**

Core: .NET 10 (LTS) · C# 14 · ASP.NET Core 10 (controllers, REST) · EF Core 10 · SQL Server · **Clean Architecture** · MediatR & CQRS · FluentValidation · Blazor Web App (WebAssembly + BFF) · MongoDB · RabbitMQ
**Security & identity:** web security · cryptography essentials · ASP.NET Core authentication & authorisation middleware · **OAuth 2.0** · **OpenID Connect** · **SAML 2.0** · JWT · **Keycloak** (admin, Admin REST API, themes, **Java SPI plugins**) · **Duende IdentityServer** · **Ping Identity** · Microsoft Entra ID · threat modelling · OWASP
**DevOps:** **Git** · pull requests · **GitHub Actions CI/CD** · code, dependency & container scanning · Docker · config-as-code
Testing: xUnit · FluentAssertions 7.x · NSubstitute · Bogus · Testcontainers (incl. Keycloak) · bUnit · JUnit (SPIs)
Observability: Serilog · structured logging · correlation IDs · health checks · OpenTelemetry · Application Insights
Azure: App Service · Container Apps (Keycloak) · Key Vault · Application Insights · Service Bus · Functions & Durable Functions · Logic Apps · API Management

---

## 0. How this course works

### Who it's for
You know some programming but **nothing about security, identity or DevOps**. By the end you can build, operate, troubleshoot and **explain in an interview** a .NET system secured by a real identity platform, integrated with partners over OAuth 2.0, OIDC and SAML 2.0, with a customised Keycloak and a production-grade pipeline.

### The depth stack (every 🟢 concept)

| Layer | What it answers | Marker |
|---|---|---|
| 1. Orientation | What is it? Where does it fit? | 🧭 |
| 2. Why | Which problem does it solve? (analogy) | 💡 |
| 3. Definition | Plain words, then technical (+ RFC/spec) | 🟦 |
| 4. The basics | How do I use it? | 📘 |
| 5. Under the hood | What happens at runtime — for protocols, a 📜 raw HTTP trace | ⚙️ |
| 6. Trade-offs | Gain, cost, when not, alternative | ⚖️ |
| 7. Failure modes | Symptom (exact error) → cause → notice → fix | 💣 |
| 8. Production practice | What a senior engineer insists on | ⬛ |

Then 🟩 example · 🟥 mistake · 🟪 say it out loud · 🎤 interview angle · 🟨 in short.

**Priority levels:** 🟢 Core (full stack, max 3–4 per lesson) · 🔵 Important (compact) · 🟣 Advanced (pointers → `DEEPER`).

### Why identity comes early — and why not on day 1
Security can't be understood without something to secure, and identity can't be understood without web basics. So:
1. **Days 1–4** build a small but real system (API, REST design, Git & CI, Clean Architecture, domain, data, logging, a portal) — everything identity will plug into, and the **observability you need to troubleshoot auth**.
2. **Days 4–9 (L15–L33)** are the **security & identity track**: web security → crypto → ASP.NET Core auth middleware → OAuth 2.0 → OIDC → Keycloak → securing API and portal → service-to-service and partner integrations → custom middleware → SAML 2.0 → troubleshooting → Keycloak plugins → IdentityServer → Ping & Entra → threat modelling → a **mock-interview checkpoint**.
3. **Days 9–13** reuse that identity foundation everywhere: securing messaging and services, a DevSecOps pipeline, Keycloak in Azure, APIM validating Keycloak tokens.

### Clean Architecture is the backbone

```
 Api / Portal host (BFF) / Worker / Functions   ← authentication handlers, middleware, composition root
   Infrastructure   ← EF Core, Keycloak Admin client, brokers, Key Vault, ACL clients
     Application    ← use cases, ICurrentUser, authorisation requirements, ports
       Domain       ← entities, value objects, business rules (incl. "only the target department's specialist may accept")
```
Dependencies point inward. Swaps proven in the course: dev token issuer → Keycloak (L22) · RabbitMQ → Service Bus (L46) · hosted timer → Durable Functions (L47).

### DDD — only where it helps
No DDD lessons. A short 🧩 **DDD note** explains a design choice where it matters (Appendix E lists them). `DDD <term>` for more.

### Tests at the end of every lesson (no TDD)
Part 2 builds and runs the feature. **Part 3 — Test** writes every test on the lesson's 🧪 list. Security tests are first-class: every protected endpoint is tested for 401/403/404 and for wrong issuer, audience and lifetime. Once per lesson you 💥 break a rule on purpose and watch its test go red.

### Git workflow from L04
Every lesson after L04 is a **feature branch → small commits → pull request → CI green → merge**. Then you press **Sync** on the repository in Project knowledge, so the next lesson starts from your real code (🔎 Repo check). By the end you'll have ~45 merged PRs — real Git practice, not theory.

### Five parts per lesson (~2 hours)

| Part | Time | What you do |
|---|---|---|
| 1. Understand | 35 min | Concepts, 📜 traces, 🎬 animations. Explain each ⚙️ out loud. |
| 2. Build | 45 min | Add the feature (Visual Studio, Keycloak console, IntelliJ, Azure Portal). |
| 3. Test | 20 min | Write and run the lesson's tests; one 💥 break-it. |
| 4. Practice | 12 min | Quiz, explain-it, **interview drill** — out loud. |
| 5. Review | 8 min | Summary, glossary, merge the PR, update `02_CONTINUITY.md`. |

**Between lessons:** 10 minutes away from the screen. Lunch after the second lesson.

### A note on expectations
Four lessons a day is intense, and the identity track is the hardest part. If a day overruns, send `BEHIND`, or move the last lesson to the next morning. L33 and L42 are built-in catch-up days. Use `MOCK INTERVIEW <topic>` any evening for 15 minutes of practice instead of rereading.

---

## 1. The project: CareRoute

### 1.1 The story (fictional)
CareRoute is a **hospital platform**: **GPs** register **patients** and **refer** them to a **hospital department**; the GP fills in the department's **intake form**; **specialists** **triage** and accept or reject; an accepted referral becomes an **appointment** within a deadline. A **partner laboratory** integrates over OAuth 2.0 client credentials; a **partner hospital** signs its staff in through its own identity provider (federation); a **legacy reporting app** uses SAML 2.0. Every access to patient data is audited. **All data and users are fictional.**

### 1.2 Bounded contexts

| Context | What it owns | Where it lives |
|---|---|---|
| **Patient Registry** | Patients, national numbers, the assigned GP | `CareRoute.Api`, SQL Server |
| **Referrals** (core) | Referral workflow | `CareRoute.Api`, SQL Server |
| **Scheduling** (core) | Schedules, appointments, booking rules | `CareRoute.Api`, SQL Server |
| **Hospital Directory** | Hospitals, departments, doctors | `CareRoute.Api`, SQL Server |
| **Clinical Intake** | Intake forms | `CareRoute.Intake.Api`, MongoDB |
| **Notifications** | Telling people something happened | Worker → Logic App |
| **Identity & Access** | Who you are, what you may do | **Keycloak** (realm `careroute`); partner IdPs: Duende IdentityServer (OIDC), a SAML IdP; Ping/Entra as comparisons |

### 1.3 The identity model (built from L21)

| Item | Value |
|---|---|
| Realm | `careroute` |
| Realm roles | `gp`, `specialist`, `admin`, `auditor` |
| Groups | `/gps`, `/specialists/cardiology`, `/specialists/orthopaedics`, `/specialists/dermatology` |
| User attributes → token claims | `doctor_id`, `department_id` (custom mapper, L29) |
| Clients | `careroute-portal` (confidential, auth code + PKCE, BFF) · `careroute-api` (resource server / audience) · `careroute-intake-api` · `careroute-worker` (client credentials) · `partner-lab` (client credentials, limited scopes) · `careroute-reports` (SAML 2.0 SP) |
| Client scopes | `patients:read`, `referrals:write`, `intake:read`, `directory:read`, … |
| Identity providers (brokering) | `northside-hospital` (OIDC → Duende IdentityServer, L30) · `legacy-saml-idp` (SAML, L26) |
| Customisations | CareRoute login theme (L28) · `doctor-claims` protocol mapper, `careroute-audit` event listener, `licence-check` authenticator (L29) |

### 1.4 How the project grows

| Lesson | What we add |
|---|---|
| **Part A — Foundations** | |
| L01 | Solution + first API (`PatientsController`) |
| L02 | Pipeline, DI, configuration, validation, ProblemDetails |
| L03 | REST API design done right: resources, verbs, status codes, pagination, versioning, idempotency, OpenAPI |
| L04 | Git & DevOps foundations: branches, PRs, branch protection, GitHub Actions CI |
| L05 | Clean Architecture + testing toolkit + architecture tests |
| **Part B — Core logic** | |
| L06 | Domain I: patients, value objects, invariants |
| L07 | Domain II: referral workflow, scheduling, domain events |
| L08 | Application layer: MediatR, FluentValidation, behaviors |
| **Part C — Data** | |
| L09 | SQL Server basics |
| L10 | SQL for developers: indexes, transactions, concurrency |
| L11 | EF Core 10 in Infrastructure |
| L12 | EF Core in practice + Testcontainers + Bogus |
| **Part D — Observability & UI** | |
| L13 | Observability: Serilog, correlation IDs, health checks, OpenTelemetry |
| L14 | Blazor Web App portal (WebAssembly interactivity, server host), bUnit |
| **Part E — Security & identity** | |
| L15 | Web security I: HTTP, TLS & certificates, origins, cookies, CORS |
| L16 | Web security II: attacks & defences (OWASP Top 10, XSS, CSRF, SSRF, open redirect…) |
| L17 | Cryptography for identity: hashing, HMAC, signatures, keys, X.509, JWK, Data Protection |
| L18 | Authentication & authorisation in ASP.NET Core: schemes, handlers, JWT, policies, resource checks, BOLA |
| L19 | OAuth 2.0 from first principles |
| L20 | OpenID Connect |
| L21 | Keycloak fundamentals |
| L22 | Securing the API with Keycloak |
| L23 | Securing the portal: OIDC + BFF, sessions, refresh, logout |
| L24 | Service-to-service & partner integrations: client credentials, token exchange, scopes, introspection |
| L25 | Authentication & authorisation middleware in depth |
| L26 | SAML 2.0 |
| L27 | Troubleshooting authentication flows |
| L28 | Customising Keycloak I: Java primer, themes, Admin REST API, config as code |
| L29 | Customising Keycloak II: SPI plugins |
| L30 | Duende IdentityServer & identity federation |
| L31 | Identity platforms compared: Ping Identity, Entra ID, choosing & migrating |
| L32 | Secure by design: threat modelling, audit, privacy, production identity checklist |
| L33 | ★ Checkpoint 1: identity & security mock interview |
| **Part F — Distributed system** | |
| L34 | CQRS & domain-event dispatch |
| L35 | Docker & MongoDB: the Clinical Intake service (secured) |
| L36 | RabbitMQ & integration events behind `IEventBus` |
| L37 | Reliable messaging: outbox, inbox, idempotency |
| L38 | Distributed tracing across services |
| **Part G — Production** | |
| L39 | DevSecOps pipeline & production readiness |
| L40 | Performance & caching |
| L41 | Resilience & service boundaries (anti-corruption layer) |
| L42 | ★ Checkpoint 2 |
| **Part H — Azure** | |
| L43 | Azure basics, App Service, Key Vault, managed identity, CD |
| L44 | Keycloak in production on Azure (Container Apps + PostgreSQL) |
| L45 | Application Insights |
| L46 | Service Bus replaces RabbitMQ |
| L47 | Azure Functions & Durable Functions: booking deadlines |
| L48 | Logic Apps: notifications |
| L49 | API Management: one secured front door |
| L50 | ★ Final checkpoint, capstone & technical-interview simulation (double) |

### 1.5 Where we end up

```
 Browser ─► Portal host (Blazor Web App = BFF: OIDC cookie session, tokens stay server-side)
                │ cookie                           │ OIDC (code + PKCE)
                ▼                                  ▼
        API Management (validate-jwt, rate limit) ◄── Keycloak (Container Apps + PostgreSQL)
                │                                      ├─ brokers ► Northside IdentityServer (OIDC)
        ┌───────┴─────────────┐                        ├─ brokers ► legacy SAML IdP
        ▼                     ▼                        └─ SAML IdP ► CareRoute.Reports (SP)
  CareRoute.Api          CareRoute.Intake.Api
  (Clean Architecture)   (MongoDB)                Partner lab ─(client credentials)─► APIM
  Azure SQL                   │
     │ outbox → IEventBus     │
     ▼                        ▼
  Service Bus ─► Durable Functions (deadlines) · Logic App (notifications)
  Key Vault (managed identity) · Application Insights (logs, traces, auth failures, Keycloak events)
```

### 1.6 Final solution structure

```
CareRoute (.slnx)
├─ src
│  ├─ CareRoute.Domain · CareRoute.Application · CareRoute.Infrastructure · CareRoute.Api
│  ├─ CareRoute.Contracts                # DTOs + integration-event contracts
│  ├─ CareRoute.ServiceDefaults          # shared Serilog/OTel/health/auth defaults (L13, L25)
│  ├─ CareRoute.Portal (+ .Client)       # Blazor Web App: server host = BFF, client = WebAssembly
│  ├─ CareRoute.Reports                  # legacy-style Razor Pages app, SAML 2.0 SP (L26)
│  ├─ CareRoute.Intake.Api · CareRoute.Notifications.Worker · CareRoute.Functions
├─ identity
│  ├─ keycloak/realm-careroute.json      # realm as code (L21, L28)
│  ├─ keycloak/themes/careroute          # login theme (L28)
│  ├─ keycloak/spi/careroute-keycloak-extensions   # Java/Maven SPIs (L29)
│  ├─ keycloak/Dockerfile                # custom image with themes + SPIs (L29, L44)
│  └─ Northside.IdentityServer           # Duende IdentityServer partner IdP (L30)
├─ tests
│  ├─ CareRoute.Testing                  # Bogus fakers, fixtures, TestTokens, Keycloak fixture
│  ├─ CareRoute.Domain.Tests · .Application.Tests · .Api.IntegrationTests · .Portal.Tests
│  ├─ CareRoute.Security.Tests           # cross-cutting web-security/auth/OWASP test suite (L15 →)
│  └─ CareRoute.ArchitectureTests
├─ tools
│  ├─ CareRoute.PartnerLab.Client        # partner console app using client credentials (L24)
│  ├─ CareRoute.NationalRegistry.Fake    # external system, requires client credentials (L24, L41)
│  ├─ CareRoute.TokenInspector           # local JWT/SAML decoder — never paste tokens online (L17)
│  └─ CareRoute.LoadTest
├─ .github/workflows                     # ci.yml (L04), security.yml (L39), deploy.yml (L43)
└─ docs                                  # ubiquitous language, ADRs, threat model, runbooks, auth-troubleshooting guide
```

### 1.7 Business & access rules (fictional)

**Patient Registry** — 11-digit Belgian-style national number (YYMMDD + serial + mod-97 check; prefix `2` for 2000+ births), unique; one assigned GP per patient.
**Referrals** — `Draft → Submitted → InTriage → Accepted | Rejected`; Draft/Submitted can be `Withdrawn`; urgent triaged within 48 h; to submit: reason ≥ 20 chars, intake complete, draft ≤ 30 days old; only a specialist of the target department may triage/accept/reject; rejecting needs a reason.
**Scheduling** — 20 or 40 minutes on a 20-minute grid; no overlap per doctor; no booking in the past; < 24 h cancellation = late; accepted referral booked within 7 days (urgent) / 60 days (routine).
**Access** — GPs see only their own patients and referrals; specialists only their department's referrals and their own schedule; the partner lab may read only lab-order data it is scoped for; auditors read the access log only; **every read of patient data is audited**; administrators of Keycloak are not CareRoute users.

---

## 2. Schedule — 4 lessons a day, 13 days

| Day | 09:00–11:00 | 11:15–13:15 | 14:15–16:15 | 16:30–18:30 |
|---|---|---|---|---|
| **1** | L01 First API | L02 ASP.NET Core fundamentals | L03 REST API design | L04 Git & DevOps foundations |
| **2** | L05 Clean Architecture + tests | L06 Domain I | L07 Domain II | L08 Application layer |
| **3** | L09 SQL basics | L10 SQL for developers | L11 EF Core 10 | L12 EF Core + Testcontainers |
| **4** | L13 Observability | L14 Blazor portal | L15 Web security I | L16 Web security II |
| **5** | L17 Cryptography | L18 ASP.NET Core auth | L19 OAuth 2.0 | L20 OpenID Connect |
| **6** | L21 Keycloak fundamentals | L22 Securing the API | L23 Securing the portal (BFF) | L24 Service-to-service & partners |
| **7** | L25 Auth middleware in depth | L26 SAML 2.0 | L27 Troubleshooting auth | L28 Keycloak customisation I |
| **8** | L29 Keycloak SPI plugins | L30 Duende IdentityServer | L31 Ping, Entra & platforms | L32 Secure by design |
| **9** | L33 ★ Checkpoint 1 (mock interview) | L34 CQRS & events | L35 MongoDB Intake | L36 RabbitMQ |
| **10** | L37 Outbox & inbox | L38 Distributed tracing | L39 DevSecOps pipeline | L40 Performance |
| **11** | L41 Resilience & ACL | L42 ★ Checkpoint 2 | L43 Azure App Service & Key Vault | L44 Keycloak on Azure |
| **12** | L45 Application Insights | L46 Service Bus | L47 Durable Functions | L48 Logic Apps |
| **13** | L49 API Management | L50 ★ Final + interview simulation (double) | ← | — |

**Evenings:** 20 minutes max — handouts, weak spots, or `MOCK INTERVIEW <today's topic>`.

### Why this order
- **Git and CI on day 1 (L04)** so every later change is practised as a reviewed pull request.
- **Observability before identity (L13)** — you can't troubleshoot an auth flow you can't see.
- **The portal before identity (L14)** — OIDC login needs a real browser client; the portal is built as a Blazor Web App so the server host can become a **BFF** in L23.
- **Web security → crypto → ASP.NET Core auth → OAuth → OIDC → Keycloak:** each step only uses words defined in the step before. You first issue your own JWT (L18) so you understand exactly what a token is — and why production systems don't do that (L19).
- **Standards before products:** OAuth/OIDC/SAML first, then Keycloak, then IdentityServer and Ping — so you can switch identity platforms in an interview answer as easily as in configuration.
- **Troubleshooting (L27) after every protocol is built**, so drills use your own broken flows.
- **Customisation (L28–L29) after you know Keycloak well enough to know what *not* to customise.**
- **Everything after L33 is secured from day one** — services, messages, pipelines and Azure all reuse the identity platform.

---

## 3. Before Lesson 1 — setup (installers, no terminal)

1. **Visual Studio 2026**: workloads **ASP.NET and web development** and **Azure development**; components **.NET 10 Runtime (LTS)**, **SQL Server Express LocalDB**, **Git for Windows**, **Container development tools**.
2. **.NET 10 SDK** from Microsoft if `.NET 10.0` isn't offered in L01.
3. **GitHub account** (free) — needed from **L01** (push) and **L04** (PRs, Actions).
4. **Docker Desktop** (WSL 2) — needed from **L12**; Keycloak from **L21**.
5. **A second browser profile** (or a second browser) for testing logins as two different users — from L18.
6. **SAML-tracer** browser extension (or equivalent, verify) — L26.
7. **JDK 21** (Eclipse Temurin, verify the version Keycloak requires) and **IntelliJ IDEA Community Edition** — needed from **L28**.
8. **MongoDB Compass** — L35.
9. **Azure account** — from **L43** (budget alert set the same day).
10. Optional: **PingOne free trial** (verify availability) for the optional lab in L31.
11. In **Claude**: Web search, Artifacts, Code execution and file creation on.

---

## 4. What you'll be able to explain (interview map)

| Area | Basics | Under the hood | Trade-offs & failure modes |
|---|---|---|---|
| REST APIs | Resources, verbs, status codes, ProblemDetails, versioning, pagination | Idempotency, conditional requests, content negotiation | Chatty APIs, breaking changes, leaking internals |
| Web security | HTTPS, origins, cookies, CORS, OWASP Top 10 | TLS handshake, same-origin policy, cookie attributes | XSS, CSRF, SSRF, open redirects, misconfigured CORS |
| Cryptography | Hash vs encrypt vs sign, symmetric vs asymmetric, certificates | HMAC, RSA/ECDSA signatures, chain of trust, JWKS | Weak hashing, key leaks, expired certs, `alg` confusion |
| AuthN/AuthZ in ASP.NET Core | Schemes, handlers, claims, policies | Middleware order, challenge/forbid, handler pipeline | BOLA, open endpoints, claim mapping surprises |
| OAuth 2.0 | Roles, grants, scopes, tokens | Front vs back channel, PKCE, refresh rotation, introspection | Token leakage, scope creep, deprecated grants |
| OIDC | ID token, discovery, userinfo, logout | Nonce, state, `at_hash`, session management | ID token used as access token, broken logout |
| SAML 2.0 | IdP, SP, assertions, bindings, metadata | XML signatures, NameID, RelayState | Signature wrapping, clock skew, certificate rollover |
| Keycloak | Realms, clients, roles, scopes, mappers, flows, brokering | Token issuance, sessions, events, SPI provider model | Issuer/hostname mismatches, upgrade cadence, over-customisation |
| IdentityServer / Ping / Entra | Same standards, different products | Extension points (IProfileService, adapters, app registrations) | Licensing, lock-in, migration |
| Git & DevOps | Branches, PRs, CI/CD, environments | Merge vs rebase, runners, artifacts, OIDC federation to cloud | Broken main, secrets in history, unscanned dependencies |
| Observability | Serilog, correlation IDs, health checks, traces | `Activity`, W3C trace context, probes | PII in logs, missing correlation, bad probes |
| Azure | App Service, Key Vault, App Insights, Service Bus, Functions, Logic Apps, APIM | Managed identity, slots, durable replay, APIM policies | Cost, cold starts, misconfigured identity |

---

# PART A — FOUNDATIONS

## L01 — Start the project: Visual Studio 2026, .NET 10 & your first API
**Day 1 · Slot 1 · P01**
**📍 Start:** nothing. **🏁 End:** a `CareRoute` solution with a running API whose `PatientsController` returns fictional patients, tested from a `.http` file, pushed to GitHub.

### Concepts
🟢 **Core**
- .NET vs C# vs ASP.NET Core; SDK vs runtime; `net10.0`; **LTS** (to Nov 2028). ⚙️ C# → IL → JIT → host starts **Kestrel**.
- Solution (`.slnx`) vs project; `Program.cs`, `appsettings.json`, `launchSettings.json`.
- **HTTP**: method, URL, status code, headers, body. 📜 One raw request/response, line by line.
- **Controllers & actions**. ⚙️ `AddControllers()` discovers, `MapControllers()` maps; a new controller per request.

🔵 **Important**
- C# for today (class vs record, properties, `List<T>`, `var`, attributes, `=>`, interpolation). F5 vs Ctrl+F5, breakpoints. `.http` files, Endpoints Explorer, OpenAPI at `/openapi/v1.json`. Controllers vs minimal APIs. **HTTPS dev certificate** — what it is, why the browser trusts it (preview of L15).

🟣 C# 14 highlights · `dotnet run app.cs` · Scalar UI.

### ⚖️ 💣 ⬛
- ⚖️ Controllers vs minimal APIs; LTS vs STS.
- 💣 Controller not found → 404 empty body. 💣 Port in use / untrusted dev certificate. 💣 Different SDKs per machine (`global.json`, L04).
- ⬛ Run only LTS in production · pin the SDK · never disable HTTPS "to make it work".

### 🎬 Visuals
- 🎬 Pressing F5: build → Kestrel → request → routing → controller → JSON.

### 🛠️ Build
1. **Blank Solution** `CareRoute`. 2. **ASP.NET Core Web API** `CareRoute.Api` (.NET 10.0 LTS, Auth None, HTTPS ✓, OpenAPI ✓, **Use controllers ✓**). 3. Send the sample request; debug it. 4. `PatientsController` with `GET api/patients` and `GET api/patients/{id}` (404). 5. **Git > Create Git Repository** → push to GitHub (public or private — explain the difference for secrets).
6. **Connect the repo to the Claude Project**: Project knowledge > **+** > **GitHub** → pick the `CareRoute` repository → select `src/`, `tests/`, `docs/`, `CareRoute.slnx`, `.gitignore` (not `bin/`, `obj/`, `.vs/`). From now on, every lesson starts with a 🔎 Repo check of your real code; after every merged PR, press **Sync**.

### 🧪 Tests (Part 3)
Manual test plan in `CareRoute.Api.http` (unit tests start in L05): list → 200 (3 items); known id → 200; unknown → 404; malformed id → explain 404 vs 400. 💥 Remove `MapControllers()` → every request 404 → restore.

### 🏁 Commit `feat: create CareRoute API with PatientsController`

### 🎤 Explain-it & interview
1. SDK vs runtime? 2. What happens when a request reaches your controller? 3. Why LTS? 4. Controllers vs minimal APIs — what do you give up? 5. 🎤 *"Walk me through an HTTP request from the browser to your code."*

---

## L02 — ASP.NET Core fundamentals: pipeline, DI, configuration, validation, errors
**Day 1 · Slot 2 · P02**
**📍 Start:** hard-coded list. **🏁 End:** injected store, `POST` with automatic validation, options + `ValidateOnStart`, ProblemDetails, `ILogger`, `/health`.

### Concepts
🟢 **Core**
- **Middleware pipeline** (💡 airport checkpoints) — order is behaviour. ⚙️ Each middleware wraps the next. *This is where authentication and authorisation will plug in (L18) — remember the picture.*
- **Controller request flow** ⚙️: routing → controller (DI) → binding → validation → filters → action → result.
- **DI & lifetimes** (singleton, scoped, transient).
- **Model validation**: DataAnnotations on DTOs → automatic 400 ValidationProblemDetails.

🔵 **Important**
- Configuration layers & options pattern. `ILogger` templates (IDs only). `IExceptionHandler` + ProblemDetails — **never leak stack traces** (first security habit). `async`/`await`, `CancellationToken`. Filters vs middleware. Over-posting & DTOs. `MapHealthChecks("/health")`.

### ⚖️ 💣 ⬛
- ⚖️ Singleton vs scoped store; DataAnnotations vs FluentValidation (L08) vs domain rules (L06).
- 💣 Captive dependency. 💣 Middleware out of order. 💣 Over-posting (a client sets a field it shouldn't — a **security** bug). 💣 `.Result` → thread-pool starvation.
- ⬛ Bind DTOs only · ProblemDetails without internals outside Development · fail fast on bad config · structured logs without personal data.

### 🎬 Visuals
- 🎬 Request through the pipeline, exception caught on the way back. 🎬 Validation short-circuit. 🎬 Lifetimes across two requests.

### 🛠️ Build
1. DTOs. 2. `IPatientStore` + thread-safe in-memory store. 3. `POST api/patients` → 201 `CreatedAtAction`. 4. `Referrals` options with `ValidateOnStart`. 5. `ILogger`. 6. ProblemDetails + `IExceptionHandler`; `/health`.

### 🧪 Tests (Part 3)
Manual `.http` plan: valid POST → 201 + `Location`; missing field → 400; extra `id` in body ignored (over-posting); thrown exception → 500 without stack trace; bad config → startup failure. 💥 Swap to a plain `List<T>` singleton → explain the race → restore.

### 🏁 Commit `feat: DI, configuration, validation, ProblemDetails, health`

### 🎤 Explain-it & interview
1. Middleware and order? 2. What does `[ApiController]` do? 3. Lifetimes and captive dependency? 4. Over-posting? 5. 🎤 *"Where in the pipeline would you put a check that every request is authenticated, and why there?"*

---

## L03 — REST API design done right
**Day 1 · Slot 3 · P03**
**📍 Start:** a working but unplanned API. **🏁 End:** a reviewed REST design for CareRoute (`docs/api-guidelines.md`), resource-oriented routes, correct status codes, pagination, filtering, versioning (`v1`), idempotency for `PUT`/`DELETE`, ETags for concurrency (preview), a complete OpenAPI document.

### Concepts
🟢 **Core**
- **REST** (💡 a library catalogue: everything is a resource with an address) — resources, representations, uniform interface, statelessness. Nouns in URLs, verbs as HTTP methods.
- **HTTP methods & semantics**: safe, idempotent, cacheable; `GET`/`POST`/`PUT`/`PATCH`/`DELETE`; actions that aren't CRUD (`POST /referrals/{id}/submit`) — when it's fine.
- **Status codes as a contract**: 200/201/202/204, 400/401/403/404/409/412/415/422/429, 500/503 — and the security meaning of **401 vs 403 vs 404**.
- **Collections**: pagination (offset vs keyset/cursor), filtering, sorting, `Link`/metadata; never unbounded lists.

🔵 **Important**
- Versioning (URL vs header vs media type) — CareRoute uses `/api/v1/...`. ProblemDetails (RFC 9457) with a stable `type`/code. Content negotiation. ETag + `If-Match` → 412 (optimistic concurrency at the HTTP level). Idempotency keys for `POST` (implemented in L37). **API security basics**: no IDs that reveal counts (GUIDs), no sensitive data in URLs (they end up in logs), consistent errors. OpenAPI as a contract (APIM imports it in L49). HATEOAS (concept only).

### ⚖️ 💣 ⬛
- ⚖️ Offset vs keyset pagination. ⚖️ URL versioning (visible, simple) vs header (clean URLs, harder to test). ⚖️ Fine-grained vs coarse-grained resources (chatty vs over-fetching).
- 💣 Breaking change without a new version → partners fail. 💣 `GET` that changes state → crawlers/prefetchers trigger it. 💣 National number in the URL → in every proxy and server log. 💣 Unbounded list → timeouts and data exposure.
- ⬛ Document every endpoint in OpenAPI · version from day one · max page size enforced server-side · consistent ProblemDetails · never put secrets or personal data in URLs.

### 🎬 Visuals
- Diagram: CareRoute's resource tree (`/patients/{id}/referrals`, `/referrals/{id}/submit`, `/schedules/{doctorId}/{date}/appointments`). 🎬 ETag/If-Match → 412.

### 🛠️ Build
1. `docs/api-guidelines.md` (naming, status codes, errors, pagination, versioning, security rules).
2. Routes under `api/v1`; `GET api/v1/patients?page=&pageSize=&search=` with a `PagedResponse<T>`; max page size 100.
3. `PUT api/v1/patients/{id}` (idempotent) and `DELETE` (204, idempotent) on the in-memory store.
4. Consistent ProblemDetails `type` URIs and an `errorCode` extension.
5. OpenAPI: response types (`[ProducesResponseType]`), descriptions; review `/openapi/v1.json`.

### 🧪 Tests (Part 3)
Still manual (`.http`), written as a checklist in `docs/api-checklist.md`: page size 1000 → capped/400; `PUT` twice → same result; `DELETE` twice → 204 then 404 (explain why both are acceptable idempotent behaviour); unknown route → 404 ProblemDetails; wrong content type → 415. 💥 Add a `GET` that deletes → discuss why a link prefetcher could trigger it → remove.

### 🏁 Commit `feat: REST conventions, pagination, versioning, OpenAPI`

### 🎤 Explain-it & interview
1. What makes an API RESTful? 2. Safe vs idempotent? 3. 401 vs 403 vs 404? 4. Offset vs keyset pagination? 5. 🎤 *"How would you version a public API used by partner organisations, and how do you avoid breaking them?"*

---

## L04 — Git & DevOps foundations: branches, pull requests and CI
**Day 1 · Slot 4 · P04**
**📍 Start:** commits straight to `main`. **🏁 End:** a protected `main`, a branching workflow, PR template, `.gitignore`/`.gitattributes`/`global.json`, Conventional Commits, a GitHub Actions CI workflow that builds on every PR, Dependabot enabled.

### Concepts
🟢 **Core**
- **Git model** (💡 a photo album of the whole project: each commit is a snapshot pointing to its parent) ⚙️: working tree → staging (index) → commit → push; `HEAD`, branches as movable labels, remotes.
- **Branches, merge & rebase**: feature branch → PR → merge; merge commit vs squash vs rebase; resolving a conflict in Visual Studio's merge editor.
- **Pull requests & code review**: what reviewers look for (correctness, security, tests); small PRs; **branch protection** (require PR, require CI, no force-push).
- **DevOps & CI** (💡 an automatic inspector on every change): DevOps = culture + automation; **GitHub Actions** ⚙️: workflow (YAML) → trigger → job → runner (a fresh VM) → steps → logs.

🔵 **Important**
- `.gitignore` (never commit `bin/`, `obj/`, `*.user`, secrets); **a secret committed once stays in history** — rotate it, don't just delete. Conventional Commits. Trunk-based vs GitHub Flow vs GitFlow. Tags & releases. `global.json`. Dependabot for dependency updates (security!). Status checks. Signed commits (concept).

🟣 Interactive rebase · `git bisect` · monorepo vs multi-repo.

### ⚖️ 💣 ⬛
- ⚖️ Merge (true history) vs squash (clean main) vs rebase (linear, rewrites history). ⚖️ Long-lived branches (isolation) vs trunk-based (fewer conflicts, needs feature flags).
- 💣 Secret pushed to GitHub → leaked even after deletion. 💣 Force-push overwrites a colleague's work. 💣 "Works locally, fails in CI" (SDK version, OS paths, culture). 💣 Huge PR nobody really reviews.
- ⬛ Protect `main` · every change via PR with green CI · small PRs · secrets never in Git (secret scanning in L39) · pin the SDK · automate dependency updates.

### 🎬 Visuals
- 🎬 Commit graph: branch, commits, PR, merge vs squash vs rebase. 🎬 A CI run: push → runner → restore → build → ✅/❌ blocks the merge.

### 🛠️ Build (Visual Studio Git windows + GitHub website)
1. `.gitignore` (Visual Studio template), `.gitattributes`, `global.json` (SDK 10.0.x, `rollForward: latestFeature`).
2. `.github/pull_request_template.md` (what/why/how tested/security impact).
3. `.github/workflows/ci.yml`: on PR and push to `main` → checkout → setup .NET (from `global.json`) → restore → build `-warnaserror` (explain) → (tests from L05).
4. GitHub: **Settings > Branches** → protect `main` (require PR, require status check `ci`); **Settings > Code security** → Dependabot alerts + version updates (`dependabot.yml`).
5. Practise: branch `feature/l04-ci` → commit → PR → watch CI → merge (squash). Create and resolve a deliberate merge conflict.

### 🧪 Tests (Part 3)
The CI itself is the test today: a PR with a compile error is **blocked**; a PR with a warning is blocked (`-warnaserror`); direct push to `main` is refused. 💥 Commit a fake secret (`"ApiKey": "fake-123"`) on a branch → see that deleting it in the next commit leaves it in history → discuss rotation and history rewriting → close the branch.

### 🏁 PR `feat(ci): branch protection, CI workflow, Dependabot`

### 🎤 Explain-it & interview
1. What is a commit, really? 2. Merge vs rebase vs squash? 3. Why protect `main`? 4. What does a CI pipeline do? 5. 🎤 *"A developer pushed an API key to GitHub. What do you do, in order?"*

---

## L05 — Clean Architecture, the testing toolkit & architecture tests
**Day 2 · Slot 1 · P05**
**📍 Start:** one API project. **🏁 End:** four layer projects with correct references, ports & adapters, `CareRoute.Testing` with Bogus, first unit tests (xUnit + FluentAssertions + NSubstitute), **architecture tests** guarding the layers, tests running in CI.

### Concepts
🟢 **Core**
- **Clean Architecture** (💡 an onion) — Domain · Application · Infrastructure · Api; **dependency rule**. ⚙️ Project references enforce it.
- **Ports & adapters** and the **composition root**. *Later: `ICurrentUser` (a port) hides how the user was authenticated.*
- **xUnit + FluentAssertions 7.x + NSubstitute + Bogus**: AAA, `[Fact]`/`[Theory]`, readable assertions, substitutes for ports only, fixed seeds.
- **Architecture tests** (`NetArchTest.Rules` or `TngTech.ArchUnitNET.xUnit` — verify): the layer rules as executable tests.

🔵 **Important**
- Good tests; test doubles; licences (FluentAssertions 7.x pinned; AwesomeAssertions fallback); TDD exists, we test at the end.

### 🧩 DDD notes
- **Bounded contexts** as folders inside each layer; share IDs, not classes. **Ubiquitous language** → `docs/ubiquitous-language.md`.

### ⚖️ 💣 ⬛
- ⚖️ Clean Architecture vs CRUD ceremony; projects (enforced) vs folders (leak silently); mocks vs real objects; random data vs repeatability.
- 💣 Domain referencing EF Core. 💣 Over-mocking. 💣 Tests asserting nothing.
- ⬛ Run tests in CI on every PR · architecture rules as tests · fixed Bogus seeds · never mock what you own.

### 🎬 Visuals
- 🎬 The onion; an outward arrow turns red.

### 🛠️ Build
1. `CareRoute.Domain`, `.Application`, `.Infrastructure`; references.
2. Port `IPatientStore` (Application), adapter (Infrastructure), `AddApplication()`/`AddInfrastructure()`.
3. Test projects `CareRoute.Domain.Tests`, `CareRoute.Application.Tests`, `CareRoute.ArchitectureTests`; library `CareRoute.Testing`.
4. NuGet: FluentAssertions (latest **7.x**), NSubstitute, Bogus, architecture package.
5. `PatientFaker` (fixed seed). `docs/adr/ADR-001-clean-architecture.md`.
6. `ci.yml`: add `dotnet test` with results uploaded as an artifact.

### 🧪 Tests (Part 3)
Store: add/get, unknown → null, 100 concurrent adds. Service with NSubstitute: `AddAsync` received once. Faker: same seed → same patient. **Architecture:** Domain references nothing; Application doesn't reference Infrastructure or ASP.NET Core; Infrastructure doesn't reference Api. 💥 Add Domain → Infrastructure reference → architecture test red **locally and in the PR** → remove.

### 🏁 PR `feat: Clean Architecture skeleton, testing toolkit, architecture tests`

### 🎤 Explain-it & interview
1. Dependency rule? 2. Port, adapter, composition root? 3. What should you never substitute? 4. Why a fixed Bogus seed? 5. 🎤 *"How do you stop an architecture from eroding over two years and ten developers?"*

---

# PART B — CORE LOGIC

## L06 — Domain layer I: patients, value objects & invariants
**Day 2 · Slot 2 · P06**
**📍 Start:** `Patient` record in the API. **🏁 End:** rich `Patient` with `NationalNumber`, `PersonName`, typed IDs, `DomainException` → 409; a Bogus rule generating valid national numbers.

### Concepts
🟢 Entity vs value object · invariants & factory methods ("parse, don't validate") · `NationalNumber` (mod-97) · strongly typed IDs.
🔵 Exceptions vs Result · primitive obsession · private setters & `field` `[C# 14]` · **input validation is also a security control** (reject early, allow-list formats).

### 🧩 DDD notes
- Value object (`NationalNumber`, `PersonName`) · entity (`Patient`).

### ⚖️ 💣 ⬛
- ⚖️ Rich vs anemic; typed IDs vs mapping.
- 💣 Public setter bypasses invariants. 💣 Swapped `Guid`s. 💣 `default(PatientId)`.
- ⬛ National numbers are personal data — never logged, never in URLs, masked in UIs.

### 🎬 Visuals
- 🎬 Mod-97 check on `850730033` → `28`, then the 2000+ variant.

### 🛠️ Build
1. SharedKernel (`Entity<TId>`, `DomainException`, typed IDs). 2. PatientRegistry types. 3. API mapping + `DomainExceptionHandler` → 409. 4. Bogus `NationalNumber()` extension.

### 🧪 Tests (Part 3)
`NationalNumber` theory (valid 1985/2000, 10/12 digits, letters, wrong check, separators, birth date, equality) · `PersonName` · `Patient` · 1,000 Bogus numbers all valid · reflection: no public setters. 💥 Break mod-97 → red → restore.

### 🏁 PR `feat(domain): Patient, NationalNumber, typed IDs`

### 🎤 1. Entity vs value object? 2. Where are invariants enforced? 3. Why `Create`? 4. Typed IDs' weak spot? 5. 🎤 *"How do you make invalid states unrepresentable?"*

---

## L07 — Domain layer II: referral workflow, scheduling & domain events
**Day 2 · Slot 3 · P07**
**📍 Start:** `Patient`. **🏁 End:** `Referral` state machine, `DoctorSchedule` (no overlap), domain events, `TimeProvider`.

### Concepts
🟢 State machine in a class · `TimeProvider`/`FakeTimeProvider` · `TimeSlot` half-open intervals · domain events.
🔵 `BookingEligibilityService` · one transaction = one aggregate · **business authorisation rules belong here** ("only the target department's specialist may accept") — the domain receives *who* is acting as a parameter, it never reads tokens.

### 🧩 DDD notes
- Aggregate & root · aggregate size (doctor per day) · domain event.

### ⚖️ 💣 ⬛
- ⚖️ Big vs small aggregates.
- 💣 `DateTime.Now` in the domain. 💣 Two copies accept the same slot (L12). 💣 Events never dispatched (L34).
- ⬛ All times UTC `DateTimeOffset` · rules in one place · state changes only through named methods.

### 🎬 🎬 Referral state machine. 🎬 Two bookings racing.

### 🛠️ Build
1. Referrals types with `Accept(SpecialistId actingSpecialist, DepartmentId specialistDepartment)`. 2. Scheduling types. 3. Events on `AggregateRoot`. 4. Temporary controllers. 5. `ReferralBuilder`.

### 🧪 Tests (Part 3)
Every transition and guard, boundaries (19/20 chars, 30 days ± 1 tick), **accept by a specialist of another department fails**, TimeSlot cases, DoctorSchedule rules, eligibility service. 💥 `<` → `<=` in overlap → red → restore.

### 🏁 PR `feat(domain): referral workflow, scheduling, domain events`

### 🎤 1. Aggregate boundary? 2. Reference by ID? 3. Domain events — when dispatched? 4. Testing time? 5. 🎤 *"Where should an authorisation rule like 'only this department may accept' live — controller, handler or domain? Why?"*

---

## L08 — Application layer: MediatR use cases, FluentValidation & behaviors
**Day 2 · Slot 4 · P08**
**📍 Start:** orchestration in controllers. **🏁 End:** handlers per use case, validation and logging behaviors, thin controllers, an `ICurrentUser` **port** (returning a fixed dev user until L18).

### Concepts
🟢 Use case = load → domain → save · MediatR **12.5.0** · pipeline behaviors ⚙️ · thin controllers.
🔵 FluentValidation; three validation layers; feature folders; commands as an allow-list (anti mass-assignment); **`ICurrentUser` port** — Application asks "who is acting?" without knowing about tokens; an **authorisation behavior** slot reserved for L18.

### ⚖️ 💣 ⬛
- ⚖️ MediatR vs indirection.
- 💣 Fat controllers. 💣 Rules leaking into handlers. 💣 Behaviors out of order.
- ⬛ One handler per use case · no `HttpContext` in Application · validate every command.

### 🎬 🎬 Request → Logging → Validation → Handler; invalid request stopped.

### 🛠️ Build
1. MediatR 12.5.0, FluentValidation. 2. Use cases (`RegisterPatient`, `CreateReferral`, `SubmitReferral`, `GetReferralById`, `BookAppointment`). 3. Behaviors. 4. `ICurrentUser` + `DevCurrentUser` (Infrastructure, clearly marked ⚠️ DEV ONLY). 5. Thin controllers. 6. ADR-002.

### 🧪 Tests (Part 3)
Validators pass/fail per rule · handlers with NSubstitute repos and `ICurrentUser` · behavior order · architecture: Application has no ASP.NET Core reference. 💥 Swap behavior order → red → restore.

### 🏁 PR `feat(app): MediatR use cases, validation, ICurrentUser port`

### 🎤 1. Handler vs domain? 2. MediatR cost and licence? 3. Behaviors? 4. Validation layers? 5. 🎤 *"Why shouldn't your application layer read `HttpContext.User` directly?"*

---

# PART C — DATA

## L09 — SQL Server basics
**Day 3 · Slot 1 · P09**
**🏁 End:** `CareRouteDb` (LocalDB), schema per context, keys, constraints, sample data, queries.

### Concepts
🟢 Tables & types · PK/FK ⚙️ · constraints · SELECT/JOIN/GROUP BY.
🔵 Schemas per context · NULL logic · **SQL injection** & parameters (first hands-on attack — OWASP A03) · aggregate → tables · **least-privilege database users** (the app user can't `DROP TABLE`).

### 🧩 FKs inside a context, IDs across.

### ⚖️ 💣 ⬛
- ⚖️ GUID vs int; FKs across contexts.
- 💣 Injection. 💣 `= NULL`. 💣 Missing UNIQUE. 💣 Local time.
- ⬛ Parameterised queries only · app connects with a least-privilege login · UTC everywhere · named constraints.

### 🎬 🎬 JOINs. 🎬 Injection concatenated vs parameterised.

### 🛠️ Build
SQL Server Object Explorer → `CareRouteDb`; schemas + 7 tables; sample data; queries; a `careroute_app` login with only `SELECT/INSERT/UPDATE/DELETE` on the schemas (script explained).

### 🧪 Tests (Part 3)
`docs/database/004_checks.sql`: UQ, CHECKs, cascade, injection demo (all rows vs 0), `careroute_app` cannot `DROP`/`ALTER`. 💥 Drop the UNIQUE, insert a duplicate, fail to re-add → clean up.

### 🏁 PR `feat(db): schema, constraints, least-privilege login`

### 🎤 1. PK vs FK? 2. INNER vs LEFT? 3. Prevent injection? 4. `= NULL`? 5. 🎤 *"How would you limit the damage if your API's database credentials leaked?"*

---

## L10 — SQL Server for developers: indexes, plans, transactions, concurrency
**Day 3 · Slot 2 · P10**
**🏁 End:** ~100k rows, index with before/after numbers, transaction demo, `rowversion`, filtered unique index.

### Concepts
🟢 Index & B-tree ⚙️ · execution plans · transactions & ACID · optimistic concurrency (`rowversion`).
🔵 Covering/SARGable · isolation & deadlocks · parameter sniffing · defence in depth (domain + DB).

### ⚖️ 💣 ⬛
- ⚖️ Index read/write cost; optimistic vs pessimistic.
- 💣 Lost update. 💣 Deadlock. 💣 Scan from a function on a column.
- ⬛ Measure with real plans · short transactions · retry deadlock victims · concurrency tokens on aggregate roots.

### 🎬 🎬 Seek vs scan. 🎬 Deadlock. 🎬 rowversion conflict.

### 🛠️ Build
Generation script; measure; index; `BEGIN TRAN/ROLLBACK`; `RowVersion`; filtered unique index; blocking demo.

### 🧪 Tests (Part 3)
Scripted: same doctor/start refused; overlap accepted by SQL (domain still needed); rowversion changes; stale update → 0 rows. 💥 `CONVERT(date, …)` → scan → restore.

### 🏁 PR `perf(db): indexes, rowversion, double-booking guard`

### 🎤 1. Index cost? 2. ACID? 3. rowversion? 4. Deadlock? 5. 🎤 *"Two users save the same record at once. What happens and what should?"*

---

## L11 — EF Core 10 in the Infrastructure layer
**Day 3 · Slot 3 · P11**
**🏁 End:** `CareRouteDbContext` mapping the clean domain, repositories, migrations; API persists; connection string in **User Secrets** (first secrets habit).

### Concepts
🟢 ORM, `DbContext` (scoped) · mapping without attributes (converters, complex types `[EF10]`, backing fields) · migrations · repository per aggregate.
🔵 Change tracking · `AsNoTracking` · enums as strings · **User Secrets** ⚙️ (a JSON file outside the repo, Development only — not encryption).

### 🏛️ Only Infrastructure references EF Core.

### ⚖️ 💣 ⬛
- ⚖️ ORM vs SQL control; repositories vs DbContext in handlers.
- 💣 Shared DbContext across threads. 💣 Migration drift. 💣 Early `.ToList()`. 💣 Connection string with a password committed.
- ⬛ Secrets via User Secrets/Key Vault · migrations reviewed in PRs · never `EnsureCreated` in production.

### 🎬 🎬 LINQ → SQL → objects. 🎬 Change tracker.

### 🛠️ Build
EF packages 10.0.x; DbContext + configurations; **Manage User Secrets** for the connection string; `Add-Migration InitialCreate`, `Update-Database`; repositories.

### 🧪 Tests (Part 3)
Model tests: every aggregate mapped, converters present, `PersonName` → two columns. CI check: no `ConnectionStrings` with `Password=` in `appsettings*.json` (simple test scanning files). 💥 Remove a converter → red → restore.

### 🏁 PR `feat(infra): EF Core persistence, migrations, user secrets`

### 🎤 1. What does an ORM hide? 2. Why scoped? 3. Value objects without polluting the domain? 4. Repository per aggregate? 5. 🎤 *"Where do database credentials live in development, CI and production?"*

---

## L12 — EF Core in practice + Testcontainers & Bogus
**Day 3 · Slot 4 · P12**
**🏁 End:** `DoctorSchedule` persisted, projections (no N+1), concurrency → 409, unit of work, `CareRoute.Api.IntegrationTests` against **SQL Server in a container**, running in CI.

### Concepts
🟢 N+1 vs `Include` vs projection · concurrency token → 409 · unit of work · `WebApplicationFactory` + **Testcontainers** ⚙️ (lifecycle; Docker on GitHub's Ubuntu runners).
🔵 Data reset between tests (Respawn — verify) · no InMemory provider · split queries · named query filters `[EF10]`.

### ⚖️ 💣 ⬛
- ⚖️ Realistic containers vs speed (one container per collection).
- 💣 N+1. 💣 Concurrency → 500. 💣 Shared test state. 💣 Docker not running.
- ⬛ Integration tests against the real engine · run them in CI · cap result sizes.

### 🎬 🎬 N+1 counter. 🎬 Testcontainers lifecycle. 🎬 Two saves, one conflict.

### 🛠️ Build
Scheduling migration; N+1 → projection; concurrency tokens; `IUnitOfWork`; integration test project with `Testcontainers.MsSql`, `CareRouteApiFactory`, `DatabaseSeeder`; CI runs integration tests.

### 🧪 Tests (Part 3)
Round trip · duplicate national number → 409 · submit twice → 409 · overlap → 409, DB unchanged · stale version → 409 · 50 referrals = one query. 💥 Re-introduce N+1 → red → restore.

### 🏁 PR `test: Testcontainers integration tests, projections, concurrency`

### 🎤 1. N+1? 2. Two users, one aggregate? 3. Unit vs integration? 4. Why Testcontainers? 5. 🎤 *"How do you make integration tests reliable in CI?"*

---

# PART D — OBSERVABILITY & UI

## L13 — Observability: Serilog, structured logging, correlation IDs & health checks
**Day 4 · Slot 1 · P13**
**📍 Start:** default logging. **🏁 End:** Serilog (console, file, **Seq** in Docker), request logging, a correlation ID on every log and response, PII masking, `/health/live` + `/health/ready`, OpenTelemetry traces locally, all in a shared `CareRoute.ServiceDefaults` library.

### Concepts
🟢 Structured logging (💡 spreadsheet vs diary) · **Serilog** (sinks, enrichers, levels, request logging, bootstrap logger) · **correlation IDs** ⚙️ (`Activity`, W3C `traceparent`, `X-Correlation-Id`, `traceId` in ProblemDetails) · **health checks** (liveness vs readiness).
🔵 `LogContext`; **PII and secrets in logs** (masking policy; *never log tokens, Authorization headers, cookies or passwords* — the rule the identity track depends on); OpenTelemetry basics + local dashboard (Aspire dashboard or Jaeger — verify); **security events as structured logs** (`SecurityEvent` property: login failed, access denied) — preview of L32.

### ⚖️ 💣 ⬛
- ⚖️ More logs vs cost/privacy; sync vs async sinks; strict vs lenient readiness.
- 💣 `$"…"` interpolation → unqueryable. 💣 National number or bearer token in a log line. 💣 Lost correlation in background work. 💣 Liveness checking the DB.
- ⬛ Templates only · never log secrets/tokens/PII · correlation ID on every response · liveness without dependencies · flush on shutdown.

### 🎬 🎬 One correlation ID across 5 log lines. 🎬 Liveness vs readiness during a DB outage.

### 🛠️ Build
`CareRoute.ServiceDefaults` (`AddCareRouteObservability()`); Serilog packages (verify names); Seq container (verify image/licence env var); `CorrelationIdMiddleware`; masking policy; header redaction for `Authorization`/`Cookie` in request logging; health checks (`AspNetCore.HealthChecks.SqlServer` — verify); OTel.

### 🧪 Tests (Part 3)
Every response has `X-Correlation-Id`; incoming echoed; ProblemDetails has `traceId`; test sink: `PatientId` is a property, no national number, **no `Authorization` header value ever logged**; `/health/live` 200 and `/health/ready` 503 with DB container stopped. 💥 `$"…"` → red → restore.

### 🏁 PR `feat(obs): Serilog, correlation IDs, health checks, OpenTelemetry`

### 🎤 1. Structured vs plain logs? 2. How does a correlation ID reach every line? 3. Liveness vs readiness? 4. What must never be logged? 5. 🎤 *"A user says 'I got an error at 10:42'. How do you find exactly what happened?"*

---

## L14 — The Blazor Web App portal (WebAssembly interactivity + server host)
**Day 4 · Slot 2 · P14**
**📍 Start:** no UI. **🏁 End:** `CareRoute.Portal` as a **Blazor Web App** (server host project + `.Client` WebAssembly project): patient list, register form with server errors, referral detail with Submit, booking page, ErrorBoundary; the server host forwards `/api/*` to `CareRoute.Api` with **YARP** (so the browser talks to one origin — the BFF in L23 builds on this); bUnit tests.

### Concepts
🟢 Blazor render modes (Static SSR, Server, WebAssembly, Auto) and why CareRoute uses **WebAssembly interactivity with a server host** · components, parameters, events, binding, lifecycle · forms (`EditForm`, validation, showing 400/409 ProblemDetails) · **the server host as a same-origin proxy** (YARP) ⚙️ — no CORS needed, and the place tokens will live later.
🔵 Routing; state service with unsubscribe; typed API client; QuickGrid; **everything in the `.Client` project is downloaded and readable by anyone** — never secrets there; bUnit ⚙️.

### ⚖️ 💣 ⬛
- ⚖️ Same-origin proxy (no CORS, central security) vs browser calling APIs directly (CORS, tokens in the browser).
- 💣 Secrets in the WASM project. 💣 Forgotten unsubscribe. 💣 Double submit. 💣 UI hides a button but the API allows the action (fixed in L18).
- ⬛ No secrets client-side · the server enforces everything · friendly errors with the correlation ID for support.

### 🎬 🎬 How a WASM app loads. 🎬 Browser → portal host → YARP → API (one origin).

### 🛠️ Build
1. **Blazor Web App** template: interactive render mode **WebAssembly**, interactivity **per page/component** (verify option names), **Authentication type: None** (we add OIDC in L23).
2. `CareRoute.Contracts` for DTOs. 3. YARP in the host (`MapReverseProxy`, route `/api/{**catch-all}`). 4. Pages: patients, register, referral detail, booking. 5. ErrorBoundary + ProblemDetails mapping showing `traceId`. 6. `CareRoute.Portal.Tests` (bUnit).

### 🧪 Tests (Part 3)
bUnit: status badge per status; register form shows server field errors (fake client); ErrorBoundary shows trace ID. Integration: `/api/v1/patients` through the portal host reaches the API. Typed client: 400/409/500 mapping. 💥 Put a fake "secret" in `.Client/wwwroot/appsettings.json` → find it in the browser's Network tab → remove.

### 🏁 PR `feat(portal): Blazor Web App portal with same-origin API proxy`

### 🎤 1. Render modes? 2. Why a server host in front of WASM? 3. What's visible to the browser? 4. bUnit? 5. 🎤 *"Is hiding a button in the UI a security control?"*

---

# PART E — SECURITY & IDENTITY (the core of the course)

> This track assumes **zero prior knowledge**. Every lesson defines its vocabulary in 📖 boxes, traces the raw HTTP (📜), shows the exact error messages (💣), and ends with ⬛ production rules and an 🎤 interview drill. By L33 you should be able to whiteboard any flow below from memory.

## L15 — Web security I: HTTP, TLS & certificates, origins, cookies, CORS
**Day 4 · Slot 3 · P15**
**📍 Start:** an API and a portal over HTTPS you've never looked at. **🏁 End:** you can read the HTTPS connection, the certificate chain, every cookie attribute and every CORS header in CareRoute; HSTS and HTTPS redirection configured correctly; a `docs/security/web-security-basics.md` you wrote.

### Concepts
🟢 **Core**
- **Threats in one picture** (💡 a letter through the post): eavesdropping, tampering, impersonation → confidentiality, integrity, authenticity. CIA triad.
- **HTTPS/TLS** ⚙️: TLS handshake (client hello → certificate → key exchange → encrypted channel), what TLS protects and what it doesn't (it doesn't prove *who the user is*). TLS 1.2 vs 1.3.
- **Certificates & the chain of trust** (💡 a passport signed by a government your country recognises): subject, SAN, issuer, validity, root/intermediate CAs, the Windows certificate store, the ASP.NET Core dev certificate. Inspect CareRoute's certificate in the browser.
- **Origins & the same-origin policy** ⚙️: scheme + host + port; what the browser blocks and what it doesn't (it blocks *reading* responses, not *sending* requests — the root of CSRF in L16).
- **Cookies** ⚙️: `Set-Cookie`, `Domain`, `Path`, `Expires`, **`Secure`, `HttpOnly`, `SameSite` (Strict/Lax/None)**, `__Host-` prefix; session vs persistent; why `SameSite=Lax` is the default and why `None` needs `Secure`.

🔵 **Important**
- **CORS** ⚙️: simple vs preflight (`OPTIONS`), `Access-Control-Allow-Origin/Credentials/Headers/Methods`; CORS relaxes the same-origin policy — it never adds protection. **HSTS** & HTTPS redirection; mixed content; reverse proxies terminating TLS (`X-Forwarded-Proto` — it matters again in L22/L44). Ports and hostnames: `localhost` vs `127.0.0.1` are different origins.

🟣 Certificate transparency · OCSP · TLS cipher suites · HTTP/2 & 3.

### ⚖️ 💣 ⬛
- ⚖️ `SameSite=Strict` (strong CSRF protection, breaks some cross-site navigations) vs `Lax` vs `None` (needed for some federated flows — remember for SAML/OIDC).
- 💣 Expired or hostname-mismatched certificate → `NET::ERR_CERT_*` / `The remote certificate is invalid`. 💣 Disabling certificate validation in `HttpClient` "to make it work" → man-in-the-middle. 💣 `AllowAnyOrigin` + credentials. 💣 Cookie without `Secure` sent over HTTP. 💣 HSTS on a dev domain locks you out.
- ⬛ HTTPS everywhere, HSTS in production · never disable certificate validation · cookies `Secure; HttpOnly; SameSite=Lax` (or stricter) by default · explicit CORS allow-list · terminate TLS deliberately and forward headers correctly.

### 🎬 Visuals
- 🎬 TLS handshake. 🎬 Chain of trust from CareRoute's cert to a root. 🎬 Same-origin policy: request sent, response blocked. 🎬 CORS preflight.

### 🛠️ Build
1. Browser dev tools tour: **Network** (preserve log), **Security** tab (certificate), **Application > Cookies**. 📜 Read one full HTTPS exchange.
2. Windows **Manage user certificates** → find the ASP.NET Core dev certificate; explain trust.
3. API: `UseHttpsRedirection`, `UseHsts` (non-Development only, explain why), and a temporary CORS policy for a second origin (Endpoints Explorer/.http) to *see* preflight.
4. Portal host: a test cookie with and without `Secure/HttpOnly/SameSite` → observe in dev tools; JavaScript `document.cookie` can't read `HttpOnly`.
5. `docs/security/web-security-basics.md`: your own words for each term.

### 🧪 Tests (Part 3)
Integration (`CareRoute.Security.Tests` — new project): HTTP request → redirect to HTTPS; production environment sends `Strict-Transport-Security`; CORS preflight from an allowed origin → allowed headers, from another origin → no `Access-Control-Allow-Origin`; the test cookie is `HttpOnly`, `Secure`, `SameSite=Lax`. 💥 `AllowAnyOrigin().AllowCredentials()` → see the startup exception ASP.NET Core throws → restore.

### 🏁 PR `feat(security): HTTPS, HSTS, cookie defaults, CORS policy`

### 🎤 Explain-it & interview
1. What does TLS protect, and what doesn't it? 2. How does the browser decide to trust a certificate? 3. What's an origin? 4. Explain `HttpOnly`, `Secure`, `SameSite`. 5. 🎤 *"CORS is configured — does that protect our API from other websites?"* 6. 🎤 *"A service throws 'The remote certificate is invalid'. What do you check?"*

---

## L16 — Web security II: attacks & defences (OWASP Top 10)
**Day 4 · Slot 4 · P16**
**📍 Start:** you know the browser's rules. **🏁 End:** you've attacked a deliberately vulnerable page in a `Playground` project and fixed it; CareRoute has antiforgery on the portal host, output encoding checked, a **Content-Security-Policy**, security headers, open-redirect protection, SSRF protection on outbound calls, and an OWASP Top 10 map.

### Concepts
🟢 **Core**
- **OWASP Top 10 (web) & API Security Top 10** — what they are; CareRoute's map (verify current list editions).
- **XSS** (💡 someone slips a note into your letter that the reader obeys): stored, reflected, DOM; ⚙️ why Razor/Blazor encode by default and how `MarkupString`/`@Html.Raw` break it; **CSP** as a second wall; why XSS is fatal for tokens in the browser (→ BFF in L23).
- **CSRF** ⚙️: the browser sends cookies automatically → a malicious site triggers a state change; defences: `SameSite`, **antiforgery tokens**, no state change on `GET`; why bearer-token APIs are not CSRF-prone but cookie-based BFFs are.
- **Injection & broken access control** — recap of SQL injection (L09) and over-posting (L02); **BOLA/IDOR** preview (L18).

🔵 **Important**
- **Open redirect** (`returnUrl` abuse — critical for login flows: `LocalRedirect`, `Url.IsLocalUrl`). **SSRF** (the server fetches a URL an attacker chose — allow-list outbound hosts). Clickjacking (`frame-ancestors`). Security headers: CSP, `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`. Insecure deserialization; security misconfiguration; vulnerable dependencies (Dependabot from L04).

🟣 Subresource Integrity · Trusted Types · rate limiting login endpoints (L25).

### ⚖️ 💣 ⬛
- ⚖️ Strict CSP (strong XSS defence) vs breaking third-party scripts and Blazor needs (`wasm-unsafe-eval` — verify current Blazor CSP guidance).
- 💣 `MarkupString` with user input → stored XSS. 💣 State-changing `GET` → CSRF via an image tag. 💣 `Redirect(returnUrl)` → phishing via your own login page. 💣 "Fetch this URL for me" endpoint → reads cloud metadata (`169.254.169.254`).
- ⬛ Encode output, never trust input · CSP + security headers on every response · antiforgery on every cookie-authenticated state change · only local redirects after login · allow-list outbound URLs · keep dependencies patched.

### 🎬 Visuals
- 🎬 Stored XSS stealing a token from `localStorage` vs failing against an `HttpOnly` cookie. 🎬 CSRF with and without `SameSite`/antiforgery. 🎬 Open redirect after login.

### 🛠️ Build
1. `tools/Playground.VulnerableApp` (Razor Pages, localhost only, clearly labelled): XSS, CSRF, open redirect, SSRF — attack each in the browser, then fix each.
2. CareRoute portal host: antiforgery for state-changing requests (`AddAntiforgery`, Blazor's built-in antiforgery — verify), `LocalRedirect` helper for `returnUrl`.
3. `SecurityHeadersMiddleware` in `ServiceDefaults` (CSP tuned for Blazor WASM, `frame-ancestors 'none'`, `nosniff`, `Referrer-Policy`, `Permissions-Policy`).
4. Outbound `HttpClient` allow-list handler (for L24/L41).
5. `docs/security/owasp-map.md`.

### 🧪 Tests (Part 3)
Security tests: every response has the security headers and a CSP; a state-changing request without antiforgery token on the portal host → 400; `returnUrl=https://evil.example` → redirected to `/`; outbound handler refuses `http://169.254.169.254`; bUnit: a component renders `<script>` input as text. 💥 Replace `LocalRedirect` with `Redirect` → open-redirect test red → restore.

### 🏁 PR `feat(security): security headers, CSP, antiforgery, redirect & SSRF guards`

### 🎤 Explain-it & interview
1. XSS types and defences? 2. Why are cookie-based apps CSRF-prone and bearer APIs not? 3. What does CSP do? 4. What is SSRF? 5. 🎤 *"Why is an open redirect on a login page dangerous?"* 6. 🎤 *"Name three OWASP Top 10 risks and how your app defends against each."*

---

## L17 — Cryptography for identity: hashing, signatures, keys & certificates
**Day 5 · Slot 1 · P17**
**📍 Start:** you know TLS uses certificates. **🏁 End:** you can hash, HMAC, sign and verify in C#; generate an RSA/EC key and a self-signed certificate; read a JWK Set; protect data with ASP.NET Core **Data Protection**; and `tools/CareRoute.TokenInspector` decodes JWTs and SAML assertions **locally**.

### Concepts
🟢 **Core**
- **Encoding vs hashing vs encryption vs signing** (💡 translating vs fingerprinting vs locking vs sealing with wax). Base64/Base64url are **encoding — anyone can read them**.
- **Hashes & HMAC** ⚙️: SHA-256 properties; HMAC = hash + secret key → integrity + authenticity for whoever shares the key (HS256 JWTs).
- **Symmetric vs asymmetric** ⚙️: one shared key (AES, HMAC) vs key pairs (RSA, ECDSA); **signing with the private key, verifying with the public key** — the foundation of RS256/ES256 tokens and SAML signatures.
- **Keys & certificates for identity**: X.509 (public key + identity, signed by a CA), **JWK/JWKS** (public keys as JSON, `kid`), key rotation ⚙️ (publish new key, sign with it, keep old one until tokens expire).

🔵 **Important**
- **Password hashing** (PBKDF2, bcrypt, Argon2 — slow on purpose, salted; ASP.NET Core Identity's `PasswordHasher`). Secure randomness (`RandomNumberGenerator`) — for `state`, `nonce`, PKCE verifiers. **ASP.NET Core Data Protection** ⚙️ (key ring; why cookies break when it isn't shared across instances — again in L43). Never invent your own crypto; constant-time comparison. Key storage: never in code; Key Vault/HSM later.

🟣 Elliptic curves intuition · AEAD (AES-GCM) · post-quantum (awareness only).

### ⚖️ 💣 ⬛
- ⚖️ HS256 (simple, but every verifier can also *forge* tokens) vs RS256/ES256 (only the issuer can sign; anyone can verify — the right choice for multi-service systems).
- 💣 MD5/SHA-1 or plain SHA-256 for passwords → cracked offline. 💣 Signing key in `appsettings.json` → anyone with the repo forges tokens. 💣 `alg: none` / algorithm-confusion attacks when validation isn't strict. 💣 Data Protection keys not persisted → users logged out on every deploy. 💣 `kid` not found after rotation → `IDX10503`/`IDX10500` signature errors.
- ⬛ Asymmetric signing for tokens · pin allowed algorithms · rotate keys on a schedule with overlap · store keys in Key Vault/HSM · use framework crypto only · persist and protect Data Protection keys.

### 🎬 Visuals
- 🎬 Sign with private key → verify with public key; tampered payload fails. 🎬 Key rotation timeline with two keys in the JWKS. 🎬 Password hashing with salt vs rainbow table.

### 🛠️ Build
1. `tools/Playground.Crypto` console: SHA-256, HMAC, RSA/ECDSA sign/verify, a self-signed certificate (`CertificateRequest`), export public key as JWK.
2. `tools/CareRoute.TokenInspector` console: paste a JWT → header/payload pretty-printed, `exp`/`iat` as UTC, optional signature check against a JWKS URL; paste a SAML response (Base64) → decoded XML. ⚠️ Labelled: for fictional tokens only.
3. Data Protection in the portal host persisted to a folder (dev) — Key Vault in L43.
4. `docs/security/crypto-cheatsheet.md`.

### 🧪 Tests (Part 3)
HMAC changes when one byte changes; signature verification fails for a modified payload and for the wrong public key; TokenInspector parses a known JWT and reports `exp` correctly; Data Protection round trip; password hasher: same password → different hashes, both verify. 💥 Verify an RS256 token while accepting any algorithm → show why `ValidAlgorithms` matters → restore.

### 🏁 PR `feat(security): crypto playground, TokenInspector, data protection`

### 🎤 Explain-it & interview
1. Hashing vs encryption vs signing? 2. Why isn't a JWT encrypted? 3. HS256 vs RS256 — which for microservices and why? 4. How does key rotation work with JWKS? 5. 🎤 *"How should passwords be stored?"* 6. 🎤 *"What's in an X.509 certificate, and how is it validated?"*

---

## L18 — Authentication & authorisation in ASP.NET Core: the middleware, claims, policies & BOLA
**Day 5 · Slot 2 · P18**
**📍 Start:** everyone can do everything; `ICurrentUser` returns a fake user. **🏁 End:** the API validates JWTs (**issued by a temporary dev issuer you build yourself** — so you understand what a token server does), `ICurrentUser` reads claims, `GP`/`Specialist` policies, a fallback policy, resource-based authorisation, filtered queries, and a BOLA test suite.

### Concepts
🟢 **Core**
- **Authentication vs authorisation** (💡 badge at the entrance vs each ward door). **Identity, claims, `ClaimsPrincipal`, `ClaimsIdentity`.**
- **The ASP.NET Core auth middleware** ⚙️: `AddAuthentication` registers **schemes** → each scheme has a **handler** → `UseAuthentication` runs the default scheme's handler → `HttpContext.User` → `UseAuthorization` evaluates endpoint metadata → **challenge (401)** vs **forbid (403)**. Middleware order and why.
- **JWT bearer validation** ⚙️: signature (key/`kid`) → issuer → audience → lifetime (+ clock skew); `TokenValidationParameters`; claim type mapping surprises (`sub` → `nameidentifier`, `MapInboundClaims = false`).
- **Policies & resource-based authorisation**: requirements + handlers, `[Authorize(Policy=…)]`, fallback policy, `IAuthorizationService.AuthorizeAsync(user, resource, requirement)`; **BOLA** (OWASP API #1).

🔵 **Important**
- Role vs policy vs resource-based. Filter lists by user *and* check single resources. 403 vs 404 for other people's records. `ICurrentUser` (Infrastructure adapter reading `HttpContext.User`). An **authorisation pipeline behavior** in Application for commands marked `IRequireRole`. Why the dev issuer is ⚠️ DEV ONLY and what a real authorization server adds (L19).

### 🧩 DDD notes
- Access control (who may *see*) in Application; business permission (who may *accept*) also in the Domain.

### ⚖️ 💣 ⬛
- ⚖️ Self-contained JWT (no lookup) vs revocation → short lifetimes. ⚖️ 404 (hides existence) vs 403 (clearer).
- 💣 `UseAuthorization` before `UseAuthentication` → everyone anonymous. 💣 New controller without `[Authorize]` (fallback policy prevents). 💣 `IDX10223` lifetime expired / clock skew. 💣 Role claim not found because of claim-type mapping. 💣 Trusting a user ID from the request body.
- ⬛ Fallback policy = authenticated user · least privilege · authorise every object access · validate issuer/audience/lifetime/signature with pinned algorithms · never trust client-sent identity.

### 🎬 Visuals
- 🎬 One request through authN → authZ → resource check (200/401/403/404). 🎬 JWT validation check by check. 🎬 BOLA attack with and without the check.

### 🛠️ Build
1. `Microsoft.AspNetCore.Authentication.JwtBearer` (10.0.x); validation with explicit parameters (`ValidAlgorithms = RS256`, `MapInboundClaims = false`, small `ClockSkew`).
2. 🧪 `DevTokenController` (Development only, `[DevelopmentOnly]` filter → 404 elsewhere) signing RS256 tokens with a dev key from User Secrets; publishes its public key at a dev JWKS endpoint.
3. `HttpCurrentUser : ICurrentUser`; policies `GP`, `Specialist`; fallback policy; resource handlers for patient/referral/schedule; filtered list queries; `AuthorizationBehavior`.
4. Portal: temporary dev login (picks a seeded doctor) — replaced by OIDC in L23.

### 🧪 Tests (Part 3)
`CareRoute.Testing/TestTokens` (signs tokens in-process). No token → 401 · expired → 401 · wrong issuer → 401 · wrong audience → 401 · `alg: none` token → 401 · GP A reads GP B's patient → 404 · GP list never contains others' patients (Bogus 3×20) · GP can't accept → 403 · Dermatology specialist can't triage Cardiology → 403 (domain test too) · `/api/dev/token` → 404 in Production · reflection test: every action is authorised or explicitly `[AllowAnonymous]` from an allow-list. 💥 Remove the resource check → BOLA test red → restore.

### 🏁 PR `feat(auth): JWT validation, policies, resource-based authorisation, BOLA tests`

### 🎤 Explain-it & interview
1. Schemes, handlers, challenge, forbid? 2. What does JWT bearer validate, in order? 3. Role vs policy vs resource-based? 4. What is BOLA and how do you test it? 5. 🎤 *"Why shouldn't every API issue its own tokens?"* 6. 🎤 *"Users get 403 though their token has the role. What do you check?"* (claim type mapping, role claim name, policy, scheme)

---

## L19 — OAuth 2.0 from first principles
**Day 5 · Slot 3 · P19**
**📍 Start:** your own dev issuer. **🏁 End:** you can perform the **authorization code + PKCE** flow and the **client credentials** flow **by hand** with `.http` files and a browser against a real authorization server (Keycloak started in dev mode, configured minimally — L21 teaches it properly), read every parameter, and explain why other grants are deprecated.

### Concepts
🟢 **Core**
- **The problem OAuth solves** (💡 a valet key: lets the valet drive, not open the boot): delegated, limited access without sharing passwords. **OAuth is authorisation, not authentication.**
- **Roles**: resource owner, client, authorization server, resource server. **Endpoints**: authorize, token (+ discovery/metadata RFC 8414, revocation RFC 7009, introspection RFC 7662).
- **Authorization code + PKCE** 📜 (RFC 6749 + RFC 7636): front channel (browser redirect with `response_type`, `client_id`, `redirect_uri`, `scope`, `state`, `code_challenge`) → login → redirect back with `code` & `state` → back channel `POST /token` with `code_verifier` → tokens. Why `state` (CSRF), why PKCE (stolen code), why the code is single-use and short-lived.
- **Client credentials** 📜: machine-to-machine; confidential clients; client authentication (secret, `private_key_jwt`, mTLS).

🔵 **Important**
- **Access tokens** (JWT profile RFC 9068 vs opaque + introspection), **refresh tokens** and **rotation**, scopes vs roles, **audience**. Public vs confidential clients. **Deprecated**: implicit and resource-owner password grants — why. **OAuth 2.1** & the **Security BCP (RFC 9700)**. Device authorization grant (RFC 8628). Token exchange (RFC 8693) and DPoP (RFC 9449) — preview of L24.

🟣 PAR (RFC 9126) · RAR · FAPI 2.0.

### ⚖️ 💣 ⬛
- ⚖️ JWT access tokens (fast local validation, hard to revoke) vs opaque + introspection (revocable, network call). ⚖️ Long vs short access token lifetime.
- 💣 `invalid_grant` (code reused/expired, wrong `redirect_uri`, bad `code_verifier`). 💣 `invalid_client` (wrong secret/auth method). 💣 `redirect_uri` mismatch (exact match!). 💣 Missing `state` check → login CSRF. 💣 Tokens in URLs (implicit) → leaked via history/referrer/logs.
- ⬛ Code + PKCE for every client · exact redirect URIs · short-lived access tokens, rotated refresh tokens · least-privilege scopes · confidential clients authenticate strongly (prefer `private_key_jwt`/mTLS over shared secrets) · never use implicit or password grants.

### 🎬 Visuals
- 🎬 Auth code + PKCE: browser · client · authorization server · API, front vs back channel coloured, then a stolen-code attack failing because of PKCE. 🎬 Client credentials. 🎬 Refresh token rotation and reuse detection.

### 🛠️ Build
1. Docker Desktop: `quay.io/keycloak/keycloak:<pinned 26.x>` with `start-dev` (⚠️ DEV ONLY), one realm `oauth-lab`, one public client, one confidential client, one user (click paths given; full explanation in L21).
2. `docs/oauth/flows.http`: discovery document → build the authorize URL (PKCE verifier/challenge generated by a tiny helper in TokenInspector) → log in in the browser → copy the `code` from the redirect → token request → call the userinfo endpoint → refresh → revoke → introspect. Client credentials request.
3. Decode each token with TokenInspector; annotate claims (`iss`, `sub`, `aud`, `azp`, `scope`, `exp`, `iat`, `jti`).
4. `docs/oauth/glossary.md` in your own words.

### 🧪 Tests (Part 3)
`CareRoute.Security.Tests/OAuthLabTests` (Testcontainers.Keycloak with the exported lab realm — verify package): client credentials returns a token with expected `aud`/`scope`; wrong secret → `invalid_client`; code exchange with a wrong `code_verifier` → `invalid_grant` (code obtained via the direct-grant-disabled flow is hard to automate → test the token endpoint error contracts and document the browser part as a manual checklist); PKCE helper produces a correct S256 challenge for the RFC 7636 test vector. 💥 Reuse an authorization code → `invalid_grant` → explain why.

### 🏁 PR `docs(oauth): hand-run OAuth 2.0 flows and lab realm`

### 🎤 Explain-it & interview
1. The four roles? 2. Walk through auth code + PKCE with every parameter. 3. Why is the implicit grant deprecated? 4. JWT vs opaque access tokens? 5. 🎤 *"Is OAuth an authentication protocol?"* 6. 🎤 *"What does PKCE protect against, and why do confidential clients use it too?"* 7. 🎤 *"How would you revoke access for a stolen JWT?"*

---

## L20 — OpenID Connect: identity on top of OAuth 2.0
**Day 5 · Slot 4 · P20**
**📍 Start:** OAuth by hand. **🏁 End:** an OIDC login in a small `tools/Playground.OidcWeb` (Razor Pages) using ASP.NET Core's **OpenID Connect handler** against the lab realm — with cookie session, `nonce`/`state` validation you can see, userinfo, ID token claims and **RP-initiated logout**; a written comparison of ID token vs access token.

### Concepts
🟢 **Core**
- **OIDC = OAuth 2.0 + identity**: `openid` scope, **ID token** (who logged in, for the client) vs **access token** (what the client may do, for the API). Never send ID tokens to APIs.
- **ID token validation** ⚙️: signature, `iss`, `aud` = client ID, `exp`, **`nonce`** (replay), `azp`, `at_hash`/`c_hash`, `auth_time`, `acr`/`amr`.
- **Discovery & JWKS** 📜: `/.well-known/openid-configuration`, `jwks_uri`, how the .NET handler downloads and caches keys (and refreshes on unknown `kid`).
- **The ASP.NET Core OIDC handler** ⚙️: challenge → redirect → callback (`/signin-oidc`) → code redemption → claims → **cookie scheme signs the user in**; `SaveTokens`; events (`OnRedirectToIdentityProvider`, `OnTokenValidated`, `OnRemoteFailure`).

🔵 **Important**
- Standard claims & scopes (`profile`, `email`), userinfo endpoint. `prompt`, `max_age`, `login_hint`, `acr_values` (step-up/MFA). **Logout**: RP-initiated (`end_session_endpoint`, `id_token_hint`, `post_logout_redirect_uri`), front-channel, **back-channel logout** (server-to-server — best for BFFs). Session lifetime vs token lifetime. Hybrid flow (legacy).

### ⚖️ 💣 ⬛
- ⚖️ Claims in the ID token (fast, bigger cookie) vs userinfo (extra call, fresh). ⚖️ Front-channel vs back-channel logout reliability.
- 💣 "Correlation failed" (state cookie lost: `SameSite`, HTTP vs HTTPS, multiple instances without shared Data Protection). 💣 `IDX21323` nonce missing/invalid. 💣 Cookie too large → `431`/header too long (`SaveTokens` + many claims). 💣 Logout only clears the local cookie → still logged in at the IdP. 💣 ID token sent to an API as a bearer token.
- ⬛ Code + PKCE for OIDC login · validate `nonce`/`state` (handler does — don't disable) · keep cookies small · implement IdP logout (back-channel when possible) · `SameSite` and HTTPS correct · shared Data Protection across instances.

### 🎬 Visuals
- 🎬 OIDC login through the .NET handler: challenge → IdP → `/signin-oidc` → cookie → page. 🎬 RP-initiated and back-channel logout.

### 🛠️ Build
1. `tools/Playground.OidcWeb` (Razor Pages): `AddCookie` + `AddOpenIdConnect` (Authority = lab realm, `ResponseType = code`, `UsePkce = true`, `MapInboundClaims = false`, scopes, `SaveTokens`), a claims page, logout.
2. Log the handler events (redacting tokens) to Seq; correlate a login end to end.
3. Read the discovery document; watch the JWKS fetch in the logs.
4. `docs/oidc/id-vs-access-token.md`.

### 🧪 Tests (Part 3)
Unit: the OIDC options factory sets `code`, PKCE, `MapInboundClaims=false`, required scopes, `RequireHttpsMetadata` true outside Development. Integration (Testcontainers Keycloak): discovery document reachable and `issuer` matches the configured authority exactly; JWKS contains the `kid` of a freshly issued token. Manual checklist: login, claims page, logout clears IdP session (second login asks for password). 💥 Set `SameSite=Strict` on the correlation cookie → "Correlation failed" → explain → restore.

### 🏁 PR `feat(oidc): OIDC playground login and logout`

### 🎤 Explain-it & interview
1. OAuth vs OIDC? 2. ID token vs access token — who consumes each? 3. What's the nonce for? 4. How does the handler get the signing keys? 5. 🎤 *"Users see 'Correlation failed' after login. Walk me through your diagnosis."* 6. 🎤 *"How do you log a user out everywhere?"*

---

## L21 — Keycloak fundamentals
**Day 6 · Slot 1 · P21**
**📍 Start:** a throwaway lab realm. **🏁 End:** Keycloak (pinned 26.x) + **PostgreSQL** in Docker (Docker Compose file, explained), the `careroute` realm from §1.3 (roles, groups, users, clients, client scopes, audience mapper), password policy, brute-force detection, OTP for admins, token & session lifetimes chosen deliberately, events enabled, and the realm **exported to `identity/keycloak/realm-careroute.json`** (secrets removed).

### Concepts
🟢 **Core**
- **What Keycloak is** (💡 the hospital's badge office): an open-source (Apache 2.0) identity and access management server — OIDC/OAuth 2.0 and SAML 2.0 provider, user store, identity broker, user federation. **Realms** (tenants), master realm only for administration.
- **Clients** ⚙️: client types (OpenID Connect / SAML), **client authentication** on/off (confidential vs public), authentication flows allowed (standard flow, direct access grants — off!, service accounts), valid redirect URIs, web origins, PKCE method.
- **Users, groups, roles & scopes**: realm vs client roles, composite roles, groups as role carriers, **client scopes** and **protocol mappers** (what ends up in a token: `realm_access.roles`, audience mapper → `aud: careroute-api`, user attribute mapper).
- **Authentication flows & policies**: browser flow, required actions (verify email, configure OTP), password policy, **brute-force detection**, OTP/WebAuthn, sessions (SSO session idle/max), token lifespans, refresh token rotation ("revoke refresh token").

🔵 **Important**
- **Dev vs production mode** ⚙️ (`start-dev` vs `start`: H2 vs PostgreSQL, HTTP vs HTTPS, hostname settings `KC_HOSTNAME`, proxy headers, build-time vs runtime options, health/metrics endpoints). **Events** (login events, admin events) — the audit trail you'll troubleshoot with in L27. **Realm export/import** as config-as-code (secrets aren't in exports by default — verify). **Upgrade cadence**: no LTS, only latest patched. Keycloak Organizations (multi-tenancy) and Authorization Services (fine-grained, UMA) — awareness.

### ⚖️ 💣 ⬛
- ⚖️ Realm per tenant vs one realm with Organizations. ⚖️ Realm roles (simple, global) vs client roles (scoped per app). ⚖️ Longer sessions (convenience) vs shorter (exposure).
- 💣 Token issued with `iss = http://localhost:8080/...` but the API in Docker expects `http://keycloak:8080/...` → `IDX10205` issuer validation failed. 💣 Direct access grants left on → password grant usable. 💣 Wildcard redirect URI (`*`) → code stolen via open redirect. 💣 Admin console exposed to the internet with weak admin password. 💣 Running `start-dev` in production.
- ⬛ Pin an exact version and patch promptly · PostgreSQL, never H2 · fixed public hostname · exact redirect URIs · disable unused grants · brute-force detection + MFA for privileged users · admin console on a separate, restricted hostname · events on and shipped to logs · realm config versioned (no secrets).

### 🎬 Visuals
- Diagram: realm → clients → scopes → mappers → token. 🎬 What happens inside Keycloak when the portal logs in (flow → session → token with mappers).

### 🛠️ Build (🔑 Keycloak admin console + Docker Desktop)
1. `identity/docker-compose.yml`: `postgres` + `keycloak` (`start-dev` locally for now, but **with PostgreSQL**; hostname set; volume) — every line explained; start from Docker Desktop.
2. Realm `careroute`: roles, groups, 6 fictional users (GPs and specialists with `doctor_id`/`department_id` attributes; one auditor; one admin with OTP required).
3. Clients: `careroute-portal` (confidential, standard flow + PKCE S256, exact redirect URIs for the portal host), `careroute-api` (no flows — represents the audience), `careroute-worker` and `partner-lab` (service accounts only), client scopes `patients:read`, `referrals:write`, `directory:read`; audience mapper; user-attribute mappers (built-in for now; custom in L29).
4. Password policy, brute-force detection, token/session lifespans (document every number and why), refresh-token rotation, events + admin events on.
5. **Realm settings > Action > Partial export** → `realm-careroute.json`; review for secrets; commit. Import on container start (`--import-realm`).

### 🧪 Tests (Part 3)
`KeycloakFixture` in `CareRoute.Testing` (Testcontainers.Keycloak, imports the realm): discovery `issuer` is the configured one; `partner-lab` client credentials token has `aud: careroute-api`, only its scopes, no user claims; direct access grant for `careroute-portal` → `unauthorized_client`; a user token (via a test-only client with direct grant enabled **only inside the test realm variant** — ⚠️ labelled) contains `realm_access.roles` and `doctor_id`; 6 wrong passwords → user temporarily locked. 💥 Remove the audience mapper → the `aud` test goes red → restore.

### 🏁 PR `feat(identity): Keycloak realm as code with PostgreSQL`

### 🎤 Explain-it & interview
1. Realm, client, client scope, mapper? 2. Confidential vs public client in Keycloak? 3. How does a role get into a token? 4. Dev vs production mode? 5. 🎤 *"Our API rejects Keycloak tokens with an issuer error only when running in Docker. Why?"* 6. 🎤 *"How do you manage Keycloak configuration across dev, test and prod?"*

---

## L22 — Securing the API with Keycloak
**Day 6 · Slot 2 · P22**
**📍 Start:** the API trusts your dev issuer. **🏁 End:** the API trusts **Keycloak** through configuration only (`Authority`, `Audience`), maps Keycloak's role structure with an `IClaimsTransformation`, uses scopes *and* roles in policies, all L18 security tests now run against a real Keycloak in Testcontainers, and the dev issuer is deleted. *The Clean Architecture swap: Domain and Application didn't change.*

### Concepts
🟢 **Core**
- **Resource server configuration** ⚙️: `Authority` → discovery → JWKS (cached, refreshed on unknown `kid`); `Audience`/`ValidAudiences`; `ValidIssuer` must match **exactly** (hostname, scheme, path, trailing slash).
- **Claims transformation** ⚙️: Keycloak puts roles in a nested JSON claim `realm_access.roles` (and `resource_access.<client>.roles`); `IClaimsTransformation` flattens them into role claims; runs on every authentication — keep it cheap and idempotent.
- **Scopes vs roles in policies**: scope = what the *client* may do on the user's behalf; role = what the *user* may do; CareRoute requires both for writes (`referrals:write` **and** role `gp`).

🔵 **Important**
- Container networking: internal vs public issuer URL (`KC_HOSTNAME`, `KC_HOSTNAME_BACKCHANNEL_DYNAMIC` — verify names; `MetadataAddress` vs `Authority`). `RequireHttpsMetadata` (⚠️ off only in Development). Token lifetimes vs `ClockSkew`. Logging auth failures as structured security events (never the token). `IdentityModelEventSource.ShowPII` — Development only, why.

### ⚖️ 💣 ⬛
- ⚖️ Validating JWTs locally (fast, offline) vs introspection (revocation, network). ⚖️ Roles in tokens (stateless) vs looking up permissions in CareRoute's DB (fresh, but a call per request — cache).
- 💣 `IDX10205` issuer mismatch · `IDX10214` audience invalid · `IDX10500/10503` signature key not found (rotation, wrong realm) · `IDX10223` expired · roles missing because no claims transformation → everyone 403.
- ⬛ Configure trust via settings, not code · validate audience always · map claims explicitly · short `ClockSkew` · structured security logs · don't call Keycloak per request.

### 🎬 Visuals
- 🎬 First request after startup: API fetches discovery + JWKS, caches; later Keycloak rotates keys → unknown `kid` → refresh → success.

### 🛠️ Build
1. `Authentication` section in config (`Authority`, `Audience`, `RequireHttpsMetadata`).
2. `KeycloakRolesClaimsTransformation` (Infrastructure) + unit-testable parser.
3. Policies combining scope and role requirements; `ICurrentUser` reads `doctor_id`/`department_id` claims.
4. `JwtBearerEvents.OnAuthenticationFailed/OnChallenge` → structured security log with reason (no token).
5. Delete the dev issuer; update `.http` files to fetch a token from Keycloak (client credentials for API-only tests).

### 🧪 Tests (Part 3)
All L18 security tests rewired to `KeycloakFixture` tokens (real signatures) · token from another realm → 401 · token for another audience → 401 · token without `referrals:write` but with role `gp` → 403 · claims transformation: nested roles → role claims, idempotent when run twice · issuer with trailing slash difference → documents the failure. 💥 Remove the claims transformation → every role test 403 → restore.

### 🏁 PR `feat(auth): trust Keycloak; remove dev token issuer`

### 🎤 Explain-it & interview
1. How does the API get Keycloak's public keys? 2. What does claims transformation do here? 3. Scope vs role? 4. Name four `IDX` errors and their causes. 5. 🎤 *"How would you migrate an API from one identity provider to another with no downtime?"* (multiple issuers — L25)

---

## L23 — Securing the portal: OIDC login, the BFF pattern, sessions, refresh & logout
**Day 6 · Slot 3 · P23**
**📍 Start:** the portal has a dev login. **🏁 End:** the portal host is a **Backend-for-Frontend**: users log in with OIDC (code + PKCE, confidential client) against Keycloak; the browser holds only an `HttpOnly; Secure; SameSite` cookie; the host attaches access tokens when proxying `/api/*` with YARP; tokens refresh server-side; logout is RP-initiated and back-channel; antiforgery protects state changes; WASM components get the user's display info via an authentication state endpoint.

### Concepts
🟢 **Core**
- **Where tokens should live** ⚙️: SPA with tokens in the browser (exposed to XSS) vs **BFF** (tokens server-side; browser has a session cookie). Why the BFF is today's recommended pattern for browser apps (IETF *OAuth 2.0 for Browser-Based Apps* — verify status) — especially with health data.
- **Blazor Web App + OIDC** ⚙️ (Microsoft's BFF guidance for Blazor — verify current sample): `AddOpenIdConnect` + cookie on the host; `PersistentComponentState`/auth state serialization to the WASM client; YARP transform adding `Authorization: Bearer` from the saved tokens.
- **Token refresh & session lifetime**: refresh before expiry in the host (cookie `OnValidatePrincipal`), refresh token rotation, what happens when refresh fails (sign out).
- **Logout done right**: local cookie + Keycloak session (RP-initiated) + **back-channel logout** endpoint invalidating the server-side session.

🔵 **Important**
- Server-side session store for tokens (keep cookies small) — `ITicketStore` (in-memory now, distributed cache later). Antiforgery for the BFF's own endpoints. Duende BFF library as an alternative (licensing — verify). Step-up authentication with `acr_values` (e.g., MFA before viewing a patient's full record).

### ⚖️ 💣 ⬛
- ⚖️ BFF (strongest browser security, needs server and sticky/shared session) vs pure SPA with tokens (simpler hosting, XSS exposure).
- 💣 Cookie too big (`SaveTokens` with large tokens) → `431`. 💣 Refresh not implemented → users bounced after 5 minutes. 💣 Multiple instances without shared Data Protection/session store → "Correlation failed"/random logouts. 💣 Logout leaves the Keycloak session → silent re-login. 💣 Proxying `/api` without antiforgery → CSRF.
- ⬛ BFF for sensitive browser apps · tokens never reach JavaScript · server-side ticket store · refresh with rotation · back-channel logout · antiforgery on all cookie-authenticated mutations.

### 🎬 Visuals
- 🎬 BFF login → browser cookie → `/api/patients` → host adds bearer → API. 🎬 Token refresh in the host. 🎬 Same XSS attack: SPA loses tokens; BFF attacker can only act while the page is open.

### 🛠️ Build
1. Portal host: cookie + OIDC (confidential client secret from User Secrets), `MapInboundClaims=false`, scopes, `SaveTokens` into a server-side `ITicketStore`.
2. YARP transform adding the access token; 401 from the API → challenge.
3. Refresh logic; sign-out endpoint (RP-initiated); back-channel logout endpoint registered in the Keycloak client.
4. Auth state to WASM (`AuthenticationStateProvider` on both sides — verify current API); `AuthorizeView` for UI hints.
5. Remove the dev login; step-up demo for "view full patient record" with `acr_values` (optional).

### 🧪 Tests (Part 3)
Integration (portal host + Keycloak fixture): unauthenticated `/api/*` through the host → 401 (no redirect for API calls); authenticated session cookie flags correct; the browser-facing response never contains `access_token`; YARP adds `Authorization` header (test backend echoes it); expired access token is refreshed; back-channel logout request invalidates the session; mutation without antiforgery → 400. Playwright (optional — verify): full login click-through. 💥 Return tokens to the WASM client "for convenience" → the "no token in browser responses" test goes red → restore.

### 🏁 PR `feat(portal): OIDC login with BFF, refresh and logout`

### 🎤 Explain-it & interview
1. BFF vs tokens in the browser? 2. How does the host get tokens to the API? 3. How do you refresh tokens in a BFF? 4. RP-initiated vs back-channel logout? 5. 🎤 *"Design authentication for a SPA that shows patient records."* 6. 🎤 *"After deploying a second instance, users get randomly logged out. Why?"*

---

## L24 — Service-to-service & partner integrations across the organisation
**Day 6 · Slot 4 · P24**
**📍 Start:** only humans call the API. **🏁 End:** `tools/CareRoute.PartnerLab.Client` (a partner lab) calls a **partner API surface** with client credentials and narrow scopes; `CareRoute.Api` calls `tools/CareRoute.NationalRegistry.Fake` (which requires its own audience) with cached client-credentials tokens via a `DelegatingHandler`; **token exchange** demonstrated for "API calls a downstream API on behalf of the user"; introspection for the high-risk endpoint; an integration onboarding guide for other teams.

### Concepts
🟢 **Core**
- **Machine-to-machine with client credentials** ⚙️: token caching until shortly before expiry (`Duende.AccessTokenManagement` — verify licence, or a small hand-written cache), one client per calling system, scopes per capability, audience per API.
- **Delegation across services**: forwarding the user token (simple, broad audience) vs **token exchange** (RFC 8693, Keycloak standard token exchange — verify version/feature flag) vs client credentials (loses the user). Choosing per call.
- **Designing integrations for the wider organisation**: an integration contract (OpenAPI + scopes + rate limits + error model), onboarding checklist, credentials lifecycle (issue, rotate, revoke), environments, support with correlation IDs.

🔵 **Important**
- Client authentication options: shared secret, **`private_key_jwt`** (signed client assertion), **mTLS** & certificate-bound tokens (RFC 8705), DPoP (RFC 9449) — trade-offs. Introspection (RFC 7662) for revocation-sensitive endpoints, cached briefly. API keys: when they're acceptable (low-risk, combined with something else) — implemented as a custom scheme in L25. Webhooks with HMAC signatures (inbound partner callbacks).

### ⚖️ 💣 ⬛
- ⚖️ Token forwarding vs exchange vs client credentials. ⚖️ Shared secrets (easy) vs `private_key_jwt`/mTLS (no shared secret, more setup).
- 💣 Requesting a new token for every call → Keycloak overload. 💣 One "god" client shared by many systems → no revocation granularity, no audit. 💣 Forwarded user token accepted by every API (audience too broad). 💣 Client secret in a partner's Git repo → rotate; how you'd find out.
- ⬛ One client per integration · narrowest scopes and a dedicated audience · cache tokens · prefer asymmetric client auth · rotate credentials on a schedule · log the `azp`/`client_id` on every request · document the contract.

### 🎬 Visuals
- 🎬 Partner lab → Keycloak (client credentials) → CareRoute API (scope check). 🎬 User → portal → API → registry: forward vs exchange vs client credentials, showing the `aud`/`sub`/`act` claims each time.

### 🛠️ Build
1. Keycloak: clients `partner-lab` (scope `lab-orders:read`), `careroute-api-client` (to call the registry), `national-registry` audience; token exchange permission (verify setup).
2. API: `api/v1/partner/lab-orders` controller with `PartnerLab` policy (client token only — no user — and scope).
3. `ClientCredentialsTokenHandler` (`DelegatingHandler`, cached) for the registry `HttpClient`.
4. `tools/CareRoute.PartnerLab.Client` console using client credentials; `tools/CareRoute.NationalRegistry.Fake` (controllers, requires `aud: national-registry`).
5. Token exchange demo endpoint (optional path) + introspection on `DELETE api/v1/patients/{id}`.
6. `docs/integrations/onboarding-guide.md` (how another team or partner integrates with CareRoute).

### 🧪 Tests (Part 3)
Partner token can read lab orders, cannot read patients (403), cannot use user endpoints; user token can't call the partner endpoint; registry rejects a CareRoute **user** token (wrong audience) and accepts the client-credentials one; token handler requests one token for 50 calls and a new one after expiry (`FakeTimeProvider`); exchanged token has the expected `aud` and still identifies the user; introspection says inactive after revocation → 401. 💥 Disable caching → count token requests to Keycloak in the test → red → restore.

### 🏁 PR `feat(integration): client credentials, token exchange, partner API`

### 🎤 Explain-it & interview
1. How does a background service authenticate to an API? 2. Forward vs exchange vs client credentials? 3. `private_key_jwt` vs client secret vs mTLS? 4. When introspection? 5. 🎤 *"Another department wants to call our API. Walk me through onboarding them securely."* 6. 🎤 *"A partner's client secret leaked. What's your plan?"*

---

## L25 — Authentication & authorisation middleware in depth
**Day 7 · Slot 1 · P25**
**📍 Start:** you use the built-in handlers. **🏁 End:** you've **built and maintain** CareRoute's own auth building blocks in `CareRoute.ServiceDefaults`: a custom **API-key authentication handler** for a legacy partner, a **policy scheme** choosing between Keycloak, IdentityServer (L30) and API keys, custom requirements/handlers, an `IAuthorizationMiddlewareResultHandler` for consistent ProblemDetails, a security-context middleware, authentication rate limiting — all versioned, documented and tested like a library.

### Concepts
🟢 **Core**
- **Writing an `AuthenticationHandler<TOptions>`** ⚙️: `HandleAuthenticateAsync` (NoResult / Fail / Success with a ticket), `HandleChallengeAsync`, `HandleForbiddenAsync`; options & validation; constant-time key comparison; hashing stored keys.
- **Multiple schemes & policy schemes** ⚙️: `AddPolicyScheme` + `ForwardDefaultSelector` (peek at the token's `iss` to pick Keycloak vs IdentityServer), per-endpoint schemes, migrating identity providers without downtime.
- **Custom authorisation**: `IAuthorizationRequirement` + `AuthorizationHandler<TRequirement, TResource>`, `IAuthorizationPolicyProvider` for dynamic policies (`[RequireScope("referrals:write")]`), `IAuthorizationMiddlewareResultHandler` (uniform 401/403 ProblemDetails with `traceId`).
- **Custom middleware vs filter vs handler**: which concern goes where (authentication → handler; "is allowed?" → authorisation; cross-cutting context, e.g. a `SecurityContext` with tenant/client/user for logging → middleware after authentication).

🔵 **Important**
- `JwtBearerEvents`/`OpenIdConnectEvents` as extension points; `IClaimsTransformation` pitfalls (runs per request, may run twice). Rate limiting on auth-sensitive endpoints (`AddRateLimiter`, partition by client ID/IP). **Maintaining middleware**: semantic versioning, changelog, backwards compatibility, performance (no I/O per request without cache), observability (security events, metrics: auth failures by reason), test matrix.

### ⚖️ 💣 ⬛
- ⚖️ Custom handler (fits legacy needs) vs standard protocols (always preferred — API keys only where justified). ⚖️ Dynamic policy provider (flexible) vs explicit policies (discoverable).
- 💣 Handler returns `Fail` instead of `NoResult` when its header is absent → breaks other schemes. 💣 Policy scheme selector throws on malformed tokens → 500 instead of 401. 💣 API keys compared with `==` → timing leak; stored in plain text. 💣 Claims transformation calling a database per request → latency. 💣 Middleware placed before `UseAuthentication` → no user.
- ⬛ Prefer standard protocols; custom schemes only with a written justification · `NoResult` when not applicable · hash and rotate API keys · consistent 401/403 responses · metrics for auth failures · treat auth middleware as a versioned internal library with tests and docs.

### 🎬 Visuals
- 🎬 Policy scheme: request → selector peeks `iss` → forwards to Keycloak or IdentityServer or ApiKey handler → principal. 🎬 Requirement handlers voting (succeed/fail/neither).

### 🛠️ Build
1. `ApiKeyAuthenticationHandler` (header `X-Api-Key`, keys hashed in config/Key Vault later, client name claim, rate-limited) for `legacy-pharmacy`.
2. Policy scheme `CareRouteDefault` with a selector (Bearer + `iss` → Keycloak scheme or a placeholder `NorthsideIdp` scheme; `X-Api-Key` → API key scheme).
3. `[RequireScope]` attribute + dynamic policy provider + `ScopeAuthorizationHandler`.
4. `ProblemDetailsAuthorizationResultHandler`.
5. `SecurityContextMiddleware` pushing `ClientId`, `UserId`, `AuthScheme` into Serilog `LogContext`.
6. Rate limiter on partner endpoints and the BFF login endpoint.
7. `docs/security/auth-middleware.md` (design, extension points, versioning policy, CHANGELOG).

### 🧪 Tests (Part 3)
Handler unit tests with `TestServer`: missing header → `NoResult`; wrong key → 401 with ProblemDetails; correct key → principal with client claim; timing-safe comparison used (code review test via reflection or analyzer note). Selector: Keycloak token → Keycloak scheme; unknown issuer → 401 (not 500); garbage token → 401. Dynamic policy: missing scope → 403 ProblemDetails with `traceId`. Middleware: log events carry `ClientId`. Rate limit: 11th call → 429. 💥 Make the API-key handler return `Fail` when the header is missing → Keycloak-authenticated requests start failing → restore.

### 🏁 PR `feat(auth): custom API-key handler, policy scheme, dynamic policies, result handler`

### 🎤 Explain-it & interview
1. `NoResult` vs `Fail`? 2. How do you support two identity providers in one API? 3. When is a custom authentication handler justified? 4. Middleware vs filter vs authorisation handler? 5. 🎤 *"Walk me through how you'd build and maintain shared auth middleware used by twenty services."* 6. 🎤 *"After a change in our auth library, three services return 500 instead of 401. How do you investigate?"*

---

## L26 — SAML 2.0: assertions, bindings, metadata & federation
**Day 7 · Slot 2 · P26**
**📍 Start:** OIDC everywhere. **🏁 End:** `CareRoute.Reports` (a legacy-style Razor Pages app) signs users in with **SAML 2.0** using `Sustainsys.Saml2.AspNetCore2` against Keycloak as the **SAML IdP**; SP and IdP metadata exchanged; signed assertions; SP-initiated SSO and single logout; plus Keycloak **brokering an external SAML IdP** into OIDC for the portal (protocol bridging).

### Concepts
🟢 **Core**
- **SAML 2.0** (💡 a sealed, notarised letter of introduction): **IdP**, **SP**, **assertion** (subject/NameID, conditions — audience & time window, authentication statement, attribute statement), **signature** (XML-DSig), optional encryption.
- **Bindings & profiles** 📜: SP-initiated Web SSO — `AuthnRequest` via **HTTP-Redirect** (deflated, Base64, query string) → login → `Response` via **HTTP-POST** (auto-submitted form to the SP's **ACS** URL) with `RelayState`; IdP-initiated SSO and its risks.
- **Metadata** ⚙️: entity IDs, ACS URLs, signing certificates, NameID formats — exchanged between organisations; certificate rollover.
- **SAML vs OIDC**: XML vs JSON, browser POST vs redirects + back channel, enterprise legacy vs modern apps/APIs; brokering one into the other.

🔵 **Important**
- NameID formats (persistent, transient, email) and account linking. Single Logout (SLO) and why it's fragile. `InResponseTo` and replay protection. **XML Signature Wrapping (XSW)** — why you use a maintained library and never hand-parse assertions. `SameSite=None; Secure` on the SP cookies needed for the cross-site POST back.

### ⚖️ 💣 ⬛
- ⚖️ SAML (mature, enterprise IdPs everywhere) vs OIDC (simpler, API-friendly, mobile). ⚖️ IdP-initiated (convenient portals) vs SP-initiated (protected by `InResponseTo`).
- 💣 Signature validation fails after the IdP rotates its certificate (metadata not refreshed). 💣 `NotBefore`/`NotOnOrAfter` clock skew → assertion rejected. 💣 Audience/entity ID mismatch (trailing slash!). 💣 ACS URL mismatch behind a proxy (http vs https). 💣 Assertions accepted unsigned → impersonation. 💣 Cookie `SameSite=Lax` → POST binding loses the session.
- ⬛ Require signed assertions (and signed responses) · validate audience, recipient, time window, `InResponseTo` · automate metadata refresh and plan certificate rollover · prefer SP-initiated · prefer OIDC for new apps, bridge SAML at the IdP (broker) · never parse SAML by hand.

### 🎬 Visuals
- 🎬 SP-initiated SSO: Redirect binding → login → POST binding → ACS → cookie. 🎬 Keycloak brokering: portal (OIDC) → Keycloak → external SAML IdP → back as OIDC tokens. 🎬 Certificate rollover with two signing certificates in metadata.

### 🛠️ Build
1. Keycloak: SAML client `careroute-reports` (entity ID, ACS URL, signatures on, NameID persistent, role/attribute mappers); download IdP metadata.
2. `CareRoute.Reports` (Razor Pages, .NET 10) + `Sustainsys.Saml2.AspNetCore2` (verify version): SP entity ID, IdP from metadata URL, signing certificate for the SP (dev cert from L17), cookie `SameSite=None; Secure`; a reports page requiring role `auditor`.
3. SAML-tracer: capture and annotate one full login (AuthnRequest, Response, assertion) with TokenInspector.
4. SLO configured; test it.
5. Brokering: a second Keycloak realm `legacy-hospital` acting as a **SAML IdP**, added to the `careroute` realm as an **Identity Provider (SAML v2.0)** with mappers (first-broker-login flow) → a portal user can log in "with Legacy Hospital".
6. `docs/saml/saml-cheatsheet.md`.

### 🧪 Tests (Part 3)
Integration (Keycloak fixture + Reports via `WebApplicationFactory`): `/reports` unauthenticated → 302 to Keycloak with a SAML `AuthnRequest` containing the right `Issuer` and `AssertionConsumerServiceURL` (decode and assert); SP metadata endpoint publishes the expected entity ID and certificate; posting a **tampered** (modified attribute) SAML response → rejected; expired assertion (crafted with a test signing key and IdP metadata pointing to it) → rejected; non-auditor → 403. 💥 Turn off "require signed assertions" in the SP options → the tampered-response test goes red → restore.

### 🏁 PR `feat(saml): SAML 2.0 SP for Reports, Keycloak SAML IdP and broker`

### 🎤 Explain-it & interview
1. IdP, SP, assertion, ACS, entity ID? 2. Redirect vs POST binding? 3. What's in SAML metadata and why it matters for certificate rollover? 4. SAML vs OIDC — when each? 5. 🎤 *"A partner's SAML login started failing this morning with 'signature invalid'. What do you check first?"* 6. 🎤 *"How would you let a SAML-only enterprise IdP log in to an OIDC application?"*

---

## L27 — Troubleshooting authentication flows
**Day 7 · Slot 3 · P27**
**📍 Start:** OAuth, OIDC and SAML all work. **🏁 End:** a repeatable troubleshooting method, a toolbelt you know by heart, `docs/runbooks/auth-troubleshooting.md` (symptom → evidence → cause → fix for ~25 failures), auth dashboards/queries in Seq, and you've diagnosed **10 broken scenarios** injected into your own CareRoute.

### Concepts
🟢 **Core**
- **The method** (💡 a doctor's differential diagnosis): reproduce → locate the failing hop (browser ↔ client ↔ IdP ↔ API) → collect evidence at that hop → compare with the expected 📜 trace → one change → verify → write it down.
- **The toolbelt** ⚙️: browser dev tools (Network with *preserve log*, *disable cache*, cookies, redirects), **HAR files** (contain secrets/tokens — sanitise, never share raw), **SAML-tracer**, TokenInspector, Keycloak **login events & admin events**, Keycloak server logs, ASP.NET Core auth logs (`Microsoft.AspNetCore.Authentication` at Debug, JwtBearer/OIDC events), `IdentityModelEventSource.ShowPII` (⚠️ local only), Seq queries by correlation ID, `curl`/`.http` to discovery and JWKS, clock checks.
- **The failure catalogue** by hop: redirect URI mismatch · invalid client/secret · `invalid_grant` (code reuse, PKCE, expired, wrong redirect) · correlation failed · nonce invalid · issuer/audience/signature/lifetime `IDX` errors · claims missing (mapper/scope/claim mapping) · 401 vs 403 confusion · CORS on token calls · cookie `SameSite`/size · reverse proxy `X-Forwarded-*` (wrong scheme in `redirect_uri`) · Data Protection key ring · refresh failing after rotation reuse · logout not propagating · SAML signature/cert/clock/audience/ACS · clock drift.

🔵 **Important**
- Reading Keycloak event types (`LOGIN_ERROR` with `error=invalid_redirect_uri`, `CODE_TO_TOKEN_ERROR`…). Correlating Keycloak events with CareRoute logs (session/client/user IDs, timestamps). Production troubleshooting etiquette: no tokens in tickets, least-privilege access to logs, time-boxed debug logging.

### ⚖️ 💣 ⬛
- ⚖️ Verbose auth logging (fast diagnosis) vs exposure of personal data/tokens → targeted, temporary, redacted.
- 💣 Debug logging left on in production with PII. 💣 HAR file with tokens attached to a public ticket. 💣 "Fixing" by disabling validation (`ValidateIssuer = false`, `RequireHttpsMetadata = false`) in production.
- ⬛ Runbooks for the top auth failures · security events with reason codes and correlation IDs · NTP on every host · alerts on auth-failure spikes · never weaken validation to fix an incident.

### 🎬 Visuals
- 🎬 A decision tree: "Where does the flow stop?" → browser error page / IdP error page / app error / API 401 / API 403 → next check.

### 🛠️ Build
1. `docs/runbooks/auth-troubleshooting.md` template; Seq saved queries ("auth failures by reason", "logins by client").
2. Keycloak event listener to stdout → Seq (built-in jboss-logging listener, structured — verify config).
3. **Ten drills** (`tools/AuthDrills` — config toggles that break CareRoute on purpose): wrong redirect URI · wrong client secret · issuer hostname mismatch · audience mapper removed · clock skew (container time offset) · `SameSite=Strict` correlation cookie · proxy without forwarded headers · Data Protection not shared across two portal instances · SAML IdP certificate rotated · scope missing for an endpoint. For each: symptom → find evidence → fix → add the row to the runbook.

### 🧪 Tests (Part 3)
Regression tests for each drill that is automatable (issuer, audience, scope, clock skew, redirect URI via Keycloak admin API in the fixture); a test that production configuration never has `ShowPII`, `RequireHttpsMetadata=false` or `ValidateIssuer=false` (options validation at startup). 💥 Enable `ShowPII` with Production environment → startup validation fails → restore.

### 🏁 PR `docs(runbook): auth troubleshooting guide, drills, startup guards`

### 🎤 Explain-it & interview
1. Your troubleshooting method in five steps? 2. Which tool for which hop? 3. Why are HAR files dangerous? 4. How do you correlate Keycloak events with app logs? 5. 🎤 *"Users in one office can log in, users in another get 'invalid_grant'. Go."* 6. 🎤 *"Login works locally, fails behind the load balancer with a redirect_uri error. Why?"*

---

## L28 — Customising Keycloak I: Java primer, themes, Admin REST API & config as code
**Day 7 · Slot 4 · P28**
**📍 Start:** Keycloak configured by hand, exported once. **🏁 End:** a CareRoute **login theme**; a typed **Keycloak Admin REST API** client in C# (`Infrastructure/Identity/KeycloakAdminClient`) used to provision doctors when they're added to the Hospital Directory; realm configuration applied repeatably from Git; and enough **Java & Maven** (in IntelliJ) to write SPIs in L29.

### Concepts
🟢 **Core**
- **Java & Maven for C# developers** (💡 a sister language): JDK vs JRE (≈ SDK vs runtime), packages vs namespaces, classes/interfaces/annotations, `pom.xml` (≈ `.csproj` + NuGet), Maven lifecycle (`compile`, `test`, `package`), JUnit 5 (≈ xUnit). IntelliJ: open Maven project, Maven tool window, run tests.
- **Keycloak extension points overview**: themes (no code), Admin REST API (external), SPIs (in-process Java plugins — L29). Rule: **configure first, theme second, SPI last**.
- **Themes** ⚙️: theme types (login, account, email, admin), FreeMarker templates, `theme.properties`, messages/i18n, inheriting from the base theme, hot reload in dev (cache settings).
- **Admin REST API** ⚙️: a service-account client with **least-privilege realm-management roles** (`manage-users`, not `realm-admin`), token via client credentials, endpoints for users, groups, role mappings; idempotent provisioning.

🔵 **Important**
- **Configuration as code**: realm export/import vs `keycloak-config-cli` (verify) vs Terraform provider (verify) — pros/cons; no secrets in Git; environment-specific values. Keycloak's own Java Admin client vs hand-written C# client. Upgrade risk of theme templates (they change between versions — keep overrides minimal).

### ⚖️ 💣 ⬛
- ⚖️ Custom theme (branding, UX) vs upgrade cost. ⚖️ Provisioning via Admin API (control) vs user federation (L29, directory as source of truth).
- 💣 Overriding whole templates → broken login page after an upgrade. 💣 Admin client with `realm-admin` → a compromised CareRoute can take over the realm. 💣 Non-idempotent provisioning → duplicate users. 💣 Theme caching on in dev → "my change doesn't show".
- ⬛ Minimal theme overrides, diffed on every upgrade · least-privilege admin service account · idempotent provisioning with retries · config as code reviewed in PRs · admin API calls audited.

### 🎬 Visuals
- Diagram: Keycloak extension pyramid (config → theme → Admin API → SPI). 🎬 Doctor added in Directory → command handler → `KeycloakAdminClient` → user + group + attributes in Keycloak.

### 🛠️ Build (☕ IntelliJ + 🔑 console + Visual Studio)
1. IntelliJ: create a Maven project `java-primer` → a class, an interface, a JUnit test → run; compare with C# side by side.
2. `identity/keycloak/themes/careroute/login`: `theme.properties` (parent `keycloak.v2` — verify), CSS, logo, messages (EN/NL/FR), a small template override; mounted into the container; select it for the realm.
3. Keycloak client `careroute-provisioner` (service account, roles `manage-users`, `view-users` only).
4. `KeycloakAdminClient` (typed `HttpClient`, client credentials handler from L24) behind an Application port `IIdentityProvisioning`; `AddDoctor` use case provisions the user idempotently (search by `doctor_id` attribute first) and sends a required-action email (to a local mail catcher container — verify image).
5. Realm config pipeline: a CI job that validates `realm-careroute.json` is importable (start Keycloak container, import, smoke-check) — no secrets.

### 🧪 Tests (Part 3)
Java: JUnit test in the primer. C#: `KeycloakAdminClient` against the Keycloak fixture — creates user with attributes and group; second call doesn't duplicate; provisioner **cannot** create a client or change realm settings (403); theme served on the login page (HTML contains the CareRoute logo/message). 💥 Give the provisioner `realm-admin` → the "cannot change realm" test goes red → restore least privilege.

### 🏁 PR `feat(identity): login theme, Keycloak admin provisioning, realm config CI`

### 🎤 Explain-it & interview
1. Keycloak's extension options and when to use each? 2. How does a theme inherit? 3. What roles does an Admin API client need? 4. Config as code options for Keycloak? 5. 🎤 *"How do you keep Keycloak configuration consistent across environments?"* 6. 🎤 *"Maven vs NuGet/MSBuild — explain to a .NET colleague."*

---

## L29 — Customising Keycloak II: SPI plugins (provider development)
**Day 8 · Slot 1 · P29**
**📍 Start:** Java basics, themes, Admin API. **🏁 End:** `identity/keycloak/spi/careroute-keycloak-extensions` (Maven) with three providers — a **protocol mapper** `doctor-claims`, an **event listener** `careroute-audit` (sends login/admin events to CareRoute's audit endpoint), and an **authenticator** `licence-check` (blocks login if the doctor's licence is suspended in the Hospital Directory) — tested with JUnit + Testcontainers, packaged into a **custom Keycloak image** built in CI.

### Concepts
🟢 **Core**
- **The SPI model** ⚙️: Service Provider Interface = an interface Keycloak calls; a **`ProviderFactory`** (created once, `init`/`postInit`/`create`/`close`, `getId()`) creates **`Provider`** instances per session; registration via `META-INF/services/<factory-interface>`; `KeycloakSession` as the entry point (≈ a scoped DI container per request).
- **Protocol mapper SPI**: extending `AbstractOIDCProtocolMapper` + `OIDCAccessTokenMapper`/`OIDCIDTokenMapper`/`UserInfoTokenMapper`, configurable properties in the admin console.
- **Event listener SPI**: `EventListenerProviderFactory`/`EventListenerProvider` — `onEvent(Event)` and `onEvent(AdminEvent, boolean)`; transactions (send *after* commit), never block login on a slow sink.
- **Authenticator SPI**: `AuthenticatorFactory`/`Authenticator` — `authenticate`, `action`, `requiresUser`, `configuredFor`; adding it to a copied browser flow; `context.success()` / `failure()` / `challenge()` with a themed error page.

🔵 **Important**
- Packaging & deployment ⚙️: JAR in `/opt/keycloak/providers`, **`kc.sh build`** (optimised image), custom `Dockerfile` (multi-stage: Maven build → Keycloak builder → runtime). Dependencies inside providers (shade carefully). **Public vs private/internal SPIs** (internal ones can change between versions — verify which are supported). User Storage SPI (federate users from an existing database) and Required Action SPI — concepts and when to choose them. Calling external services from an SPI: timeouts, caching, failure policy (fail open vs fail closed).

### ⚖️ 💣 ⬛
- ⚖️ SPI (powerful, in-process) vs out-of-process options (Admin API, webhooks, token exchange, claims added by the API) — upgrade and stability cost. ⚖️ Fail-closed (secure) vs fail-open (available) when the licence service is down.
- 💣 Slow HTTP call inside an authenticator → every login hangs; Keycloak worker threads exhausted. 💣 Provider compiled against a different Keycloak version → `NoSuchMethodError` at startup. 💣 Event listener throwing → login errors. 💣 Mapper adds sensitive data (national number) to tokens. 💣 Forgetting `kc.sh build` → provider not loaded in optimised mode.
- ⬛ Customise only what configuration can't · pin Keycloak version in `pom.xml` = image version · timeouts and caching for every external call · decide fail-open/closed explicitly · never put personal data in tokens · integration-test providers in a real Keycloak container · rebuild and re-test on every Keycloak upgrade.

### 🎬 Visuals
- 🎬 Keycloak startup: scans providers → factories `init` → per-request providers via `KeycloakSession`. 🎬 Login with `licence-check`: username/password → authenticator → Directory call → success or themed "licence suspended" page.

### 🛠️ Build (☕ IntelliJ + Docker Desktop)
1. Maven project with `keycloak-server-spi`, `keycloak-server-spi-private`, `keycloak-services` as `provided` (version = pinned Keycloak — verify artifacts).
2. `DoctorClaimsProtocolMapper` (adds `doctor_id`, `department_id`, `role_scope` from user attributes; configurable claim names) → replace the built-in mappers from L21.
3. `CareRouteAuditEventListener` (POSTs a minimal event — type, client, user ID, IP hash, time — to `api/v1/audit/identity-events` using client credentials; async with bounded queue; never logs tokens).
4. `LicenceCheckAuthenticator` (calls `api/v1/directory/doctors/{id}/licence` with a 1-second timeout, short cache, **fail closed** with a clear message) + a copied browser flow using it.
5. `identity/keycloak/Dockerfile` (multi-stage) → `careroute/keycloak:<version>`; Compose uses it.
6. CI job: `mvn verify` + image build.

### 🧪 Tests (Part 3)
JUnit unit tests for mapper logic and the authenticator's decision function (with Mockito — the Java NSubstitute). Integration (`testcontainers-keycloak` for Java — verify artifact — loading the built JAR): a token contains `doctor_id`/`department_id`; login with a suspended licence fails, active succeeds; the event listener posts one event per login to a WireMock stub. .NET side: CareRoute's integration tests now use the custom image and still pass. 💥 Remove the timeout from the authenticator and stub a 10-second response → login test times out → restore.

### 🏁 PR `feat(keycloak): custom mapper, audit event listener, licence-check authenticator`

### 🎤 Explain-it & interview
1. What is an SPI, a ProviderFactory and a Provider? 2. How is a provider deployed? 3. Mapper vs authenticator vs event listener — which for what? 4. Fail-open vs fail-closed? 5. 🎤 *"Tell me about a Keycloak extension you built and the decisions behind it."* 6. 🎤 *"After upgrading Keycloak, it won't start: `NoSuchMethodError` in your provider. What happened and what's your process to avoid it?"*

---

## L30 — Duende IdentityServer & identity federation
**Day 8 · Slot 2 · P30**
**📍 Start:** Keycloak is CareRoute's only identity provider. **🏁 End:** `identity/Northside.IdentityServer` — a partner hospital's identity provider built with **Duende IdentityServer** and ASP.NET Core Identity (EF Core stores) — issues OIDC tokens; the `careroute` realm **brokers** it (`northside-hospital` IdP) so Northside doctors log in to the portal with their own credentials; and CareRoute.Api also accepts Northside tokens **directly** for one partner endpoint via the policy scheme from L25.

### Concepts
🟢 **Core**
- **IdentityServer as a framework** (💡 Keycloak is a ready-built office; IdentityServer is a kit to build your own): an ASP.NET Core app *you* host and code — clients, API scopes, API resources, identity resources, signing credentials, persisted grants.
- **Extension points** ⚙️: `IProfileService` (claims), `IResourceOwnerPasswordValidator` (legacy — why we don't), `IExtensionGrantValidator` (custom grants, e.g. token exchange), `ICustomTokenRequestValidator`, events, UI customisation (login/consent pages are your Razor Pages).
- **Identity brokering & federation** ⚙️: Keycloak as broker — first broker login flow, account linking, mappers from external claims to CareRoute attributes; **home-realm discovery** (`kc_idp_hint`). Trust boundaries: what CareRoute accepts from a partner and what it re-checks.

🔵 **Important**
- ASP.NET Core Identity basics (users, password hashing, lockout, 2FA). Key management (automatic key rotation in Duende — verify) and data protection. Licensing: **free for development/testing**, production licence needed (Community Edition for qualifying organisations — verify). OpenIddict as an open-source alternative (awareness). Keycloak vs IdentityServer decision factors.

### ⚖️ 💣 ⬛
- ⚖️ IdentityServer (full code control, .NET-native, you own security patches and UI) vs Keycloak (product, admin UI, Java SPIs). ⚖️ Brokering (one issuer for CareRoute) vs trusting partner tokens directly (fewer hops, more issuers to manage).
- 💣 Partner adds a claim like `role=admin` → blindly mapped → privilege escalation. 💣 Account linking by email → takeover if the partner doesn't verify emails. 💣 Signing key not persisted → tokens invalid after restart. 💣 Clock drift between IdPs.
- ⬛ Map only allow-listed claims from partners · link accounts by stable, verified identifiers · per-partner audience and scopes · monitor partner IdP metadata/keys · keep the IdentityServer and ASP.NET Core patched · follow the licence.

### 🎬 Visuals
- 🎬 Northside doctor → portal → Keycloak (`kc_idp_hint=northside-hospital`) → IdentityServer login → back to Keycloak → first-broker-login → CareRoute token. 🎬 Direct-trust path through the policy scheme.

### 🛠️ Build
1. Northside.IdentityServer from the Duende template (verify template name), ASP.NET Core Identity + EF Core (SQL Server container or LocalDB), seeded fictional Northside doctors.
2. Clients: `keycloak-broker` (confidential, code + PKCE) and `northside-referral-client` (client credentials for the direct partner endpoint); `IProfileService` adding `northside_staff_id` and `department`.
3. Keycloak: Identity Provider **OpenID Connect v1.0** `northside-hospital`, mappers (allow-listed claims → attributes, hard-coded role `gp` only for the `gps` group mapping), first-broker-login flow reviewed.
4. API: add the `NorthsideIdp` JWT scheme to the policy scheme; endpoint `api/v1/partner/referrals` accepting Northside client tokens with scope `northside.referrals`.
5. Portal: "Sign in with Northside Hospital" button using `kc_idp_hint`.
6. `docs/adr/ADR-00x-identity-federation.md`.

### 🧪 Tests (Part 3)
IdentityServer: discovery & token endpoint for `northside-referral-client`; `IProfileService` adds expected claims and nothing else. API: Northside token accepted on the partner endpoint, rejected on all others (403/401); Keycloak token rejected on the Northside-only endpoint. Broker (manual + automated where possible): Northside user ends up with role `gp` only via the mapper; an injected `role=admin` claim from Northside is **not** mapped. 💥 Map all partner claims to roles → the "admin claim ignored" test goes red → restore.

### 🏁 PR `feat(identity): Northside IdentityServer and Keycloak brokering`

### 🎤 Explain-it & interview
1. IdentityServer vs Keycloak — how would you choose? 2. Name three IdentityServer extension points. 3. What is identity brokering? 4. Account linking risks? 5. 🎤 *"A partner organisation wants their staff to use your application with their own logins. Design it."* 6. 🎤 *"How do you prevent a partner IdP from granting admin rights in your system?"*

---

## L31 — Identity platforms compared: Ping Identity, Microsoft Entra ID, choosing & migrating
**Day 8 · Slot 3 · P31**
**📍 Start:** two identity products in your hands. **🏁 End:** a concept map across **Keycloak · Duende IdentityServer · Ping Identity (PingFederate / PingOne) · Microsoft Entra ID**; CareRoute.Api proven **issuer-agnostic** (config switch to another OIDC provider passes the same tests); an optional PingOne trial lab; a migration playbook for moving an organisation between IdPs.

### Concepts
🟢 **Core**
- **Ping Identity** ⚙️ (concepts — verify product names): **PingFederate** (self-managed federation server: SP/IdP connections, OAuth clients, access token managers, adapters, authentication policies), **PingOne** (SaaS: environments, applications, populations, sign-on policies), **DaVinci** (orchestration flows), PingAccess/PingAuthorize (gateway/fine-grained authorisation). How each maps to Keycloak concepts.
- **Microsoft Entra ID** (awareness for the Azure part): tenants, app registrations, enterprise apps, app roles, `scp` vs `roles` claims, managed identities — and where CareRoute already uses it (Azure resources, L43).
- **Choosing an identity platform**: build (IdentityServer) vs self-host product (Keycloak) vs commercial (Ping) vs cloud (Entra, PingOne) — cost, compliance, data residency, customisation, operations, skills, lock-in.
- **Migrating between IdPs without downtime** ⚙️: dual-issuer acceptance (policy scheme), user migration (bulk import, password hash migration or lazy migration via user federation), client-by-client cut-over, stable subject identifiers, rollback plan.

🔵 **Important**
- Standards as the escape hatch (OIDC discovery, SAML metadata, SCIM for provisioning — RFC 7643/7644). Claims differences between platforms (role claim names, group claims overage). Support & patch models.

### ⚖️ 💣 ⬛
- ⚖️ Commercial (support, certified features) vs open source (control, people cost). ⚖️ SaaS (no ops) vs self-hosted (control, data residency).
- 💣 Subject identifiers change after migration → users lose their data. 💣 App hard-coded to one IdP's claim names. 💣 Big-bang migration without dual acceptance.
- ⬛ Code to standards, isolate product specifics in configuration/claims transformation · stable internal user ID mapped from external `sub` · SCIM for provisioning where available · migrations reversible and staged.

### 🎬 Visuals
- Table/diagram: one concept (client/application, scope, mapper/attribute contract, flow/policy, IdP connection) across the four platforms. 🎬 Staged migration with dual issuers.

### 🛠️ Build
1. `docs/identity/platform-comparison.md` (concept map + decision matrix for CareRoute).
2. Portability proof: an integration test collection parameterised by issuer configuration — Keycloak fixture and Northside IdentityServer — the same authorisation tests pass via configuration only.
3. **Optional PingOne lab** (trial — verify): create an environment, an OIDC application and a resource with a scope; configure an extra JWT scheme in CareRoute.Api from settings; call a test endpoint with a PingOne client-credentials token. If no trial: a guided walkthrough with screenshots-in-words and the equivalent Keycloak configuration.
4. `docs/identity/migration-playbook.md`.

### 🧪 Tests (Part 3)
Parameterised portability tests (Keycloak, IdentityServer) for 401/403/200 matrices; a claims-mapping test proving role claims from each issuer normalise to CareRoute roles; startup validation fails if two schemes share an issuer. 💥 Hard-code `realm_access` parsing into a policy → the IdentityServer run fails → move it back into the issuer-specific claims transformation.

### 🏁 PR `docs(identity): platform comparison, portability tests, migration playbook`

### 🎤 Explain-it & interview
1. Map Keycloak concepts to PingFederate/PingOne. 2. How does Entra ID represent app permissions? 3. Build vs buy for identity? 4. How do you migrate users' passwords? 5. 🎤 *"We use PingFederate. You know Keycloak. How quickly can you be productive and what's different?"* 6. 🎤 *"Plan a zero-downtime migration from one IdP to another."*

---

## L32 — Secure by design: threat modelling, audit, privacy & the production identity checklist
**Day 8 · Slot 4 · P32**
**📍 Start:** lots of security controls, no overall picture. **🏁 End:** a **STRIDE threat model** of CareRoute (`docs/security/threat-model.md`) with a data-flow diagram and trust boundaries; **audit of every patient read** (`AuditAccessBehavior` + identity events from L29 in one audit trail); log redaction verified end to end; secrets inventory; OWASP API Top 10 review; and the **production identity checklist** applied to CareRoute.

### Concepts
🟢 **Core**
- **Threat modelling** (💡 a building inspection before people move in): data-flow diagram, trust boundaries, **STRIDE** (Spoofing, Tampering, Repudiation, Information disclosure, Denial of service, Elevation of privilege), risk rating, mitigations, residual risk.
- **Audit & non-repudiation** ⚙️: who, what, when, from where, with which client, correlation ID; append-only storage; protecting the audit log itself; reads vs writes.
- **Privacy & data minimisation** for health data: least data in tokens, messages and logs; GDPR special-category data concepts (verify before quoting rules); retention.
- **Production identity checklist** (Appendix I): token lifetimes, key rotation, MFA for privileged users, admin console isolation, brute-force protection, monitoring/alerting on auth anomalies, backup/restore of the IdP database, upgrade cadence, incident playbooks (compromised account, leaked client secret, signing key compromise).

🔵 **Important**
- Secure SDLC: security requirements, threat modelling per feature, code review checklist, dependency hygiene, pen tests. Secrets inventory & rotation schedule. Security incident basics (detect, contain, eradicate, recover, learn).

### ⚖️ 💣 ⬛
- ⚖️ Audit everything (complete, large, sensitive) vs selective. ⚖️ Security vs usability (MFA everywhere, session lengths).
- 💣 Audit log containing personal data → a second breach target. 💣 No alert on 1,000 failed logins → credential stuffing unnoticed. 💣 Signing key compromise without a rotation plan.
- ⬛ Threat model updated with each major feature · audit trail append-only with restricted access · alerts on auth anomalies · documented and rehearsed key/secret rotation · MFA for all privileged identities.

### 🎬 Visuals
- Diagram: CareRoute DFD with trust boundaries (browser, portal host, API, Keycloak, partners, databases). 🎬 A specialist opens a patient → audit row + Keycloak login event joined by user and correlation ID.

### 🛠️ Build
1. `docs/security/threat-model.md` (Mermaid DFD + STRIDE table with mitigations already built and gaps).
2. `AccessLog` (migration) + `AuditAccessBehavior` for `ISensitiveQuery`; identity events from the SPI land in the same table; auditor-only query endpoint.
3. Redaction review: national numbers, names, tokens, cookies, `Authorization` headers — across Serilog, OTel attributes, ProblemDetails.
4. `docs/security/secrets-inventory.md` (what, where, owner, rotation).
5. Apply Appendix I to CareRoute; record gaps as GitHub issues labelled `security`.

### 🧪 Tests (Part 3)
Opening a patient writes exactly one audit row with user, client, correlation ID; a list query writes one row; auditor can read the audit log, GP cannot; no log/trace attribute contains a national number, full name or a JWT (scan the test sink for `eyJ`); body cannot set `Status`/`ReferringDoctorId` (mass assignment); OWASP API Top 10 checklist tests (BOLA, BFLA, excessive data exposure via response-shape snapshot). 💥 Add the patient's name to an audit message → the PII scan test goes red → restore.

### 🏁 PR `feat(security): threat model, access audit, redaction review, production checklist`

### 🎤 Explain-it & interview
1. Walk through STRIDE on one CareRoute flow. 2. What makes an audit trail trustworthy? 3. What's in your production identity checklist? 4. Signing key compromised — what now? 5. 🎤 *"How do you build security into the development lifecycle rather than bolt it on?"* 6. 🎤 *"What would you monitor to detect an attack on the login system?"*

---

## L33 — ★ Checkpoint 1: identity & security mock interview
**Day 9 · Slot 1**

| Part | Time | What Claude delivers |
|---|---|---|
| A. Whiteboard drills | 30 min | Draw from memory, out loud: auth code + PKCE · client credentials · OIDC login with the .NET handler · BFF with refresh · SAML SP-initiated POST · Keycloak brokering · token validation in the API. Then ⛔ reference diagrams. |
| B. Interview questions | 35 min | 25 questions in interviewer phrasing across web security, crypto, OAuth/OIDC/SAML, Keycloak (incl. SPIs), IdentityServer, Ping, middleware, Git/DevOps → ⛔ model answers + "red-flag answers" + scoring sheet. |
| C. Troubleshooting scenarios | 25 min | 5 incident stories ("Monday 08:00, every login returns `invalid_grant`…") → you list evidence and hypotheses → ⛔ walkthroughs. |
| D. Code review | 15 min | ~60 lines of auth code with ~8 flaws (`ValidateAudience = false`, `Redirect(returnUrl)`, token logged, `Fail` instead of `NoResult`, ID token used as access token, secret in config, no `state` check, claims mapped blindly) → ⛔ answers. |
| E. Your story | 15 min | Prepare 4 STAR stories from your Interview Story Bank (an SPI you built, a flow you debugged, a federation you designed, a security bug you prevented) and the honest framing of CareRoute as a learning project. |
| F. Catch-up | rest | `CATCHUP Lxx` for anything missing. Try `MOCK INTERVIEW identity` in the evening. |

---

# PART F — DISTRIBUTED SYSTEM (secured from day one)

## L34 — CQRS & dispatching domain events
**Day 9 · Slot 2 · P34**
**📍 Start:** events recorded, never dispatched. **🏁 End:** commands vs queries; `UnitOfWorkBehavior` saves then dispatches; referral timeline (who did what — from `ICurrentUser`); `ReferralAccepted` creates a `BookingRequest`.

### Concepts
🟢 CQS vs CQRS · commands by intent · dispatch after save ⚙️ · timeline as audit of changes.
🔵 CQRS levels · eventual consistency in the UI · idempotent handlers · queries also go through authorisation (a read model is not a security bypass).

### 🧩 Policy — contexts cooperate through events.

### ⚖️ 💣 ⬛
- ⚖️ Dispatch before vs after save (crash gap → outbox L37).
- 💣 Handler fails after save. 💣 Event loops. 💣 A read model exposing data the write side would deny (missing filter).
- ⬛ Same authorisation on read models · actor recorded on every change · idempotent handlers.

### 🎬 🎬 Command vs query path. 🎬 `ReferralAccepted` fan-out.

### 🛠️ Build
Markers; `UnitOfWorkBehavior`; `ReferralHistory` with actor ID; `GetReferralTimeline`; Scheduling handler; `AcceptReferral`/`StartTriage` commands with the domain authorisation from L07.

### 🧪 Tests (Part 3)
Behavior: save once, then publish, nothing published if save fails · timeline ordered with actor IDs · one `BookingRequest` per acceptance · timeline query filtered for a GP who doesn't own the referral → 404. 💥 Publish before save → red → restore.

### 🏁 PR `feat(app): CQRS, event dispatch, timeline`

### 🎤 1. CQRS vs CQS? 2. When dispatch? 3. Cooperation between contexts? 4. When is CQRS overkill? 5. 🎤 *"Can a read model become a security hole?"*

---

## L35 — Docker & MongoDB: the Clinical Intake service (secured)
**Day 9 · Slot 3 · P35**
**🏁 End:** MongoDB in Docker; `CareRoute.Intake.Api` with polymorphic intake forms, indexes, aggregation, version-based concurrency; **secured with Keycloak** (own audience `careroute-intake-api`, scopes) using the shared auth defaults from L25; the portal host proxies `/intake/*` with the user's token; API → Intake calls use **token exchange or client credentials** (decision recorded).

### Concepts
🟢 Docker image/container/port/volume · documents & collections, discriminators ⚙️ · queries, indexes, ESR, `explain` · aggregation pipeline.
🔵 MongoDB vs SQL · embed vs reference · version field → 409 · write concern · **MongoDB authentication & least-privilege users** (no anonymous DB in Docker either) · reusing `ServiceDefaults` (observability + auth) across services.

### 🧩 Bounded context as a service; one aggregate = one document.

### ⚖️ 💣 ⬛
- ⚖️ Document vs relational; separate service vs module.
- 💣 Container without volume. 💣 MongoDB exposed without auth. 💣 Unbounded documents. 💣 `MongoClient` per request. 💣 Intake accepting tokens meant for `careroute-api` (audience not checked).
- ⬛ DB auth always on, app user least privilege · each service validates its own audience · shared auth library, not copy-paste.

### 🎬 🎬 Image → container → volume. 🎬 Aggregation. 🎬 Portal → Intake with the right audience.

### 🛠️ Build
`mongo:8` with root + app users (Compose); Compass; Intake API (controllers) with `ServiceDefaults`; model, repository, controller, summary; Keycloak client + scopes; YARP route in the portal host; ADR on service-to-service delegation.

### 🧪 Tests (Part 3)
`Testcontainers.MongoDb`: discriminator round trip, filters, summary counts, version conflict → 409, indexes exist. Security: token with `aud: careroute-api` only → 401 at Intake; GP sees only intake forms of own referrals (resource check). 💥 Turn off audience validation → red → restore.

### 🏁 PR `feat(intake): MongoDB Intake service secured with Keycloak`

### 🎤 1. Container/image/volume? 2. When MongoDB? 3. Lost updates? 4. Why per-service audiences? 5. 🎤 *"How does a request carry the user's identity across two of your services?"*

---

## L36 — RabbitMQ & integration events behind `IEventBus`
**Day 9 · Slot 4 · P36**
**🏁 End:** RabbitMQ (TLS-ready, dedicated users per service, vhost); `IEventBus` port + RabbitMQ adapter; `ReferralSubmittedV1` published; `CareRoute.Notifications.Worker` consumes with correlation ID from headers; the Worker gets its own Keycloak client (client credentials) to call the API.

### Concepts
🟢 Why a broker · exchanges/queues/bindings/routing keys · `RabbitMQ.Client` 7 ⚙️ · `IEventBus` port.
🔵 Durable/persistent/acks/prefetch · **broker security**: per-service users and permissions, vhosts, TLS, no `guest` · **messages carry IDs, not personal data or tokens** · who published? (`app_id`, signed payloads concept) · RabbitMQ vs Kafka.

### 🧩 Domain vs integration event; published language in `CareRoute.Contracts`.

### ⚖️ 💣 ⬛
- ⚖️ Async messaging vs complexity; port abstraction vs hidden broker features.
- 💣 Domain event class published. 💣 Auto-ack + crash. 💣 Default `guest` user. 💣 Access token copied into a message "so the consumer can call the API" → long-lived token leak.
- ⬛ Least-privilege broker users per service · consumers authenticate to APIs with their own client credentials · no tokens or PII in messages · correlation ID in headers.

### 🎬 🎬 Exchange types. 🎬 Publish → ack.

### 🛠️ Build
`rabbitmq:4-management` with definitions file (users `careroute-api` publish-only, `notifications-worker` consume-only); contracts; port + adapter; translator; Worker (Serilog, correlation, health, Keycloak client credentials).

### 🧪 Tests (Part 3)
Translator: IDs only. `Testcontainers.RabbitMq`: publish → consume with headers; worker user cannot publish to `careroute.events` (permission test); message body contains no national number or `eyJ`. 💥 Publish the domain object → red → restore.

### 🏁 PR `feat(messaging): RabbitMQ integration events, Notifications worker`

### 🎤 1. Why a broker? 2. Domain vs integration event? 3. Exchange types? 4. Broker security? 5. 🎤 *"A consumer needs to call an API about the event's user. How does it authenticate?"*

---

## L37 — Reliable messaging: outbox, inbox, retries, dead letters & idempotency
**Day 10 · Slot 1 · P37**
**🏁 End:** publisher confirms, manual acks, delayed retry, parking lot; transactional outbox + relay; inbox in the Worker; `Idempotency-Key` filter for POSTs (scoped per authenticated client/user).

### Concepts
🟢 Failure modes · confirms/acks/DLX/retry · at-least-once · dual write → outbox · inbox · idempotency keys.
🔵 Quorum queues · relay scaling · outbox cleanup · contract versioning · **idempotency keys bound to the caller** (one client can't replay another's key) · queue-depth health.

### ⚖️ 💣 ⬛
- ⚖️ Confirms vs throughput; outbox latency; inbox lookup cost.
- 💣 Poison loop. 💣 Ack before work. 💣 Relay duplicate. 💣 Idempotency cache keyed only by the key → cross-user response leak.
- ⬛ Outbox for every integration event · idempotent consumers · DLQ monitored with a runbook · idempotency scoped to caller.

### 🎬 🎬 No-loss chain. 🎬 Retry → parking lot. 🎬 Outbox → relay → inbox.

### 🛠️ Build
Confirms; retry topology; `OutboxMessages` + relay; `ProcessedMessages`; `[Idempotent]` filter keyed by `client_id` + `sub` + key; DLQ runbook.

### 🧪 Tests (Part 3)
Outbox row in same transaction; relay marks sent; duplicate delivery → one effect; poison → parking lot after 3; same POST + key → one referral; **same key from another user → treated as new** (no leak). 💥 Stop RabbitMQ mid-flow → event still delivered.

### 🏁 PR `feat(messaging): outbox, inbox, idempotency keys`

### 🎤 1. Dual-write problem? 2. Outbox? 3. Duplicate publish? 4. DLQ ownership? 5. 🎤 *"How do you make a payment-like POST safe to retry?"*

---

## L38 — Distributed tracing across services
**Day 10 · Slot 2 · P38**
**🏁 End:** one trace from portal host → API → SQL → relay → RabbitMQ → Worker, and API → Intake; trace context through the outbox; **auth failures visible as span events**; Keycloak traces joined where supported (verify Keycloak OpenTelemetry support).

### Concepts
🟢 Trace/span ⚙️ · W3C propagation over HTTP (automatic) and broker (manual) · outbox gap · logs ↔ traces.
🔵 Correlation vs causation · sampling · **span attributes must never include tokens, cookies, emails, national numbers** (redaction processor) · tracing the BFF → API hop with the user's `sub` hashed, not raw.

### ⚖️ 💣 ⬛
- ⚖️ Visibility vs cost/PII.
- 💣 Lost context at the broker. 💣 `Authorization` header captured as a span attribute by an over-eager instrumentation option. 💣 100% sampling in production.
- ⬛ Redaction processor on all exporters · sampling policy · auth outcome recorded (reason only).

### 🎬 🎬 Trace waterfall across 4 services, then context lost.

### 🛠️ Build
Shared OTel config in `ServiceDefaults`; inject/extract `traceparent`; outbox stores it; redaction processor; span events for auth failures.

### 🧪 Tests (Part 3)
In-memory exporter: producer/consumer parent-child; outbox row has `traceparent`; consumer logs share `TraceId`; **no span attribute contains `eyJ` or `Bearer`**. 💥 Stop copying `traceparent` → red → restore.

### 🏁 PR `feat(obs): distributed tracing with redaction`

### 🎤 1. Context across HTTP/broker/outbox? 2. Trace ID vs correlation ID? 3. What must never be a span attribute? 4. Sampling? 5. 🎤 *"How would you trace a failed login all the way through your system?"*

---

# PART G — PRODUCTION

## L39 — DevSecOps pipeline & production readiness
**Day 10 · Slot 3 · P39**
**📍 Start:** CI builds and tests. **🏁 End:** a **DevSecOps pipeline**: secret scanning + push protection, **CodeQL** code scanning, dependency review & Dependabot, container image builds for every service (incl. the custom Keycloak image) with **image scanning** (Trivy — verify) and an **SBOM**, signed release tags, environments with required reviewers, migration bundles as artifacts; containers run as non-root with probes; Central Package Management; runbooks.

### Concepts
🟢 **Core**
- **DevSecOps** (💡 security checks on the assembly line, not at the end): shift left; the pipeline as a security control.
- **Pipeline stages** ⚙️: build → unit tests → integration tests (Testcontainers) → SAST (CodeQL) → dependency & licence checks → container build → image scan → SBOM → publish artifacts → (L43) deploy with approvals.
- **Supply chain security**: lock files & CPM, Dependabot, pinned action versions (by SHA), least-privilege `GITHUB_TOKEN` permissions, OIDC federation to clouds instead of stored secrets (used in L43), SBOM, signed tags/images (concept).
- **Production readiness**: Dockerfiles (multi-stage, non-root, port 8080), liveness/readiness, graceful shutdown, environment configuration, production logging, migrations from the pipeline.

🔵 **Important**
- GitHub environments, required reviewers, deployment protection rules. Branch rulesets. Secret scanning push protection. Infrastructure as code intro (Bicep — used optionally in L43). Release versioning & changelog. Keycloak image built and scanned like any other service.

### ⚖️ 💣 ⬛
- ⚖️ Blocking on every scanner finding (safe, noisy) vs triage policy (severity thresholds, expiry dates for exceptions).
- 💣 Third-party action pinned to `@main` → supply-chain attack. 💣 `permissions: write-all` → a compromised step can push code. 💣 Base image never rebuilt → months-old CVEs. 💣 Readiness checking too much → cascading restarts.
- ⬛ Least-privilege workflow tokens · actions pinned by SHA · scanners on every PR with a triage policy · images rebuilt regularly · no long-lived cloud secrets in CI · production deploys behind approvals.

### 🎬 🎬 The pipeline with a failing gate (CodeQL finding / critical CVE) stopping the merge. Diagram: multi-stage Dockerfile.

### 🛠️ Build (🐙 GitHub + Visual Studio)
1. `Directory.Build.props`, Central Package Management, `packages.lock.json`.
2. Dockerfiles for Api, Intake, Worker, Portal host, Reports (non-root, health checks); Compose for the full local stack.
3. `.github/workflows/security.yml`: CodeQL (C# and Java), dependency review, Trivy image scans, SBOM upload; `permissions:` minimal; actions pinned by SHA.
4. GitHub: secret scanning + push protection, rulesets requiring `ci` + `security` checks.
5. Environments `staging`, `production` with required reviewer (you).
6. Runbooks: release, incident, security incident (leaked secret, compromised account).

### 🧪 Tests (Part 3)
Pipeline gates: a PR adding a known-vulnerable package version is blocked; a PR with a hard-coded fake secret is blocked by push protection; an injected SQL-concatenation snippet is flagged by CodeQL (then removed); integration: production config has no dev token endpoints, ProblemDetails without details, startup guards from L27. 💥 Change a Dockerfile to run as root → a container-structure/Trivy misconfiguration check flags it → restore.

### 🏁 PR `ci(security): DevSecOps pipeline, images, SBOM, environments`

### 🎤 1. What is DevSecOps? 2. Pipeline stages and what each catches? 3. Supply-chain risks in CI? 4. Why pin actions by SHA? 5. 🎤 *"How would you design a CI/CD pipeline for a service handling health data?"* 6. 🎤 *"A critical CVE is announced in a base image. What happens in your process?"*

---

## L40 — Performance & caching
**Day 10 · Slot 4 · P40**
**🏁 End:** load-test tool, measurements, a fixed slow spot, HybridCache/output caching, per-client rate limiting; **auth performance**: JWKS caching, claims-transformation cost, token-acquisition caching measured.

### Concepts
🟢 p95/p99, measure → change → measure · HybridCache `[.NET 9+]` & output caching ⚙️ · thread pool · rate limiting.
🔵 **Caching and authorisation**: never cache per-user responses without the user/client in the key; output cache varies by auth; caching introspection results briefly. Pool limits. Accept-then-queue.

### ⚖️ 💣 ⬛
- ⚖️ Freshness vs speed; rate limits vs peaks.
- 💣 Stampede. 💣 Cache serving one GP's patients to another. 💣 Claims transformation hitting the DB on every request. 💣 Fetching a client-credentials token per call.
- ⬛ Measure first · cache shared reference data only · rate-limit by `client_id`/`sub` · auth path has no per-request I/O.

### 🎬 🎬 Stampede & coalescing. 🎬 Blocked vs free threads.

### 🛠️ Build
LoadTest tool; profiler; `.Result` fix; HybridCache for directory; `[OutputCache]` on public reference data; partitioned rate limiter; measure auth overhead (with/without cached JWKS and token handler).

### 🧪 Tests (Part 3)
429 with `Retry-After` per client; two clients separate limits; one DB hit for 50 concurrent directory requests; per-user endpoint never cached across users. 💥 Cache patients without the user key → red → restore.

### 🏁 PR `perf: caching, rate limiting, auth overhead measured`

### 🎤 1. Performance method? 2. p95? 3. Stampede? 4. Caching & privacy? 5. 🎤 *"Your API's latency doubled after adding authentication. Where do you look?"*

---

## L41 — Resilience & service boundaries
**Day 11 · Slot 1 · P41**
**🏁 End:** standard resilience on every outbound `HttpClient` (incl. token endpoint calls); an **anti-corruption layer** in front of the national registry from L24; `IntakeFormCompletedV1` removes the synchronous Submit → Intake call; optional YARP gateway; service-boundaries ADR.

### Concepts
🟢 Timeouts/retries/circuit breaker ⚙️ · `Microsoft.Extensions.Http.Resilience` · monolith vs modular monolith vs microservices; distributed monolith.
🔵 Retry storms, timeout budgets · **resilience for identity dependencies**: what happens when Keycloak is down (cached JWKS keeps validating; new logins and token refreshes fail → degrade gracefully, don't retry-storm the IdP) · service-to-service auth recap · strangler fig.

### 🧩 Anti-corruption layer; context map update.

### ⚖️ 💣 ⬛
- ⚖️ Microservices vs ops cost; retries vs load; ACL vs conformist.
- 💣 Cascading failure. 💣 Every service retrying the token endpoint during an IdP outage. 💣 Inner timeout > outer.
- ⬛ Timeouts everywhere · retries only for safe operations with jitter · circuit breakers on the IdP token client · plan for IdP outage (HA in L44).

### 🎬 🎬 Cascading failure vs contained. 🎬 Keycloak down: existing tokens still validate, refresh fails gracefully.

### 🛠️ Build
Resilience handlers; registry ACL (translator + port); `IntakeFormCompletedV1` consumer; optional `CareRoute.Gateway` (YARP); ADR.

### 🧪 Tests (Part 3)
Translator theory; slow registry → bounded failure; circuit opens; submit works with Intake stopped; **Keycloak container stopped → API still validates existing tokens (cached JWKS)**, token handler fails fast without hammering. 💥 Inner timeout > outer → red → restore.

### 🏁 PR `feat: resilience, ACL, intake-completed event`

### 🎤 1. Slow downstream? 2. Circuit breaker? 3. Dangerous retries? 4. ACL? 5. 🎤 *"Your identity provider is down for 10 minutes. What still works and what doesn't?"*

---

## L42 — ★ Checkpoint 2: review before the cloud
**Day 11 · Slot 2**

| Part | Time | What Claude delivers |
|---|---|---|
| A. Recap | 20 min | 🎬 Concept map L34–L41 with the security thread highlighted. |
| B. Self-test | 30 min | 20 questions (distributed + DevSecOps + identity in services) → ⛔ answers. |
| C. Failure drills | 35 min | "Duplicate notification", "trace stops at RabbitMQ", "Intake returns 401 for valid users", "pipeline blocked by a CVE", "Keycloak outage" → ⛔ walkthroughs. |
| D. Azure prep | 15 min | Map local → Azure (Appendix G); create the Azure account; check budget plans. |
| E. Catch-up | rest | `CATCHUP Lxx`. |

---

# PART H — AZURE

> ☁️ Every Azure lesson starts with the cost (cheapest working tier — verify prices) and ends with 🧹 cleanup. One resource group `rg-careroute-dev`, one region. **No secrets in GitHub**: deployments use OIDC federation; apps use managed identity.

## L43 — Azure basics, App Service, Key Vault, managed identity & CD
**Day 11 · Slot 3 · P43**
**🏁 End:** budget alert; Azure SQL with Entra authentication; Api and Portal host on **App Service** (Linux, .NET 10) with a staging slot; secrets (Keycloak client secret, Data Protection key protection, API-key hashes) in **Key Vault** via **managed identity**; Data Protection keys persisted to Blob Storage and protected with Key Vault; GitHub Actions deploying through **OIDC federation** with environment approvals, smoke tests, then swap.

### Concepts
🟢 Subscription/resource group/region/tier · **App Service** ⚙️ (plan, apps, slots, app settings, health check path, forwarded headers behind the front end) · **Managed identity** ⚙️ & Entra ID · **Key Vault** (RBAC, references vs config provider).
🔵 Azure SQL + Entra auth · **GitHub → Azure OIDC federation** (no publish profile, no client secret) · Data Protection in multi-instance hosting · slot swap warm-up · Cost Management.

### ⚖️ 💣 ⬛
- ⚖️ App Service vs Container Apps; Key Vault references vs provider.
- 💣 Identity without role → 403 from Key Vault, app won't start. 💣 Data Protection keys local per instance → random logouts (L23). 💣 Missing `ForwardedHeaders` → OIDC `redirect_uri` uses `http` → Keycloak rejects it. 💣 Forgotten resources.
- ⬛ Managed identity for every Azure resource access · OIDC federation for CI · secrets only in Key Vault · shared protected Data Protection keys · approvals on production · budget alerts.

### 🎬 🎬 Startup: identity → Key Vault → SQL. 🎬 Slot swap.

### 🛠️ Build
Budget; RG; Azure SQL; Key Vault; Storage (Data Protection); App Service plan + 2 web apps with system identity and role assignments; `ForwardedHeaders`; slot; `deploy.yml` with `azure/login` OIDC, migration bundle, deploy to staging, smoke tests, approval, swap. (Keycloak still local → Azure apps use L44's Keycloak; until then smoke tests use anonymous/health endpoints.) 🧹 Stop apps at day end.

### 🧪 Tests (Part 3)
Unit: Key Vault only outside Development. Smoke (CI against staging): `/health/ready` 200; `/api/v1/patients` → 401; dev endpoints 404; security headers present; HTTPS-only. 💥 Remove the Key Vault role → startup failure → Log stream → restore.

### 🏁 PR `feat(azure): App Service, Key Vault, OIDC-federated deployments`

### 🎤 1. Plan vs app? 2. Managed identity vs secret? 3. How does CI deploy without secrets? 4. Why share Data Protection keys? 5. 🎤 *"OIDC login works locally but Keycloak says invalid redirect_uri in Azure. Why?"*

---

## L44 — Keycloak in production on Azure
**Day 11 · Slot 4 · P44**
**📍 Start:** Keycloak runs only in Docker on your machine. **🏁 End:** the **custom CareRoute Keycloak image** (theme + SPIs) on **Azure Container Apps** in production mode (`start --optimized`), backed by **Azure Database for PostgreSQL Flexible Server**, with a fixed hostname over HTTPS, proxy headers configured, admin console restricted, secrets from Key Vault, health/metrics enabled, logs to Log Analytics, the realm imported from Git, and the Azure-hosted Api/Portal now trusting it.

### Concepts
🟢 **Production Keycloak** ⚙️: build-time vs runtime options, `--optimized`, hostname v2 options, `KC_PROXY_HEADERS`, TLS at the ingress, database connection pool, caches/clustering (Infinispan) and what HA requires (multiple replicas, session affinity during login flows — verify current guidance), health & metrics endpoints on the management port.
🟢 **Azure Container Apps** basics: environment, app, revisions, ingress, scale rules, secrets/Key Vault references, managed identity.
🔵 Admin console isolation (separate hostname/IP restrictions — verify Keycloak options), backups & point-in-time restore of PostgreSQL, upgrade procedure (new image → staging revision → realm smoke tests → traffic shift), signing-key rotation in production, monitoring login error rates and event volumes.

### ⚖️ 💣 ⬛
- ⚖️ Self-hosting Keycloak (control, ops burden) vs managed identity services (Entra, PingOne, Keycloak-as-a-service). ⚖️ One replica (cheap) vs HA (cost, clustering).
- 💣 Hostname not set → tokens issued with an internal hostname → every API rejects them. 💣 Proxy headers not trusted → redirect loops/HTTP URLs. 💣 Scaling to zero → cold start during login. 💣 Database firewall open to the internet. 💣 Upgrading Keycloak without rebuilding SPIs.
- ⬛ Fixed public hostname · TLS end to end where required · PostgreSQL private networking or tight firewall · min replicas ≥ 1 (≥ 2 for HA) · admin console not publicly reachable · backups tested · upgrade runbook with SPI rebuild · login error alerts.

### 🎬 🎬 Request path: user → Container Apps ingress (TLS) → Keycloak (proxy headers) → PostgreSQL; token `iss` = public hostname. 🎬 Upgrade via revisions.

### 🛠️ Build
PostgreSQL Flexible Server (cheapest burstable — verify); Container Apps environment + app from the CI-built image (push to Azure Container Registry with managed identity pull); env vars/secrets (Key Vault refs); realm import job; update client redirect URIs for Azure URLs; Api/Portal app settings point to the new authority; admin console restriction; diagnostic settings → Log Analytics. 🧹 Scale to the minimum or stop when done; PostgreSQL stop/start.

### 🧪 Tests (Part 3)
Smoke tests (CI, staging): discovery `issuer` equals the public hostname; JWKS reachable; client-credentials token works and is accepted by the Azure Api; admin console URL not reachable from the public endpoint (or requires the restricted path); login page shows the CareRoute theme; licence-check SPI loaded (`/realms/careroute` + a known login failure path). 💥 Remove the hostname setting → `iss` mismatch → Api 401s → find it with the L27 runbook → restore.

### 🏁 PR `feat(azure): production Keycloak on Container Apps with PostgreSQL`

### 🎤 1. Dev vs production Keycloak settings? 2. What does Keycloak need behind a reverse proxy? 3. HA considerations? 4. Upgrade process with custom SPIs? 5. 🎤 *"How would you run Keycloak in production?"*

---

## L45 — Application Insights: logs, traces, metrics & security alerts
**Day 12 · Slot 1 · P45**
**🏁 End:** workspace-based **Application Insights**; Azure Monitor OpenTelemetry Distro in every service; Serilog logs flowing in; Keycloak events in Log Analytics; KQL workbook with **auth dashboards** (failures by reason/client, login errors, 401/403 rates); availability test; alerts (failed requests, auth-failure spike, Keycloak login error spike).

### Concepts
🟢 App Insights on Log Analytics ⚙️ (`requests`, `dependencies`, `traces`, `exceptions`; `operation_Id` = trace ID) · `UseAzureMonitor()` ⚙️ · Serilog → App Insights (verify recommended path) · KQL basics.
🔵 Application map, transaction view; alerts & action groups; sampling, daily cap; **security monitoring** (credential stuffing patterns, unusual client activity) and its limits (it's not a SIEM — Microsoft Sentinel awareness).

### ⚖️ 💣 ⬛
- ⚖️ One place vs cost; OTel vs classic SDK.
- 💣 Duplicate pipelines. 💣 Missing connection string → silent. 💣 PII in `customDimensions`. 💣 Alert without action group.
- ⬛ Daily cap & sampling · redaction verified in the cloud · auth anomaly alerts · runbook link in every alert.

### 🎬 🎬 A request's journey into App Insights tables.

### 🛠️ Build
App Insights; distro in `ServiceDefaults`; Serilog wiring; Keycloak container logs → Log Analytics; KQL: failures by reason, p95 per endpoint, logs by `CorrelationId`, login errors by client; workbook; availability test; alerts. 🧹 Daily cap.

### 🧪 Tests (Part 3)
Unit: exporter only with connection string. Smoke (optional, slow): request with known correlation ID found via Azure Monitor Query. KQL saved queries checked into `docs/kql/`. 💥 Break the connection string → availability/alert reveals it → restore.

### 🏁 PR `feat(obs): Application Insights and auth monitoring`

### 🎤 1. `operation_Id`? 2. Distro vs SDK? 3. KQL for 401s by client? 4. Cost control? 5. 🎤 *"How would you detect a credential-stuffing attack?"*

---

## L46 — Azure Service Bus replaces RabbitMQ
**Day 12 · Slot 2 · P46**
**🏁 End:** Service Bus (Standard) topic + subscriptions with filters; `ServiceBusEventBus` adapter (config switch); managed identity with **Data Sender/Receiver** roles per service (no SAS keys); peek-lock consumer; DLQ handling.

### Concepts
🟢 Queues vs topics ⚙️ · peek-lock, lock renewal, delivery count, DLQ · the swap (only Infrastructure changes) · `Azure.Messaging.ServiceBus` + `DefaultAzureCredential`.
🔵 RabbitMQ ↔ Service Bus map · sessions · duplicate detection vs inbox · trace context in `ApplicationProperties` · **SAS keys vs Entra RBAC** (disable local auth — verify setting).

### ⚖️ 💣 ⬛
- ⚖️ Managed broker vs portability; duplicate detection vs inbox.
- 💣 Processing outlasts the lock → duplicates. 💣 Basic tier → no topics. 💣 SAS connection string with `Manage` rights in app settings.
- ⬛ Entra RBAC per service, local auth disabled · DLQ alert · inbox kept.

### 🎬 🎬 Peek-lock → crash → redelivery → DLQ.

### 🛠️ Build
Namespace; topic/subscriptions; role assignments; adapter; processor; health check; runbook. 🧹 Note Standard base cost.

### 🧪 Tests (Part 3)
Shared contract tests for both adapters; architecture: Domain/Application unchanged; optional live test (`Category=Azure`). 💥 Always-throwing consumer → DLQ in Service Bus Explorer → restore.

### 🏁 PR `feat(azure): Service Bus adapter with managed identity`

### 🎤 1. Queue vs topic? 2. Peek-lock? 3. What changed in the swap? 4. SAS vs RBAC? 5. 🎤 *"How do you authenticate services to a cloud message broker without secrets?"*

---

## L47 — Azure Functions & Durable Functions: booking deadlines
**Day 12 · Slot 3 · P47**
**🏁 End:** `CareRoute.Functions` (isolated worker, .NET 10 — verify) with a Service Bus trigger starting a **Durable orchestration** per accepted referral (instance ID = referral ID); durable timer (7/60 days) racing `AppointmentBooked`; urgent-triage 48 h escalation; escalation activity publishing via `IEventBus`; Function App with managed identity; behind the `IBookingDeadlineScheduler` port.

### Concepts
🟢 Functions triggers/bindings, hosting plans (verify Flex Consumption) · Durable orchestrator (deterministic, replay ⚙️), activities, durable timers, external events · the deadline pattern (`WhenAny`).
🔵 Orchestrator rules · storage backend (verify Durable Task Scheduler) · **securing Functions**: no function keys for internal triggers, Entra/Easy Auth or APIM in front of HTTP triggers, managed identity for outbound · hosted-service alternative (portable) vs Durable.

### 🏛️ Functions is an outer layer; the deadline *rule* comes from the Domain.

### ⚖️ 💣 ⬛
- ⚖️ Durable vs hosted service; Consumption vs Premium.
- 💣 Non-deterministic orchestrator code. 💣 Duplicate instance start. 💣 Event before orchestration. 💣 HTTP-triggered function left anonymous.
- ⬛ Deterministic orchestrators · instance ID = business ID · managed identity · no anonymous HTTP triggers.

### 🎬 🎬 Replay with a timer and external event. 🎬 Timeout → escalation.

### 🛠️ Build
Functions project; triggers; orchestrator; activities; adapter; publish to a Function App with identity + App Insights. 🧹 Storage cost note.

### 🧪 Tests (Part 3)
Orchestrator with substituted context (verify mockability): booked first → no escalation; timer first → one escalation; urgent 7 vs routine 60; deadline from `CurrentUtcDateTime`; trigger idempotent on duplicate start; architecture rules. 💥 `DateTime.UtcNow` in orchestrator → red → restore.

### 🏁 PR `feat(azure): Durable Functions booking deadlines`

### 🎤 1. Trigger vs binding? 2. Why deterministic? 3. Durable timer vs `Task.Delay`? 4. The port swap? 5. 🎤 *"How do you secure an Azure Function?"*

---

## L48 — Logic Apps: notification workflow
**Day 12 · Slot 4 · P48**
**🏁 End:** Logic App (Consumption) triggered by the `notifications` subscription (peek-lock, managed identity connection — verify connector support), Parse JSON with the contract schema, calls a CareRoute endpoint **with managed identity** (Entra-issued token, validated by the Api as a second trusted issuer for this one endpoint), sends a minimal email, completes or dead-letters; workflow JSON in Git.

### Concepts
🟢 Triggers/actions/connectors · workflow definition & run history ⚙️ · calling CareRoute with managed identity — **a third issuer** (Entra) added to the policy scheme for one app-role-protected endpoint.
🔵 Logic Apps vs Functions vs code · minimal email content (health data) · connector auth and data leaving the boundary.

### ⚖️ 💣 ⬛
- ⚖️ Low-code vs code; Consumption vs Standard.
- 💣 Patient details in email. 💣 Expired connector auth unnoticed. 💣 Message completed before email sent. 💣 Accepting *any* Entra token (no app role / audience check).
- ⬛ Minimal content · alert on failed runs · Entra tokens validated for tenant, audience and app role · workflow versioned.

### 🎬 🎬 Message → trigger → HTTP (managed identity) → email → complete; failure → DLQ.

### 🛠️ Build
Logic App + identity; trigger; Parse JSON; Entra app registration/app role for CareRoute.Api (portal click path); `EntraWorkload` scheme + endpoint `api/v1/notifications/recipients/{referralId}`; email; export JSON to `infra/logicapps/`; alert.

### 🧪 Tests (Part 3)
Api: endpoint → 401 anonymous, 403 Keycloak user token, 403 Entra token without app role, 200 with role (test tokens signed by a test key configured as the Entra scheme in tests); schema test against `ReferralSubmittedV1`. 💥 Rename an event field → schema test red → restore.

### 🏁 PR `feat(azure): Logic App notifications with managed identity`

### 🎤 1. Logic Apps vs Functions? 2. Calling an API without secrets from a workflow? 3. Email content rules? 4. Noticing failures? 5. 🎤 *"Your API now trusts Keycloak, IdentityServer and Entra ID. How do you keep that safe?"*

---

## L49 — API Management: one secured front door
**Day 13 · Slot 1 · P49**
**🏁 End:** APIM (cheapest suitable tier — verify; Developer takes 30+ minutes to create — start first) importing Api, Intake and partner APIs from OpenAPI; `validate-jwt` against **Keycloak's** discovery (and IdentityServer for the partner product); rate limits by `client_id`; correlation header; CORS; products (`partners` with subscription key **plus** OAuth); backends locked to APIM; policies in Git.

### Concepts
🟢 Gateway vs YARP · APIM structure · **policy pipeline** ⚙️ (inbound → backend → outbound → on-error; scopes) · `validate-jwt` (openid-config URL, audiences, required claims) / `validate-azure-ad-token`, `rate-limit-by-key`, `set-header`, `cors`.
🔵 **Defence in depth**: APIM validates *and* the API validates · subscription keys are not authentication · versions vs revisions · locking backends · developer portal for partners · managed identity from APIM to backends.

### ⚖️ 💣 ⬛
- ⚖️ Managed gateway vs cost/hop; gateway vs app rate limits (both).
- 💣 Backend still public. 💣 Removing API JWT checks "because APIM does it". 💣 Policy XML error → 500s. 💣 `validate-jwt` without audience → any Keycloak token accepted.
- ⬛ Validate at gateway and service · audience and issuer pinned · backends private/restricted · policies versioned · partner onboarding through products.

### 🎬 🎬 Request through the policy pipeline: 401 at inbound, 429, OK with headers.

### 🛠️ Build
APIM; imports; global and API policies; partner product; backend restrictions; portal and partner client point at APIM; `infra/apim/`. 🧹 Delete after L50 if not needed.

### 🧪 Tests (Part 3)
Smoke against APIM: no token → 401 (gateway); Keycloak token for the wrong audience → 401; valid → 200 with correlation header; direct backend → 403; partner without key → 401, key without OAuth token → 401, both → 200; burst → 429. 💥 Remove `validate-jwt` → still 401 **from the API** → defence in depth proven → restore.

### 🏁 PR `feat(azure): API Management front door`

### 🎤 1. APIM vs YARP? 2. Policy pipeline? 3. Why validate twice? 4. Subscription key vs OAuth? 5. 🎤 *"Design secure partner access to your APIs through a gateway."*

---

# FINISH

## L50 — ★ Final checkpoint, capstone & technical-interview simulation
**Day 13 · Slots 2–3 (double lesson, ~4 hours) · P50**

| Part | Time | What Claude delivers |
|---|---|---|
| A. Present CareRoute | 20 min | 5-minute walkthrough: architecture → identity model (Keycloak + brokers + SAML) → one request end to end (portal BFF → APIM → API → outbox → Service Bus → Durable → Logic App) → one App Insights trace → one security decision you'd defend. Then 8 follow-up questions. |
| B. Technical interview simulation | 60 min | Four rounds, interviewer style: (1) **protocols** — OAuth/OIDC/SAML deep dive; (2) **Keycloak** — configuration, SPIs, production; (3) **coding** — write an auth handler/authorisation handler on paper; (4) **system design** — "SSO and secure partner integrations for a hospital group with Keycloak, a legacy SAML app and an Azure-hosted API platform". ⛔ Model answers, scoring rubric, red flags. |
| C. Troubleshooting round | 25 min | 4 incidents across the full stack (`invalid_grant` after an IdP upgrade, SAML cert rollover, APIM 401s after a key rotation, duplicate escalation emails). |
| D. Remediation | 35 min | Re-teach your top 5 gaps with a visual each. |
| E. Portfolio | 40 min | README (architecture & identity diagrams, how to run locally and in Azure, ADR index, security model, known limitations, what's lab-only); a 2-minute **honest** project pitch; 6 STAR stories from your Interview Story Bank. |
| F. Cheat sheet & cleanup | 20 min | One printable page; ☁️ Azure cleanup checklist (APIM, Container Apps, PostgreSQL, Service Bus, Function App, Logic App, App Service plan, SQL, Key Vault soft-delete, workspace); tag `v1.0`. |

- PR `docs: README, interview pack, v1.0`.

---

# APPENDICES

## Appendix A — Tool cheat sheets

**Visual Studio**
| Action | Where / shortcut |
|---|---|
| Run / without debugger / stop | F5 / Ctrl+F5 / Shift+F5 |
| Breakpoint · step over · step into | F9 · F10 · F11 |
| Build · run all tests | Ctrl+Shift+B · Ctrl+R, A (Test Explorer) |
| NuGet (pick a version) | Right-click project → **Manage NuGet Packages** → **Version** |
| User secrets | Right-click project → **Manage User Secrets** |
| Git | **View > Git Changes**, **Git > Manage Branches**, **Git > GitHub > Create Pull Request** (verify) |
| Merge conflicts | **Git Changes** → conflict → **Merge Editor** |
| HTTP requests | `.http` files; **View > Other Windows > Endpoints Explorer** |
| Azure sign-in for `DefaultAzureCredential` | **Tools > Options > Azure Service Authentication** (verify) |
| Package Manager Console | **Tools > NuGet Package Manager > Package Manager Console** |

**Browser dev tools (F12):** Network (Preserve log, Disable cache, filter `Doc`/`Fetch`), Application > Cookies/Storage, Security (certificate), Console. **Never share raw HAR files.**

**Keycloak admin console:** Realm settings (General, Login, Keys, Tokens, Sessions, Events, Themes, Security defenses) · Clients (Settings, Keys/Credentials, Client scopes, Service account roles, Advanced) · Client scopes · Realm roles · Groups · Users (Attributes, Role mapping, Sessions) · Identity providers · Authentication (Flows, Required actions, Policies) · Events (user & admin) — verify labels for the pinned version.

**IntelliJ IDEA:** Open `pom.xml` as project · Maven tool window (Lifecycle → `verify`, `package`) · Run tests (gutter ▶) · Project SDK = JDK 21 (verify).

**GitHub:** Pull requests · Actions · Settings > Branches/Rulesets · Settings > Code security (Dependabot, code scanning, secret scanning, push protection) · Environments.

## Appendix B — Docker Desktop recipes

| Service | Image : tag | Ports | Notes |
|---|---|---|---|
| Keycloak | `quay.io/keycloak/keycloak` : exact 26.x (verify) → later `careroute/keycloak` | 8080, management 9000 (verify) | with PostgreSQL; realm import |
| PostgreSQL (Keycloak DB) | `postgres` : pinned major (verify supported versions) | internal only | volume `careroute-keycloak-db` |
| MongoDB | `mongo` : `8` | 27017 | auth on, volume |
| RabbitMQ | `rabbitmq` : `4-management` | 5672, 15672 | definitions file with per-service users |
| Seq | `datalust/seq` (verify) | 5341 / 80 | licence env var (verify) |
| OTel dashboard | Aspire dashboard or Jaeger (verify) | UI + OTLP 4317 | |
| Mail catcher | e.g., Mailpit (verify) | 8025 / 1025 | Keycloak & notification emails |

Full local stack in `docker-compose.yml` (L39). **Docker Desktop must be running** from L12.

## Appendix C — Packages & artifacts

| When | Package | Version / note |
|---|---|---|
| L05 | `FluentAssertions` | **7.x pinned** (v8+ commercial) |
| L05 | `NSubstitute`, `Bogus`, `NetArchTest.Rules` or `TngTech.ArchUnitNET.xUnit` | latest (verify) |
| L07 | `Microsoft.Extensions.TimeProvider.Testing` | 10.x |
| L08 | `MediatR` | **12.5.0 pinned** |
| L08 | `FluentValidation.DependencyInjectionExtensions` | latest |
| L11 | `Microsoft.EntityFrameworkCore.SqlServer`, `.Design`, `.Tools` | 10.0.x |
| L12 | `Microsoft.AspNetCore.Mvc.Testing`, `Testcontainers.MsSql` | 10.0.x / latest |
| L13 | Serilog (`Serilog.AspNetCore`, sinks, enrichers), health checks, OpenTelemetry | latest (verify) |
| L14 | `Yarp.ReverseProxy`, `bunit`, QuickGrid | latest / 10.0.x |
| L18 | `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.x |
| L19 | `Testcontainers.Keycloak` | latest (verify) |
| L20/L23 | `Microsoft.AspNetCore.Authentication.OpenIdConnect` | 10.0.x |
| L24 | `Duende.AccessTokenManagement` (verify licence) or hand-written cache | — |
| L26 | `Sustainsys.Saml2.AspNetCore2` (alt. `ITfoxtec.Identity.Saml2`) | verify .NET 10 support |
| L28–L29 | Maven: `keycloak-server-spi`, `keycloak-server-spi-private`, `keycloak-services` (provided), JUnit 5, Mockito, `testcontainers-keycloak` (dasniko — verify) | = pinned Keycloak version |
| L30 | `Duende.IdentityServer`, `.AspNetIdentity`, `.EntityFramework` | latest (dev/test free; production licence) |
| L35–L36 | `MongoDB.Driver`, `Testcontainers.MongoDb`; `RabbitMQ.Client` 7.x, `Testcontainers.RabbitMq` | |
| L40 | `Microsoft.Extensions.Caching.Hybrid` | 10.x |
| L41 | `Microsoft.Extensions.Http.Resilience` | 10.x |
| L43 | `Azure.Identity`, `Azure.Extensions.AspNetCore.Configuration.Secrets`, `Azure.Extensions.AspNetCore.DataProtection.Blobs`/`.Keys` | latest |
| L45 | `Azure.Monitor.OpenTelemetry.AspNetCore` | latest (verify) |
| L46 | `Azure.Messaging.ServiceBus` | latest |
| L47 | `Microsoft.Azure.Functions.Worker`, `.Extensions.DurableTask`, `.Extensions.ServiceBus` | verify .NET 10 |

Not used (licensing): MediatR v13+, FluentAssertions v8+, MassTransit v9+, AutoMapper v15+. Duende products: dev/test only here.

## Appendix D — Healthcare vocabulary
GP · Specialist · Referral (`ServiceRequest` in HL7 FHIR) · Triage · Intake form · Appointment/slot · National register number (personal data) · HL7 FHIR · Special-category data (GDPR) · Deadline/SLA (48 h triage; 7/60 days booking) · Audit trail (created, changed **and viewed**).

## Appendix E — DDD notes map

| Term | In CareRoute | First 🧩 note |
|---|---|---|
| Bounded context, ubiquitous language | Folders per context; `docs/ubiquitous-language.md` | L05 |
| Value object, entity, invariant | `NationalNumber`, `Patient` | L06 |
| Aggregate, domain event, domain service | `Referral`, `DoctorSchedule`, `BookingEligibilityService` | L07 |
| Policy | Accepted → booking request | L34 |
| Bounded context as a service | Clinical Intake | L35 |
| Integration event / published language | `ReferralSubmittedV1` | L36 |
| Anti-corruption layer, context map | National registry ACL | L41 |

## Appendix F — Clean Architecture quick reference

| Layer | Contains | Never contains |
|---|---|---|
| Domain | Entities, value objects, events, business permission rules | Tokens, `HttpContext`, EF Core, logging frameworks |
| Application | Use cases, validators, behaviors, ports (`ICurrentUser`, `IEventBus`, `IIdentityProvisioning`, `IBookingDeadlineScheduler`) | DbContext, broker/IdP clients, controllers |
| Infrastructure | EF Core, adapters (RabbitMQ/Service Bus, Keycloak Admin, registry ACL, Key Vault), `HttpCurrentUser`, claims transformations | Business rules |
| Api / Portal host / Worker / Functions | Authentication handlers & schemes, middleware, controllers, triggers, composition root | Business rules, SQL |

## Appendix G — Local ↔ Azure map

| Local | Azure |
|---|---|
| LocalDB / SQL container | Azure SQL (Entra auth) |
| User Secrets | Key Vault (managed identity) |
| Keycloak + PostgreSQL in Docker | Container Apps + PostgreSQL Flexible Server |
| Seq + OTel dashboard | Application Insights + Log Analytics |
| RabbitMQ | Service Bus |
| Hosted process / timers | Durable Functions |
| Worker email path | Logic App |
| YARP gateway | API Management |
| F5 / Compose | App Service + slots via GitHub Actions (OIDC federation) |

## Appendix H — Standards & RFC map (verify before quoting in interviews)

| Topic | Spec |
|---|---|
| OAuth 2.0 framework · bearer tokens | RFC 6749 · RFC 6750 |
| PKCE | RFC 7636 |
| JWT · JWS · JWK | RFC 7519 · RFC 7515 · RFC 7517 |
| Authorization server metadata | RFC 8414 |
| Token revocation · introspection | RFC 7009 · RFC 7662 |
| Device authorization grant | RFC 8628 |
| Token exchange | RFC 8693 |
| mTLS client auth & bound tokens · DPoP | RFC 8705 · RFC 9449 |
| JWT profile for access tokens | RFC 9068 |
| PAR | RFC 9126 |
| OAuth 2.0 Security Best Current Practice | RFC 9700 |
| OAuth 2.1 · OAuth for browser-based apps | IETF drafts (verify status) |
| OpenID Connect Core, Discovery, RP-Initiated / Back-Channel Logout | OpenID Foundation specs |
| SAML 2.0 (core, bindings, profiles, metadata) | OASIS |
| SCIM | RFC 7643 · RFC 7644 |
| Problem Details for HTTP APIs | RFC 9457 |
| OWASP Top 10 · API Security Top 10 · ASVS | OWASP (verify current editions) |

## Appendix I — Production identity checklist (used in L32, L44, L50)

**Identity provider:** exact pinned version, patched promptly · PostgreSQL with backups and tested restore · fixed public hostname, TLS, proxy headers · admin console isolated, admins with MFA · brute-force protection · password policy · events on and shipped to logging · signing keys rotated with overlap · HA plan · realm config as code without secrets · custom SPIs rebuilt and tested per upgrade.
**Clients:** code + PKCE only · exact redirect URIs · unused grants disabled · one client per app/integration · asymmetric client authentication where possible · least-privilege scopes · secrets in a vault, rotated.
**Tokens:** short access-token lifetime · refresh-token rotation · audience per API · minimal claims, no personal data · validated issuer/audience/signature/lifetime with pinned algorithms.
**Applications:** BFF for browser apps with sensitive data · secure cookie attributes · antiforgery · shared Data Protection keys · back-channel logout · fallback authorisation policy · resource-based checks · consistent 401/403 · auth failures logged as structured security events without tokens.
**Operations:** alerts on login error spikes and 401/403 anomalies · runbooks: leaked client secret, compromised account, signing key compromise, IdP outage, certificate rollover · troubleshooting guide maintained.

## Appendix J — Identity interview question bank (seed list — grows with `INTERVIEW`)
1. Walk me through authorization code + PKCE. 2. OAuth vs OIDC? 3. ID token vs access token? 4. How do you validate a JWT, and what attacks does each check stop? 5. Where should a SPA keep tokens? 6. How do refresh-token rotation and reuse detection work? 7. Client credentials vs token exchange for service calls? 8. SAML vs OIDC; how does SAML POST binding work? 9. What's in SAML metadata and why does it matter? 10. How would you troubleshoot "Correlation failed" / `invalid_grant` / `IDX10205`? 11. Keycloak: realm vs client vs client scope vs mapper. 12. Keycloak SPIs you'd build and why; how you deploy and upgrade them. 13. Keycloak in production: what must be configured? 14. IdentityServer vs Keycloak vs Ping — choosing. 15. How do you support two IdPs in one API? 16. Write an `AuthenticationHandler`. 17. CSRF vs XSS and defences in a cookie-based app. 18. What does CORS protect? 19. How do you secure partner integrations across an organisation? 20. Design SSO for a hospital group with legacy SAML apps and modern APIs. 21. How do you handle a leaked client secret? 22. Git: merge vs rebase; protecting main; a secret in history. 23. What's in your CI/CD pipeline for security? 24. How do you keep personal data out of logs and traces? 25. What would you monitor on an identity platform?

## Appendix K — .NET 10 & C# 14 features used

| Feature | Tag | Lesson |
|---|---|---|
| `.slnx` | VS 2026 | L01 |
| OpenAPI 3.1 document | `[.NET 10]` | L01, L03, L49 |
| `field` keyword | `[C# 14]` | L06 |
| Complex types, named query filters, `LeftJoin` | `[EF10]` | L11, L12 |
| HybridCache | `[.NET 9+]` | L40 |
| Passkeys in ASP.NET Core Identity | `[.NET 10]` (verify) | L30 🔵 |

.NET 10 is LTS (Nov 2025 → Nov 2028). CareRoute uses controllers; `AddValidation()` (minimal APIs) isn't used.
