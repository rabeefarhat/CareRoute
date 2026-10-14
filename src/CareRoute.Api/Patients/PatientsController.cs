// src/CareRoute.Api/Patients/PatientsController.cs   (Api layer · Patient Registry context)
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.Patients;

[ApiController]
[Route("api/patients")]
public sealed class PatientsController(IPatientStore store, ILogger<PatientsController> logger) : ControllerBase   // ①
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetAll(CancellationToken cancellationToken)                // ②
    {
        var patients = await store.GetAllAsync(cancellationToken);
        return Ok(patients);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Patient>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var patient = await store.GetByIdAsync(id, cancellationToken);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpPost]
    public async Task<ActionResult<Patient>> Register(RegisterPatientRequest request, CancellationToken cancellationToken) // ③
    {
        var patient = new Patient(
            Guid.NewGuid(),                                                                                         // ④
            request.FirstName!.Trim(),
            request.LastName!.Trim(),
            request.DateOfBirth!.Value);

        await store.AddAsync(patient, cancellationToken);

        logger.LogInformation("Registered patient {PatientId}", patient.Id);                                        // ⑤

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);                                  // ⑥
    }
}