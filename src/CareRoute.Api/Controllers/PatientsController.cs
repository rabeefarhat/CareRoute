// src/CareRoute.Api/Controllers/PatientsController.cs
using CareRoute.Api.Patients;
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientsController(
    IPatientStore store,
    ILogger<PatientsController> logger) : ControllerBase                        // ①
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientResponse>>> GetAll(
        CancellationToken cancellationToken)                                    // ②
    {
        var patients = await store.GetAllAsync(cancellationToken);
        return Ok(patients.Select(PatientResponse.From).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var patient = await store.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return NotFound();                                                  // ③
        }
        return PatientResponse.From(patient);
    }

    [HttpPost]
    public async Task<ActionResult<PatientResponse>> Register(
        RegisterPatientRequest request, CancellationToken cancellationToken)    // ④
    {
        var patient = new Patient(
            Guid.NewGuid(),                                                     // ⑤
            request.FirstName!.Trim(),
            request.LastName!.Trim(),
            request.DateOfBirth!.Value);

        await store.AddAsync(patient, cancellationToken);

        logger.LogInformation("Registered patient {PatientId}", patient.Id);    // ⑥

        return CreatedAtAction(nameof(GetById), new { id = patient.Id },        // ⑦
            PatientResponse.From(patient));
    }
}