<!-- docs/api-guidelines.md -->
# CareRoute API guidelines (v1)

## 1. Resources and URLs
- Base path: `/api/v{major}`. Current: `/api/v1`. Operational endpoints (`/health`) are not versioned.
- Plural, lowercase, kebab-case nouns: `/patients`, `/referral-policy`.
- Ids are GUIDs, server-generated, never sequential.
- Nest at most one level: `/patients/{id}/referrals`.
- Non-CRUD actions are `POST` sub-resources: `POST /referrals/{id}/submit`.

## 2. Methods
| Method | Use | Idempotent | Success |
|---|---|---|---|
| GET | read; never changes state | yes | 200 |
| POST | create (server picks id) or action | no (idempotency keys from L37) | 201 + Location / 200 / 202 |
| PUT | full replace of an existing resource; never creates | yes | 200 + body |
| DELETE | remove | yes (second call → 404) | 204 |

## 3. Status codes
- 400 validation · 401 not authenticated · 403 authenticated but not allowed · 404 not found
- **Patient data the caller may not see → 404, not 403** (do not confirm existence).
- 409 state conflict · 412 If-Match mismatch · 415 wrong Content-Type · 429 rate limited · 500/503 server.
- Never 200 with an error body.

## 4. Errors
- Every 4xx/5xx body is ProblemDetails (RFC 9457), `application/problem+json`.
- `type` = `https://careroute.example/problems/{kind}`; `errorCode` extension = stable snake_case code.
- Clients branch on `errorCode`, never on `title`/`detail`.
- 500s never include exception messages or stack traces; they include `traceId`.

## 5. Collections
- Always paged: `page` (1..10000, default 1), `pageSize` (1..100, default 20). Out of range → 400 (never clamped silently).
- Response: `{ items, page, pageSize, totalCount, totalPages, hasNextPage }`.
- Sort is deterministic and ends with the id.
- Filtering by an allow-list of parameters (`search` = name fragment).

## 6. Versioning
- Additive changes (new optional fields, new endpoints) stay in v1.
- Breaking changes (remove/rename field, change type, stricter validation) → v2, run both, announce a sunset date.

## 7. Security rules
- No secrets, tokens or exact personal identifiers (national number) in URLs or query strings — they land in logs. Use a POST body.
- Name fragments in `search` are accepted and documented as personal data in logs.
- Responses expose DTOs, never storage/domain objects.
- Every endpoint documents all its response codes in OpenAPI.

## 8. Concurrency (planned)
- Updates will require `If-Match` with the resource's ETag; mismatch → 412.