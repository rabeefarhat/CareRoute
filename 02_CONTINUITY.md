CONTINUITY UPDATE

(a) ## CURRENT STATE

**Last completed lesson:** L03 — REST API design done right
**Next lesson:** L04 — Git & DevOps foundations: branches, pull requests and CI
**Pace:** 4 lessons a day · **Today:** Day 1 (L01 ✓, L02 ✓, L03 ✓, L04) · **Behind schedule?** __

### Project state
- **Last project step completed:** P03 — ✅ code provided — tick when it runs on your machine [ ]
- **Physical location:** D:\CareRoute\ (__ confirm)
- **Solution tree:**
  CareRoute.slnx · .gitignore · docs/{api-guidelines.md, api-checklist.md} · src/CareRoute.Api/{Common/PagedResponse.cs, Configuration/{ReferralOptions.cs, ReferralPolicyController.cs}, Diagnostics/DiagnosticsController.cs (DEV ONLY), Errors/{GlobalExceptionHandler.cs, ProblemTypes.cs}, Patients/{Patient.cs, IPatientStore.cs, InMemoryPatientStore.cs, PatientsController.cs, RegisterPatientRequest.cs, UpdatePatientRequest.cs, PatientListQuery.cs, PatientResponse.cs}, Program.cs, appsettings.json, appsettings.Development.json, CareRoute.Api.http, Properties/launchSettings.json}
- **What works:** GET api/v1/patients?page&pageSize&search (PagedResponse; pageSize 1..100, page 1..10000, out of range → 400; sort LastName, FirstName, Id) · GET api/v1/patients/{id} 200/404 patient_not_found · POST api/v1/patients 201 + Location · PUT api/v1/patients/{id} 200 / 404 (never creates, idempotent) · DELETE api/v1/patients/{id} 204 then 404 · GET api/v1/referral-policy · /health · api/diagnostics/throw (DEV ONLY, unversioned) · all errors ProblemDetails with type https://careroute.example/problems/*, errorCode, traceId, instance = path only · unknown route 404 / wrong method 405 with ProblemDetails · OpenAPI documents all response codes · 15-row manual checklist ticked __
- **Branch / PR:** still committed to `master` for L03. Commits: docs: add API guidelines · feat(api): consistent problem types and status-code pages · feat(patients): paging, search, PUT and DELETE under api/v1 · docs(api): OpenAPI response types and summaries · test(api): manual REST checklist. From L04: branches + PRs; rename `master` → `main` planned in L04.
- **Differences from the lessons / known issues:** ETag/If-Match is concept only (implemented once EF Core gives a row version). __
- **Ports:** API HTTPS 7038 · API HTTP 5201
- **GitHub repository URL:** https://github.com/rabeefarhat/CareRoute.git

### Code facts
- `IPatientStore`: ListAsync(search, page, pageSize, ct) → (IReadOnlyList<Patient> Items, int TotalCount); GetByIdAsync; AddAsync; UpdateAsync → bool; RemoveAsync → bool. `GetAllAsync` removed on purpose (no unbounded list).
- `InMemoryPatientStore.UpdateAsync` uses a TryGetValue + TryUpdate compare-and-swap loop (prevents resurrecting a concurrently deleted patient).
- Controllers return `PatientResponse` (record, static From(Patient)); Patient is never serialized directly.
- `PatientsController` actions: List, GetById, Register, Replace, Delete; private PatientNotFound() via ProblemDetailsFactory with errorCode `patient_not_found`.
- `ProblemTypes` (CareRoute.Api.Errors): BaseUri https://careroute.example/problems/, ForStatus(int) → (Type, Code); errors customized in AddProblemDetails(CustomizeProblemDetails): overrides non-CareRoute type, TryAdd errorCode, Instance ??= Request.Path.
- GlobalExceptionHandler no longer sets Type (customizer assigns internal-error).
- Program.cs order: UseExceptionHandler → UseStatusCodePages → MapOpenApi (Dev) → UseHttpsRedirection → UseAuthorization → MapHealthChecks("/health") → MapControllers.
- No class-level [Produces] (would force problem responses to application/json).

### Identity facts — none yet
### Azure resources — none
### Ubiquitous language — (starts L05); terms in use: Patient, GP (planned), Referral policy

### Candidate weak spots
- Why DELETE returning 204 then 404 still counts as idempotent
- When status-code pages runs vs when [ApiController]'s ProblemDetailsFactory runs
- Offset vs keyset cost model (scan vs seek) and why the tie-breaker matters

(b) ## LESSON LOG row
| L03 | __ | __ | REST resources; safe vs idempotent; 401/403/404 policy; offset vs keyset paging; URL versioning; ProblemDetails type + errorCode; ETag/If-Match preview; GET-that-deletes break-it | __ | __ |

(c) ## KEY IDEAS CHEAT SHEET
- Idempotent = same final state, not same response (DELETE: 204 then 404 is fine).
- Trade-off: offset paging allows page jumps but scans past skipped rows; keyset seeks but can't jump. Always end the sort with a unique id.
- Trade-off: 404 instead of 403 for hidden patient data prevents enumeration but makes support harder — log the real reason with the traceId.
- Failure mode: a GET that changes state is triggered by prefetchers, link previews, crawlers and scanners.
- Production rule: version from day one, server-enforced page limits, one ProblemDetails shape with stable type + errorCode, no identifiers or secrets in URLs.

(d) ## INTERVIEW STORY BANK
- S: CareRoute's patient API grew without a design. T: Make it a stable contract for future partners. A: Wrote API guidelines, moved routes under /api/v1, added server-enforced paging with a deterministic sort, idempotent PUT/DELETE, and one ProblemDetails shape (type URI + errorCode + traceId) — including unknown routes via status-code pages. R: A 15-check manual checklist proves limits, idempotency and error consistency; OpenAPI now documents every outcome.
- Found a subtle race: an update implemented as "check exists, then overwrite" could resurrect a patient deleted in between; replaced it with a compare-and-swap (TryUpdate) loop, the same idea as ETag optimistic concurrency.