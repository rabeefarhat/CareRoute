CONTINUITY UPDATE

(a) Replace the whole `## CURRENT STATE` section with:

## CURRENT STATE

**Last completed lesson:** L02 — ASP.NET Core fundamentals: pipeline, DI, configuration, validation, errors
**Next lesson:** L03 — Clean Architecture & the testing toolkit
**Pace:** 4 lessons a day (~2 hours each, 9 days)
**Today:** Day 1 — L01 ✅, L02 ✅, L03, L04
**Behind schedule?** __

### Project state (CareRoute — hospital platform)
- **Last project step completed:** P02 Fundamentals — ✅ code provided — tick when it runs on your machine
- **Physical location:** __
- **Solution tree:**
```
  CareRoute/
  ├─ CareRoute.slnx
  ├─ .gitignore
  └─ src/
     └─ CareRoute.Api/
        ├─ Controllers/PatientsController.cs
        ├─ Controllers/DiagnosticsController.cs
        ├─ ErrorHandling/GlobalExceptionHandler.cs
        ├─ Options/ReferralOptions.cs
        ├─ Patients/Patient.cs
        ├─ Patients/RegisterPatientRequest.cs
        ├─ Patients/PatientResponse.cs
        ├─ Patients/IPatientStore.cs
        ├─ Patients/InMemoryPatientStore.cs
        ├─ Properties/launchSettings.json
        ├─ appsettings.json
        ├─ appsettings.Development.json
        ├─ CareRoute.Api.http
        ├─ CareRoute.Api.csproj
        └─ Program.cs
```
- **What works:** GET list (sorted by last name) / GET by id (404 ProblemDetails) / POST register → 201 + Location; automatic 400 ValidationProblemDetails; over-posted `id` ignored; unhandled exception → 500 ProblemDetails without details; `Referrals` options validated at startup; `/health` → Healthy; dev-only `/api/diagnostics/throw` and `/api/diagnostics/race`; manual tests T1–T17 pass.
- **Differences from the lessons / known issues:** __
- **HTTPS port:** __
- **GitHub repository URL:** __

### Code facts (VERIFIED names and signatures — Claude must use these, never guess)
- Project `CareRoute.Api` at `src/CareRoute.Api`, `net10.0`, Nullable + ImplicitUsings enabled.
- `Program.cs` services: `AddControllers()`, `AddOpenApi()`, `AddProblemDetails()`, `AddExceptionHandler<GlobalExceptionHandler>()`, `AddSingleton<IPatientStore, InMemoryPatientStore>()`, `AddOptions<ReferralOptions>().BindConfiguration(ReferralOptions.Section).ValidateDataAnnotations().ValidateOnStart()`, `AddHealthChecks()`.
- `Program.cs` pipeline: `UseExceptionHandler()` → `MapOpenApi()` (Development) → `UseHttpsRedirection()` → `UseAuthorization()` → `MapControllers()` → `MapHealthChecks("/health")`.
- `namespace CareRoute.Api.Patients;`
  - `public sealed record Patient(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth);` (unchanged)
  - `public sealed record RegisterPatientRequest` with `[Required][StringLength(100, MinimumLength = 1)] string? FirstName { get; init; }`, same for `LastName`, `[Required] DateOnly? DateOfBirth { get; init; }`
  - `public sealed record PatientResponse(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth)` with `public static PatientResponse From(Patient patient)`
  - `public interface IPatientStore` { `Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken)`; `Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)`; `Task AddAsync(Patient patient, CancellationToken cancellationToken)` }
  - `public sealed class InMemoryPatientStore : IPatientStore` — `ConcurrentDictionary<Guid, Patient>`, seeded with the 3 fictional patients, `AddAsync` throws `InvalidOperationException` on duplicate id.
- `namespace CareRoute.Api.Options;` → `public sealed class ReferralOptions` { `const string Section = "Referrals"`; `[Range(1,365)] int MaxDraftAgeDays { get; init; }`; `[Range(1,168)] int UrgentTriageHours { get; init; }` }; `appsettings.json` `Referrals: { MaxDraftAgeDays: 30, UrgentTriageHours: 48 }`.
- `namespace CareRoute.Api.ErrorHandling;` → `public sealed class GlobalExceptionHandler(IProblemDetailsService, ILogger<GlobalExceptionHandler>) : IExceptionHandler` → logs the error, writes 500 ProblemDetails (title "An unexpected error occurred."), returns true.
- `namespace CareRoute.Api.Controllers;`
  - `PatientsController(IPatientStore store, ILogger<PatientsController> logger) : ControllerBase` — `[ApiController] [Route("api/[controller]")]`; `GetAll(CancellationToken)` → `ActionResult<IReadOnlyList<PatientResponse>>`; `[HttpGet("{id:guid}")] GetById(Guid id, CancellationToken)` → `ActionResult<PatientResponse>`; `[HttpPost] Register(RegisterPatientRequest request, CancellationToken)` → `CreatedAtAction(nameof(GetById), …)`; logs `"Registered patient {PatientId}"`.
  - `DiagnosticsController(IHostEnvironment environment)` — `[Route("api/diagnostics")]`, `[ApiExplorerSettings(IgnoreApi = true)]`; `[HttpGet("throw")] Throw()`; `[HttpPost("race")] Race([FromServices] IPatientStore, CancellationToken)` → `{ expected, added }`; both 404 outside Development.
- Fictional patient IDs: unchanged from L01.
- `CareRoute.Api.http`: requests T1–T17 (T13 uses `# @name overpost`).

### Azure resources (from L28)
| Resource | Name | Tier | Region | Running cost note |
|---|---|---|---|---|
| _(none yet)_ | | | | |

### Working agreements
_(unchanged — keep items 1–8)_

### Project steps
- [x] P01 First API · [x] P02 Fundamentals · [ ] P03 Clean Architecture + test toolkit · [ ] P04 Domain I
_(rest unchanged)_

### Ubiquitous language (source of truth: docs/ubiquitous-language.md)
- _(empty — starts in L03)_ — terms used informally so far: Patient, Register (a patient), Referral (options only)

### Environment
_(unchanged)_

### Weak spots (max 15)
- (keep or delete from L01) Malformed id → empty 404 vs unknown id → ProblemDetails 404
- (keep or delete from L01) C# → IL → JIT chain out loud
- (keep or delete from L01) launchSettings.json vs appsettings.json
- (candidate — keep or delete) Why `[Required]` needs `DateOnly?` instead of `DateOnly`
- (candidate — keep or delete) Captive dependency: explain it and its fix without notes
- (candidate — keep or delete) Config binding is case-insensitive — what a "typo" test really needs

### Things that confused me
- __

### Skipped / deferred items
- _(none)_

### Decisions (ADRs, one line each)
- _(none yet — ADR-001 in L03)_ · informal: controllers only; 404 for malformed ids; singleton in-memory store with ConcurrentDictionary; async store interface ready for EF Core; exception details never in responses



(b) Add to `## LESSON LOG`:
| L01 | __ | __ | .NET/SDK/runtime/LTS; C#→IL→JIT→Kestrel; controllers created per request; silent 404 without MapControllers | __ /5 | __ |
| L02 | __ | __ | pipeline order; request flow & [ApiController]; DI lifetimes & captive deps; DTOs vs over-posting; ValidateOnStart; ProblemDetails; await ≠ new thread; 💥 List<T> race | __ /5 | __ |

(c) Append to `## KEY IDEAS CHEAT SHEET`:
- SDK builds, runtime runs; C# compiles to IL, the JIT turns IL into machine code at runtime, Kestrel serves HTTP.
- LTS (.NET 10, to Nov 2028) is supported, not frozen — monthly patches still have to be installed.
- Controllers are discovered once at startup but created new for every request — never keep state in controller fields.
- Trade-off: controllers give structure, filters and conventions; minimal APIs give less ceremony — pick one style per service.
- Failure mode: a 404 with an empty body means no endpoint matched — check `MapControllers()`, `public`, `: ControllerBase`, and route constraints.
- Status codes are the verdict: 404 for missing data, never 200 with an error in the body.
- Middleware is a nested chain: first registered = outermost; the exception handler goes first.
- Request flow: routing → DI creates controller → bind → validate → filters → action → result; `[ApiController]` auto-400s invalid input.
- Lifetimes flow downwards only; a singleton must be thread-safe (List<T> lost writes under 1,000 parallel adds).
- Trade-off: ValidateOnStart costs a slower start but turns config typos into deploy-time crashes.
- Failure mode: `.Result` blocks pool threads → thread-pool starvation → latency spikes with low CPU.
- DTOs are allow-lists; for value types, use `DateOnly?` + `[Required]` or a missing value becomes the default.