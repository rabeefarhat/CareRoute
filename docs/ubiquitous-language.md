<!-- docs/ubiquitous-language.md -->
# CareRoute — Ubiquitous language

Words used by hospital staff AND in code. One meaning per bounded context.
All examples are fictional.

| Term | Context | Meaning | In code |
|---|---|---|---|
| Patient | Patient Registry | A person registered by a GP, identified by a national register number (from L04) | `Patient` |
| Register (a patient) | Patient Registry | Record a new patient in CareRoute | `RegisterPatientService.RegisterAsync` |
| GP | Patient Registry, Referrals | General practitioner (family doctor) who registers and refers patients | (L04+) |
| Referral | Referrals | A GP's request that a hospital department sees a patient | (L05) |
| Department | Hospital Directory | A hospital unit such as Cardiology | (later) |
| Bounded context | (all) | A boundary inside which each term has one meaning | folders per context |
| National number | 11-digit national register number: birth date (YYMMDD) + serial + mod-97 check; 2000+ births prefix 2 before the check | `NationalNumber` |
| Person name | A first and last name, trimmed, both required | `PersonName` |
| Assigned GP | The one GP responsible for a patient; every patient has exactly one | `Patient.GpId` (`DoctorId`) |
| Reassign GP | Change a patient's assigned GP | `Patient.ReassignGp` |
| Domain rule violation | A request rejected because it would break a business rule (HTTP 409, stable `code`) | `DomainException` |