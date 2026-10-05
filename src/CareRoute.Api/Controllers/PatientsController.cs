// src/CareRoute.Api/Controllers/PatientsController.cs
using CareRoute.Api.Patients;
using CareRoute.Application.PatientRegistry;
using CareRoute.Domain.PatientRegistry;
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.Controllers;

[ApiController]
[Route("api/patients")]
public sealed class PatientsController(
    IPatientStore store,
    RegisterPatientService registerPatient,
    ILogger<PatientsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var patients = await store.GetAllAsync(cancellationToken);
        return Ok(patients.Select(PatientResponse.From).ToList());                    // ①
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var patient = await store.GetByIdAsync(new PatientId(id), cancellationToken);  // ②
        if (patient is null)
            return NotFound();

        return PatientResponse.From(patient);                                          // ③
    }

    [HttpPost]
    public async Task<ActionResult<PatientResponse>> Register(
        RegisterPatientRequest request, CancellationToken cancellationToken)
    {
        var id = await registerPatient.RegisterAsync(
            request.FirstName!, request.LastName!, request.NationalNumber!, request.GpId!.Value,   // ④
            cancellationToken);

        logger.LogInformation("Registered patient {PatientId}", id.Value);            // ⑤

        var patient = await store.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = id.Value }, PatientResponse.From(patient!));
    }
}