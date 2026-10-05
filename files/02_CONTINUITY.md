CONTINUITY UPDATE

(a) Replace the whole `## CURRENT STATE` section with:

## CURRENT STATE

**Last completed lesson:** L03 — Clean Architecture & the testing toolkit
**Next lesson:** L04 — Domain layer I: patients, value objects & invariants
**Pace:** 4 lessons a day (~2 hours each, 9 days)
**Today:** Day 1 — L01 ✅, L02 ✅, L03 ✅, L04
**Behind schedule?** __

### Project state (CareRoute — hospital platform)
- **Last project step completed:** P03 Clean Architecture + test toolkit — ✅ code provided — tick when it runs on your machine
- **Physical location:** __
- **Solution tree:**
```
  CareRoute/
  ├─ CareRoute.slnx                    (solution folders: src, tests, docs)
  ├─ .gitignore
  ├─ docs/
  │  ├─ ubiquitous-language.md
  │  └─ adr/ADR-001-clean-architecture.md
  ├─ src/
  │  ├─ CareRoute.Domain/PatientRegistry/Patient.cs
  │  ├─ CareRoute.Application/
  │  │  ├─ DependencyInjection.cs
  │  │  └─ PatientRegistry/IPatientStore.cs, RegisterPatientService.cs
  │  ├─ CareRoute.Infrastructure/
  │  │  ├─ DependencyInjection.cs
  │  │  └─ PatientRegistry/InMemoryPatientStore.cs
  │  └─ CareRoute.Api/
  │     ├─ Controllers/PatientsController.cs, DiagnosticsController.cs
  │     ├─ ErrorHandling/GlobalExceptionHandler.cs
  │     ├─ Options/ReferralOptions.cs
  │     ├─ Patients/RegisterPatientRequest.cs, PatientResponse.cs
  │     ├─ Properties/launchSettings.json
  │     ├─ appsettings.json, appsettings.Development.json
  │     ├─ CareRoute.Api.http
  │     └─ Program.cs
  └─ tests/
     ├─ CareRoute.Testing/Fakers/PatientFaker.cs
     ├─ CareRoute.Domain.Tests/PatientFakerTests.cs
     └─ CareRoute.Application.Tests/
        ├─ Infrastructure/InMemoryPatientStoreTests.cs   (temporary home — moves in L10)
        └─ PatientRegistry/RegisterPatientServiceTests.cs
```
- **What works:** everything from L02 (T1–T17), now across four layers; POST goes through `RegisterPatientService`; 8 unit tests green in Test Explorer.
- **Differences from the lessons / known issues:** __ (e.g. whether `InMemoryPatientStore` has a parameterless ctor — assumed; xUnit v2 or v3 template: __)
- **HTTPS port:** __
- **GitHub repository URL:** __

### Code facts (VERIFIED names and signatures — Claude must use these, never guess)
- Projects (all `net10.0`, Nullable + ImplicitUsings): `src/CareRoute.Domain` (no refs, no packages), `src/CareRoute.Application` (→ Domain; pkg Microsoft.Extensions.DependencyInjection.Abstractions 10.x), `src/CareRoute.Infrastructure` (→ Application; same pkg), `src/CareRoute.Api` (→ Application, Infrastructure), `tests/CareRoute.Testing` (→ Domain; pkg Bogus), `tests/CareRoute.Domain.Tests` (→ Domain, Testing; FluentAssertions `[7.2.2,8.0.0)`), `tests/CareRoute.Application.Tests` (→ Application, Infrastructure, Testing; FluentAssertions `[7.2.2,8.0.0)`, NSubstitute, NSubstitute.Analyzers.CSharp optional).
- `namespace CareRoute.Domain.PatientRegistry;` → `public sealed record Patient(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth);` (moved from Api, unchanged)
- `namespace CareRoute.Application.PatientRegistry;`
  - `public interface IPatientStore` — signatures unchanged from L02.
  - `public sealed class RegisterPatientService(IPatientStore store)` → `public async Task<Guid> RegisterAsync(string firstName, string lastName, DateOnly dateOfBirth, CancellationToken cancellationToken)` — creates `Patient` with `Guid.NewGuid()`, awaits `store.AddAsync`, returns id.
- `namespace CareRoute.Application;` → `public static class DependencyInjection` → `public static IServiceCollection AddApplication(this IServiceCollection services)` (registers `AddScoped<RegisterPatientService>()`).
- `namespace CareRoute.Infrastructure.PatientRegistry;` → `public sealed class InMemoryPatientStore : IPatientStore` (body unchanged from L02).
- `namespace CareRoute.Infrastructure;` → `public static class DependencyInjection` → `public static IServiceCollection AddInfrastructure(this IServiceCollection services)` (registers `AddSingleton<IPatientStore, InMemoryPatientStore>()`).
- `Program.cs`: `builder.Services.AddApplication().AddInfrastructure();` replaces the store line; all other services and pipeline unchanged from L02.
- `PatientsController(IPatientStore store, RegisterPatientService registerPatient, ILogger<PatientsController> logger)` — GetAll/GetById unchanged; `Register` calls `registerPatient.RegisterAsync(...)`, logs `"Registered patient {PatientId}"`, reads back, returns `CreatedAtAction(nameof(GetById), new { id }, ...)`.
- `PatientResponse`, `DiagnosticsController`: only `using`s changed (`CareRoute.Domain.PatientRegistry`, `CareRoute.Application.PatientRegistry`).
- `namespace CareRoute.Testing.Fakers;` → `public sealed class PatientFaker : Faker<Patient>` — `public const int DefaultSeed = 20260;`, private static `ReferenceDate = new DateTime(2026, 1, 1)`, ctor `PatientFaker(int seed = DefaultSeed)`, locale `"nl_BE"`, `UseSeed(seed)`, `CustomInstantiator` with `f.Random.Guid()`, `f.Name.FirstName()`, `f.Name.LastName()`, `DateOnly.FromDateTime(f.Date.Past(90, ReferenceDate))`.
- Tests: `InMemoryPatientStoreTests` (Add_then_get_returns_the_same_patient, Get_with_unknown_id_returns_null, Concurrent_adds_are_all_stored), `RegisterPatientServiceTests` (Registering_adds_the_patient_to_the_store_exactly_once, Registering_returns_the_id_of_the_stored_patient), `PatientFakerTests` (Same_seed_produces_the_same_first_patient, Different_seeds_produce_different_patients, Birth_dates_are_within_90_years_before_the_reference_date).

### Azure resources (from L28)
| Resource | Name | Tier | Region | Running cost note |
|---|---|---|---|---|
| _(none yet)_ | | | | |

### Working agreements
_(unchanged — keep items 1–8)_

### Project steps
- [x] P01 First API · [x] P02 Fundamentals · [x] P03 Clean Architecture + test toolkit · [ ] P04 Domain I
_(rest unchanged)_

### Ubiquitous language (source of truth: docs/ubiquitous-language.md)
- Patient, Register (a patient), GP, Referral, Department, Bounded context — contexts: Patient Registry, Referrals, Scheduling, Hospital Directory, Clinical Intake, Notifications, Identity & Access

### Environment
_(unchanged)_

### Weak spots (max 15)
- (keep or delete from L01) Malformed id → empty 404 vs unknown id → ProblemDetails 404
- (keep or delete from L01) C# → IL → JIT chain out loud
- (keep or delete from L01) launchSettings.json vs appsettings.json
- (keep or delete from L02) Why `[Required]` needs `DateOnly?` instead of `DateOnly`
- (keep or delete from L02) Captive dependency: explain it and its fix without notes
- (keep or delete from L02) Config binding is case-insensitive — what a "typo" test really needs
- (candidate — keep or delete) What the compiler enforces (project refs, cycles) vs what it doesn't (packages)
- (candidate — keep or delete) Stub vs spy vs mock vs fake with a CareRoute example each
- (candidate — keep or delete) Why a Bogus seed alone isn't enough (refDate, Guid.NewGuid, shared faker)

### Things that confused me
- __

### Skipped / deferred items
- Infrastructure tests live in CareRoute.Application.Tests/Infrastructure until L10.
- Curriculum correction: the L03 💥 "Domain → Infrastructure reference still builds" is refused by VS as a cycle; the 💥 used a Domain → EF Core package leak instead. Use the package version of this 💥 again in L22.

### Decisions (ADRs, one line each)
- ADR-001: Clean Architecture, one project per layer; contexts as folders; FA pinned [7.2.2,8.0.0); NSubstitute for ports only; Bogus with fixed seed + refDate. · informal: controllers only; 404 for malformed ids; singleton in-memory store; exception details never in responses



(b) Add to `## LESSON LOG`:
| L01 | __ | __ | .NET/SDK/runtime/LTS; C#→IL→JIT→Kestrel; controllers created per request; silent 404 without MapControllers | __ /5 | __ |
| L02 | __ | __ | pipeline order; request flow & [ApiController]; DI lifetimes & captive deps; DTOs vs over-posting; ValidateOnStart; ProblemDetails; await ≠ new thread; 💥 List<T> race | __ /5 | __ |
| L03 | __ | __ | dependency rule & what the compiler enforces; ports/adapters/composition root; xUnit discovery & AAA; FA 7 pinned, NSubstitute proxies, Bogus seed + refDate; 💥 Domain → EF Core package builds silently | __ /5 | __ |


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
- Inner layers define, outer layers implement; the layer that needs an interface owns it; only `Program.cs` knows every concrete class.
- The compiler refuses cyclic project references but accepts any NuGet package — guard Domain with an architecture test (L22).
- Trade-off: Clean Architecture buys a testable core and swappable infrastructure at the cost of more projects and ceremony; skip it for rule-free CRUD.
- Fake ports with NSubstitute, test adapters for real, never mock the domain.
- Failure mode: over-mocking gives green tests while DI wiring is broken — close it with API tests that boot the real app (L10).
- Repeatable data = fixed seed + fixed reference date + seeded Guids + one faker per test.