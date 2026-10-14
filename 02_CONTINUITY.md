CONTINUITY UPDATE

(a) ## CURRENT STATE

**Last completed lesson:** L02 — ASP.NET Core fundamentals: pipeline, DI, configuration, validation, errors
**Next lesson:** L03 — REST API design done right
**Pace:** 4 lessons a day · **Today:** Day 1 (L01 ✓, L02 ✓, L03, L04) · **Behind schedule?** __

### Project state
- **Last project step completed:** P02 — ✅ code provided — tick when it runs on your machine [ ]
- **Physical location:** D:\CareRoute\ (__ confirm)
- **Solution tree:**
  CareRoute.slnx · .gitignore · src/CareRoute.Api/{Configuration/{ReferralOptions.cs, ReferralPolicyController.cs}, Diagnostics/DiagnosticsController.cs (DEV ONLY), Errors/GlobalExceptionHandler.cs, Patients/{Patient.cs, IPatientStore.cs, InMemoryPatientStore.cs, PatientsController.cs, RegisterPatientRequest.cs}, Program.cs, appsettings.json, appsettings.Development.json, CareRoute.Api.http, Properties/launchSettings.json}
- **What works:** GET api/patients (seeded 3 + registered, sorted) · GET api/patients/{id:guid} 200/404 · POST api/patients → 201 + Location, 400 ValidationProblemDetails, 415, over-posted id ignored · GET api/referral-policy · GET /health · GET api/diagnostics/throw → 500 ProblemDetails (Development only, 404 elsewhere) · ReferralOptions ValidateOnStart
- **Branch / PR:** still committing to `master` (default branch is `master`; rename to `main` planned in L04). Commits: refactor(patients)…, feat(patients)…, feat(config)…, feat(errors)…, test(api)…
- **Differences from the lessons / known issues:** __
- **Ports:** API HTTPS 7038 · API HTTP 5201
- **GitHub repository URL:** github.com/rabeefarhat/CareRoute (__ private/public)

### Code facts
- Store: `IPatientStore` (GetAllAsync, GetByIdAsync, AddAsync, all with CancellationToken) + `InMemoryPatientStore` (ConcurrentDictionary, seeded with L01's 3 GUIDs, TryAdd throws InvalidOperationException on duplicate) — registered AddSingleton; both in Api/Patients until L05.
- `PatientsController(IPatientStore store, ILogger<PatientsController> logger)` primary constructor; actions GetAll, GetById, Register (POST, CreatedAtAction(nameof(GetById))).
- `RegisterPatientRequest` sealed record, init props: string? FirstName/LastName [Required, StringLength(100, MinimumLength=1)], DateOnly? DateOfBirth [Required]; no Id.
- `ReferralOptions` (namespace CareRoute.Api.Configuration, SectionName "Referrals"): UrgentTriageHours [1..168]=48, MaxDraftAgeDays [1..365]=30, UrgentBookingDays [1..30]=7, RoutineBookingDays [1..365]=60; no C# defaults on purpose.
- `GlobalExceptionHandler` (CareRoute.Api.Errors) : IExceptionHandler via IProblemDetailsService; generic Detail, logs full exception.
- Program.cs order: UseExceptionHandler → MapOpenApi (Dev) → UseHttpsRedirection → UseAuthorization → MapHealthChecks("/health") → MapControllers.
- Empty `<Folder Include="Controllers\" />` removed from csproj (__ confirm).

### Identity facts — none yet
### Azure resources — none
### Ubiquitous language — (starts L05); terms in use: Patient, GP (planned), Referral policy (triage/booking deadlines)

### Candidate weak spots
- Why a race in a non-thread-safe singleton never shows in manual testing
- `DateOnly` vs `DateOnly?` with [Required]
- Middleware vs filter: which concern goes where

(b) ## LESSON LOG row
| L02 | __ | __ | Pipeline order; MVC bind→validate→action; lifetimes & captive dependency; DTOs vs over-posting; ValidateOnStart; ProblemDetails without internals | __ | __ |

(c) ## KEY IDEAS CHEAT SHEET
- Middleware order is behaviour: exception handler first, authentication before authorisation, endpoints last.
- Trade-off: singleton store = shared and cheap, but must be thread-safe; scoped = isolated, but in-memory data dies with the request.
- Failure mode: captive dependency (singleton holding scoped) → cross-request data / ObjectDisposedException; caught at startup by scope validation in Development.
- Failure mode: non-nullable DateOnly + [Required] → missing value becomes 0001-01-01 silently.
- Production rule: bind DTOs only (allow-list), ValidateOnStart for options, 500s return generic ProblemDetails + traceId while the full exception goes to logs.

(d) ## INTERVIEW STORY BANK
- S: CareRoute's first write endpoint. T: Accept patient registrations safely. A: Bound a DTO without an Id (over-posting defence), validated with DataAnnotations (nullable DateOnly to avoid silent defaults), moved storage to a thread-safe singleton behind an interface, and added a global IExceptionHandler. R: Proved by test that a client-supplied id is ignored and that a thrown exception returns a 500 ProblemDetails with a traceId but no internal details.
- Fail-fast config: options validated on start, so a zero triage deadline stops the deployment instead of corrupting referral deadlines.