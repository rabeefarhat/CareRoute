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