// src/CareRoute.Api/Controllers/PatientsController.cs
using CareRoute.Api.Patients;
using CareRoute.Application.PatientRegistry;
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientsController(IPatientStore store,RegisterPatientService registerPatient,ILogger<PatientsController> logger) : ControllerBase
{
    #region Get All
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var patients = await store.GetAllAsync(cancellationToken);
        return Ok(patients
            .OrderBy(p => p.LastName)                           
            .Select(PatientResponse.From)
            .ToList());
    }
    #endregion

    #region Get By Id
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var patient = await store.GetByIdAsync(id, cancellationToken);
        return patient is null
            ? Problem(statusCode: StatusCodes.Status404NotFound,   // ③
                      title: "Patient not found.",
                      detail: $"No patient with id '{id}'.")
            : Ok(PatientResponse.From(patient));
    }
    #endregion

    #region Register Patient

    [HttpPost]
    public async Task<ActionResult<PatientResponse>> Register(RegisterPatientRequest request,CancellationToken cancellationToken)
    {
        var id = await registerPatient.RegisterAsync(            
            request.FirstName!,
            request.LastName!,
            request.DateOfBirth!.Value,
            cancellationToken);

        logger.LogInformation("Registered patient {PatientId}", id);

        var created = await store.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, PatientResponse.From(created!)); 
    }

    #endregion
}