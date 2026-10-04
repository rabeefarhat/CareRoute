CONTINUITY UPDATE

(a) Replace the whole `## CURRENT STATE` section with:

## CURRENT STATE

**Last completed lesson:** L01 — Start the project: Visual Studio 2026, .NET 10 & your first API
**Next lesson:** L02 — ASP.NET Core fundamentals: pipeline, DI, configuration, validation, errors
**Pace:** 4 lessons a day (~2 hours each, 9 days)
**Today:** Day 1 — L01 ✅, L02, L03, L04
**Behind schedule?** __

### Project state (CareRoute — hospital platform)
- **Last project step completed:** P01 First API — ✅ code provided — tick when it runs on your machine
- **Physical location:** __ (e.g., `D:\CareRoute\`)
- **Solution tree:**
```
  CareRoute/
  ├─ CareRoute.slnx
  ├─ .gitignore
  └─ src/
     └─ CareRoute.Api/
        ├─ Controllers/PatientsController.cs
        ├─ Patients/Patient.cs
        ├─ Properties/launchSettings.json
        ├─ appsettings.json
        ├─ appsettings.Development.json
        ├─ CareRoute.Api.http
        ├─ CareRoute.Api.csproj
        └─ Program.cs
```
- **What works:** `GET /api/patients` → 200 with 3 fictional patients; `GET /api/patients/{id:guid}` → 200 or 404 ProblemDetails; malformed id → empty 404; OpenAPI at `/openapi/v1.json` (Development only); manual test plan T1–T6 passes.
- **Differences from the lessons / known issues:** __ (none expected)
- **HTTPS port:** __ (7xxx)
- **GitHub repository URL:** __

### Code facts (VERIFIED names and signatures — Claude must use these, never guess)
- Project `CareRoute.Api` at `src/CareRoute.Api`, `<TargetFramework>net10.0</TargetFramework>`, Nullable + ImplicitUsings enabled.
- `Program.cs` (top-level statements): `AddControllers()`, `AddOpenApi()`, `MapOpenApi()` in Development, `UseHttpsRedirection()`, `UseAuthorization()`, `MapControllers()`.
- `namespace CareRoute.Api.Patients;` → `public sealed record Patient(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth);`
- `namespace CareRoute.Api.Controllers;` → `[ApiController] [Route("api/[controller]")] public sealed class PatientsController : ControllerBase`
  - `private static readonly IReadOnlyList<Patient> Patients` (3 hard-coded fictional patients)
  - `[HttpGet] public ActionResult<IReadOnlyList<Patient>> GetAll()`
  - `[HttpGet("{id:guid}")] public ActionResult<Patient> GetById(Guid id)`
- Fictional patient IDs: Lotte Peeters `3f2c1a9e-6b1d-4c7a-9a51-0f4e2b7d8c11` (1985-07-30); Jonas Maes `8d4e2b10-91a3-4f6e-b2c7-5a1d9e3f7b22` (1992-03-14); Amira El Idrissi `c7a91f35-2e4d-4b8a-8f60-3d2c1b0a9e33` (2001-11-02).
- `CareRoute.Api.http`: variable `@CareRoute.Api_HostAddress`; requests T1–T6.

### Azure resources (from L28)
| Resource | Name | Tier | Region | Running cost note |
|---|---|---|---|---|
| _(none yet)_ | | | | |

### Working agreements
_(unchanged — keep items 1–8)_

### Project steps
- [x] P01 First API · [ ] P02 Fundamentals · [ ] P03 Clean Architecture + test toolkit · [ ] P04 Domain I
_(rest unchanged)_

### Ubiquitous language (source of truth: docs/ubiquitous-language.md)
- _(empty — starts in L03)_ — terms used informally so far: Patient

### Environment
- Visual Studio 2026 version: __ · workloads: ASP.NET and web development, Azure development
- .NET SDK: __ (10.0.x)
_(rest unchanged)_

### Weak spots (max 15)
- (candidate — keep or delete) Why a malformed id gives an empty 404 (route constraint) while an unknown id gives a ProblemDetails 404 (action ran)
- (candidate — keep or delete) The C# → IL → JIT chain, explained out loud without notes
- (candidate — keep or delete) Which settings live in `launchSettings.json` vs `appsettings.json`

### Things that confused me
- __

### Skipped / deferred items
- _(none)_

### Decisions (ADRs, one line each)
- _(none yet — ADR-001 Clean Architecture comes in L03)_ · informal: controllers, not minimal APIs; 404 (not 400) for malformed ids via `:guid` constraint

(b) Add to `## LESSON LOG`:
| L01 | __ | __ | .NET/SDK/runtime/LTS; C#→IL→JIT→Kestrel; controllers created per request; silent 404 without MapControllers | __ /5 | __ |

(c) Append to `## KEY IDEAS CHEAT SHEET`:
- SDK builds, runtime runs; C# compiles to IL, the JIT turns IL into machine code at runtime, Kestrel serves HTTP.
- LTS (.NET 10, to Nov 2028) is supported, not frozen — monthly patches still have to be installed.
- Controllers are discovered once at startup but created new for every request — never keep state in controller fields.
- Trade-off: controllers give structure, filters and conventions; minimal APIs give less ceremony — pick one style per service.
- Failure mode: a 404 with an empty body means no endpoint matched — check `MapControllers()`, `public`, `: ControllerBase`, and route constraints.
- Status codes are the verdict: 404 for missing data, never 200 with an error in the body.