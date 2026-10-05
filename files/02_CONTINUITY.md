CONTINUITY UPDATE

(a) Replace the whole `## CURRENT STATE` section with:

## CURRENT STATE

**Last completed lesson:** L04 — Domain layer I: patients, value objects & invariants
**Next lesson:** L05 — Domain layer II: referral workflow, scheduling & domain events
**Pace:** 4 lessons a day (~2 hours each, 9 days)
**Today:** Day 1 — L01 ✅, L02 ✅, L03 ✅, L04 ✅ (Day 2 starts with L05)
**Behind schedule?** __

### Project state (CareRoute — hospital platform)
- **Last project step completed:** P04 Domain I — ✅ code provided — tick when it runs on your machine
- **Physical location:** __
- **Solution tree:**
```
  CareRoute/
  ├─ CareRoute.slnx                    (solution folders: src, tests, docs)
  ├─ .gitignore
  ├─ docs/
  │  ├─ ubiquitous-language.md         (+5 terms in L04)
  │  └─ adr/ADR-001-clean-architecture.md
  ├─ src/
  │  ├─ CareRoute.Domain/
  │  │  ├─ SharedKernel/DomainException.cs, Entity.cs
  │  │  └─ PatientRegistry/Patient.cs, PatientId.cs, DoctorId.cs, PersonName.cs, NationalNumber.cs
  │  ├─ CareRoute.Application/
  │  │  ├─ DependencyInjection.cs
  │  │  └─ PatientRegistry/IPatientStore.cs, RegisterPatientService.cs
  │  ├─ CareRoute.Infrastructure/
  │  │  ├─ DependencyInjection.cs
  │  │  └─ PatientRegistry/InMemoryPatientStore.cs
  │  └─ CareRoute.Api/
  │     ├─ Controllers/PatientsController.cs, DiagnosticsController.cs
  │     ├─ ErrorHandling/DomainExceptionHandler.cs, GlobalExceptionHandler.cs
  │     ├─ Options/ReferralOptions.cs
  │     ├─ Patients/RegisterPatientRequest.cs, PatientResponse.cs
  │     ├─ Properties/launchSettings.json
  │     ├─ appsettings.json, appsettings.Development.json
  │     ├─ CareRoute.Api.http          (+ requests L04-1 … L04-6)
  │     └─ Program.cs
  └─ tests/
     ├─ CareRoute.Testing/Fakers/PatientFaker.cs, CareRouteDataSet.cs
     ├─ CareRoute.Domain.Tests/
     │  ├─ PatientFakerTests.cs, CareRouteDataSetTests.cs, DomainDesignRulesTests.cs
     │  └─ PatientRegistry/NationalNumberTests.cs, PersonNameTests.cs, PatientTests.cs
     └─ CareRoute.Application.Tests/
        ├─ Infrastructure/InMemoryPatientStoreTests.cs   (temporary home — moves in L10)
        └─ PatientRegistry/RegisterPatientServiceTests.cs
```
- **What works:** POST /api/patients goes through Patient.Register; birth date derived from the national number; invalid national number / empty GP → 409 ProblemDetails with `code`; missing fields → 400; malformed id → 404 (route constraint); logs contain IDs and error codes only; ~55 test cases green.
- **Differences from the lessons / known issues:** __ (did DiagnosticsController need changes? __ ; xUnit v2 or v3: __)
- **Known limitations (by design):** national-number uniqueness not enforced (L07/L09); future birth dates accepted (L05 TimeProvider); real Belgian edge cases (bis numbers, unknown birth date) rejected; 409 mapping tested only via .http until L10.
- **HTTPS port:** __
- **GitHub repository URL:** __

### Code facts (VERIFIED names and signatures — Claude must use these, never guess)
- Projects unchanged from L03 (all net10.0, Nullable + ImplicitUsings; Domain has no refs/packages).
- `namespace CareRoute.Domain.SharedKernel;`
  - `public sealed class DomainException(string code, string message) : Exception(message)` → `public string Code { get; }`
  - `public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : struct, IEquatable<TId>` → `protected Entity(TId id)`, `public TId Id { get; }`, equality = same runtime type + same Id, `==`/`!=` overloaded.
- `namespace CareRoute.Domain.PatientRegistry;`
  - `public readonly record struct PatientId` → ctor `PatientId(Guid value)`, `Guid Value { get; }`, `bool IsEmpty`, `static PatientId New()`, `ToString()` = Guid.
  - `public readonly record struct DoctorId` → ctor `DoctorId(Guid value)`, `Guid Value { get; }`, `bool IsEmpty`, `ToString()` = Guid. (No New().)
  - `public sealed record PersonName` → private ctor; `const int MaxLength = 100`; `string First { get; }`, `string Last { get; }`; `static PersonName Create(string? first, string? last)` (trims; codes `person_name.first_required`, `person_name.last_required`, `person_name.too_long`); `ToString()` = initials "E. W.".
  - `public sealed record NationalNumber` → private ctor; `string Value { get; }` (11 clean digits), `DateOnly BirthDate { get; }`, `string Formatted` ("85.07.30-033.28"); `static NationalNumber Create(string? raw)` (strips . - space; codes `national_number.required`, `national_number.format`, `national_number.invalid_check`, `national_number.invalid_birth_date`); private `static int CheckDigitsFor(long number) => (int)(97 - number % 97)`; `const long Year2000Prefix = 2_000_000_000L`; `ToString()` = "85*******28".
  - `public sealed class Patient : Entity<PatientId>` → private ctor; `NationalNumber NationalNumber { get; }`; `PersonName Name { get; private set => field = value ?? throw new ArgumentNullException(...); }` [C# 14]; `DoctorId GpId { get; private set; }`; `DateOnly DateOfBirth => NationalNumber.BirthDate`; `static Patient Register(PatientId id, NationalNumber nationalNumber, PersonName name, DoctorId gpId)` (codes `patient.id_required`, `patient.gp_required`; null national number → ArgumentNullException); `void ReassignGp(DoctorId newGpId)` (code `patient.gp_required`); `void Rename(PersonName newName)`.
- `namespace CareRoute.Application.PatientRegistry;`
  - `public interface IPatientStore` → `Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken)`, `Task<Patient?> GetByIdAsync(PatientId id, CancellationToken cancellationToken)`, `Task AddAsync(Patient patient, CancellationToken cancellationToken)`.
  - `public sealed class RegisterPatientService(IPatientStore store)` → `public async Task<PatientId> RegisterAsync(string firstName, string lastName, string nationalNumber, Guid gpId, CancellationToken cancellationToken)`.
- `namespace CareRoute.Infrastructure.PatientRegistry;` → `public sealed class InMemoryPatientStore : IPatientStore` — parameterless ctor; `ConcurrentDictionary<PatientId, Patient>`; seeds 3 patients via Patient.Register with GP `d0c70000-0000-0000-0000-000000000001`: `11111111-…-111111111111` Lotte Janssens 62031411215; `22222222-…` Wout Maes 79092150802; `33333333-…` Noor Claes 03110224558 (2003, 2000+ rule); AddAsync throws InvalidOperationException on duplicate id.
- `namespace CareRoute.Api.Patients;`
  - `public sealed class RegisterPatientRequest` → `string? FirstName`, `string? LastName` ([Required, StringLength(PersonName.MaxLength)]), `string? NationalNumber` ([Required, StringLength(20)]), `Guid? GpId` ([Required]); all `{ get; init; }`. (DateOfBirth removed.)
  - `public sealed record PatientResponse(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth, string NationalNumber, Guid GpId)` → `static PatientResponse From(Patient patient)` (NationalNumber = Formatted).
- `PatientsController(IPatientStore store, RegisterPatientService registerPatient, ILogger<PatientsController> logger)`, `[Route("api/patients")]` → `GetAll(CancellationToken)`, `GetById(Guid id, CancellationToken)` on `{id:guid}`, `Register(RegisterPatientRequest, CancellationToken)` → CreatedAtAction(nameof(GetById), new { id = id.Value }, …); logs "Registered patient {PatientId}".
- `namespace CareRoute.Api.ErrorHandling;` → `public sealed class DomainExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<DomainExceptionHandler> logger) : IExceptionHandler` → 409, Title "A business rule was violated.", Type "https://careroute.example/problems/domain-rule", Extensions["code"]; logs Information "Request rejected by domain rule {ErrorCode}".
- `Program.cs`: `builder.Services.AddExceptionHandler<DomainExceptionHandler>();` directly BEFORE `AddExceptionHandler<GlobalExceptionHandler>()`; rest unchanged.
- `namespace CareRoute.Testing.Fakers;`
  - `public static class CareRouteFakerExtensions` → `public static CareRouteDataSet CareRoute(this Faker faker)`.
  - `public sealed class CareRouteDataSet(Faker faker)` → `public static readonly DateTime DefaultReferenceDate = new(2026, 1, 1)`; `string NationalNumber()`; `string NationalNumber(DateTime referenceDate)` (birth date `faker.Date.Past(90, referenceDate)`, serial 1–998).
  - `PatientFaker : Faker<Patient>` — `DefaultSeed = 20260`, `ReferenceDate = CareRouteDataSet.DefaultReferenceDate`, ctor `(int seed = DefaultSeed) : base("nl_BE")`, CustomInstantiator → `Patient.Register(new PatientId(f.Random.Guid()), NationalNumber.Create(f.CareRoute().NationalNumber(ReferenceDate)), PersonName.Create(f.Name.FirstName(), f.Name.LastName()), new DoctorId(f.Random.Guid()))`.
- Tests: NationalNumberTests (Valid_numbers_are_accepted_and_the_birth_date_is_derived [8 rows], Invalid_numbers_are_rejected_with_a_stable_code [9 rows], Separators_are_removed_from_the_stored_value, Leading_zeros_are_kept, Numbers_with_the_same_digits_are_equal_whatever_the_input_format, ToString_masks_the_birth_date_and_serial); PersonNameTests (A_blank_first_name_is_rejected, A_blank_last_name_is_rejected, Names_are_trimmed, A_name_of_exactly_the_maximum_length_is_accepted, A_name_one_character_too_long_is_rejected); PatientTests (7 facts); CareRouteDataSetTests (A_thousand_generated_national_numbers_are_all_valid); DomainDesignRulesTests (No_domain_type_has_a_public_setter — Public|Instance|Static|DeclaredOnly); RegisterPatientServiceTests (+An_invalid_national_number_never_reaches_the_store); InMemoryPatientStoreTests (count = 100 + 3 seeds); PatientFakerTests (same three names, new bodies).

### Azure resources (from L28)
| Resource | Name | Tier | Region | Running cost note |
|---|---|---|---|---|
| _(none yet)_ | | | | |

### Working agreements
_(unchanged — keep items 1–8)_

### Project steps
- [x] P01 First API · [x] P02 Fundamentals · [x] P03 Clean Architecture + test toolkit · [x] P04 Domain I · [ ] P05 Domain II
_(rest unchanged)_

### Ubiquitous language (source of truth: docs/ubiquitous-language.md)
- Patient, Register (a patient), GP, Referral, Department, Bounded context — contexts: Patient Registry, Referrals, Scheduling, Hospital Directory, Clinical Intake, Notifications, Identity & Access
- (L04) National number, Person name, Assigned GP, Reassign GP, Domain rule violation

### Environment
_(unchanged)_

### Weak spots (max 15)
- (keep or delete from L01) Malformed id → empty 404 vs unknown id → ProblemDetails 404
- (keep or delete from L01) C# → IL → JIT chain out loud
- (keep or delete from L01) launchSettings.json vs appsettings.json
- (keep or delete from L02) Why `[Required]` needs `DateOnly?` instead of `DateOnly`
- (keep or delete from L02) Captive dependency: explain it and its fix without notes
- (keep or delete from L02) Config binding is case-insensitive — what a "typo" test really needs
- (keep or delete from L03) What the compiler enforces (project refs, cycles) vs what it doesn't (packages)
- (keep or delete from L03) Stub vs spy vs mock vs fake with a CareRoute example each
- (keep or delete from L03) Why a Bogus seed alone isn't enough (refDate, Guid.NewGuid, shared faker)
- (candidate — keep or delete) Doing the mod-97 check by hand for a 2000+ number, and why the 2 prefix encodes the century
- (candidate — keep or delete) Why positional records/record structs break "no public setters" (`init`, `with`)
- (candidate — keep or delete) Business-rule error (DomainException → 409) vs programming error (ArgumentNullException → 500) vs input error (400)

### Things that confused me
- __
- __

### Skipped / deferred items
- Infrastructure tests live in CareRoute.Application.Tests/Infrastructure until L10.
- Curriculum correction: the L03 💥 "Domain → Infrastructure reference still builds" is refused by VS as a cycle; the 💥 used a Domain → EF Core package leak instead. Use the package version of this 💥 again in L22.
- Infrastructure tests live in CareRoute.Application.Tests/Infrastructure until L10.
- Curriculum correction: the L03 💥 "Domain → Infrastructure reference still builds" is refused by VS as a cycle; the 💥 used a Domain → EF Core package leak instead. Use the package version of this 💥 again in L22.
- National-number uniqueness → L07/L09 unique index. Future birth dates → L05 (TimeProvider). DomainException → 409 automated test → L10 (WebApplicationFactory).
### Decisions (ADRs, one line each)
- ADR-001: Clean Architecture, one project per layer; contexts as folders; FA pinned [7.2.2,8.0.0); NSubstitute for ports only; Bogus with fixed seed + refDate. · informal: controllers only; 404 for malformed ids; singleton in-memory store; exception details never in responses
- ADR-001: Clean Architecture, one project per layer; contexts as folders; FA pinned [7.2.2,8.0.0); NSubstitute for ports only; Bogus with fixed seed + refDate. · informal: controllers only; 404 for malformed ids; singleton in-memory store; exception details never in responses
- informal (L04): rich domain model; broken rules throw DomainException with stable code → 409 (not 422); messages never contain input; value objects = sealed records with get-only props and masked ToString; typed IDs = readonly record struct with explicit ctor; national number stored as string; birth date derived, not stored.



(b) Add to `## LESSON LOG`:
| L01 | __ | __ | .NET/SDK/runtime/LTS; C#→IL→JIT→Kestrel; controllers created per request; silent 404 without MapControllers | __ /5 | __ |
| L02 | __ | __ | pipeline order; request flow & [ApiController]; DI lifetimes & captive deps; DTOs vs over-posting; ValidateOnStart; ProblemDetails; await ≠ new thread; 💥 List<T> race | __ /5 | __ |
| L03 | __ | __ | dependency rule & what the compiler enforces; ports/adapters/composition root; xUnit discovery & AAA; FA 7 pinned, NSubstitute proxies, Bogus seed + refDate; 💥 Domain → EF Core package builds silently | __ /5 | __ |
| L04 | __ | __ | entity vs value object & record codegen; private ctor + Create; mod-97 + 2000 rule (string storage, long math); typed IDs & default hole; DomainException → 409, handler order; reflection test for public setters; 💥 %96 cascade + duplicated-formula trap | __ /5 | __ |


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
- Entities compare by ID and survive change; value objects compare by value and are replaced, never edited.
- Private constructor + Create: every object that exists is valid — parse raw input once, at the edge.
- National number: 11-digit string; check = 97 − (first 9 mod 97); born 2000+ → prefix 2; compute with long.
- Typed IDs make swapped IDs a compile error at zero runtime cost, but default = Guid.Empty must be guarded.
- Trade-off: a rich domain model makes invalid states impossible at the cost of mapping and serializer/ORM setup.
- Failure mode: a public setter bypasses every invariant while behaviour tests stay green — a reflection test catches it; register DomainExceptionHandler before the global handler or rules return 500.