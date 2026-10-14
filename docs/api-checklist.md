<!-- docs/api-checklist.md -->
# CareRoute API manual checklist (L03)

Run with `src/CareRoute.Api/CareRoute.Api.http`. Restart the API (Shift+F5, F5) before a full run:
the in-memory store resets to the 3 seeded patients.

| # | Rule protected | Request (.http #) | Expected | Result |
|---|---|---|---|---|
| C1 | Server-enforced max page size | #4 `pageSize=1000` | 400, errorCode `validation_failed`, `errors.PageSize` | ☐ |
| C2 | Lower bounds | `page=0` / `pageSize=0` | 400 | ☐ |
| C3 | Paging metadata | #2 `page=1&pageSize=2` | 2 items, totalCount 3, totalPages 2, hasNextPage true | ☐ |
| C4 | Last page | `page=2&pageSize=2` | 1 item, hasNextPage false | ☐ |
| C5 | Beyond last page | `page=5&pageSize=2` | 200, empty items, totalCount 3 | ☐ |
| C6 | Search allow-list | #3 `search=pee` | only Emma Peeters | ☐ |
| C7 | PUT idempotent | #7 sent twice | 200 both times, identical body; #5 shows the new last name | ☐ |
| C8 | PUT never creates | #8 unknown id | 404, errorCode `patient_not_found`; list count unchanged | ☐ |
| C9 | DELETE idempotent | #9 sent twice | 204, then 404; list no longer contains Lucas | ☐ |
| C10 | Unknown route is ProblemDetails | #10 | 404, `application/problem+json`, errorCode `not_found` | ☐ |
| C11 | Wrong method | #11 PATCH | 405, ProblemDetails, errorCode `method_not_allowed` | ☐ |
| C12 | Content negotiation | #12 text/plain | 415, errorCode `unsupported_media_type` | ☐ |
| C13 | No internals in 500s | `GET /api/diagnostics/throw` | 500, errorCode `internal_error`, traceId, no exception text | ☐ |
| C14 | No query string echoed | `GET /api/v1/doctors?search=Peeters` | `instance` is `/api/v1/doctors` (no "Peeters") | ☐ |
| C15 | Contract complete | #14 OpenAPI | every patient operation lists all its status codes | ☐ |