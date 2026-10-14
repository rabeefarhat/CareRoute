// src/CareRoute.Api/Patients/PatientsController.cs   (Api layer · Patient Registry context)
using CareRoute.Api.Common;
using CareRoute.Api.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.Patients;

[ApiController]
[Route("api/v1/patients")]                                                                              // ①
public sealed class PatientsController(IPatientStore store, ILogger<PatientsController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List patients, paged and optionally filtered by a name fragment.")]               // ②
    [ProducesResponseType<PagedResponse<PatientResponse>>(StatusCodes.Status200OK)]                     // ③
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public async Task<ActionResult<PagedResponse<PatientResponse>>> List(
        [FromQuery] PatientListQuery query, CancellationToken cancellationToken)                        // ④
    {
        var (items, totalCount) = await store.ListAsync(query.Search, query.Page, query.PageSize, cancellationToken);

        var response = new PagedResponse<PatientResponse>(
            items.Select(PatientResponse.From).ToList(),                                                // ⑤
            query.Page,
            query.PageSize,
            totalCount);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get one patient by id.")]
    [ProducesResponseType<PatientResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<ActionResult<PatientResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var patient = await store.GetByIdAsync(id, cancellationToken);
        return patient is null ? PatientNotFound() : Ok(PatientResponse.From(patient));
    }

    [HttpPost]
    [Consumes("application/json")]                                                                     // ⑥
    [EndpointSummary("Register a new patient. The server assigns the id.")]
    [ProducesResponseType<PatientResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType, "application/problem+json")]
    public async Task<ActionResult<PatientResponse>> Register(RegisterPatientRequest request, CancellationToken cancellationToken)
    {
        var patient = new Patient(
            Guid.NewGuid(),
            request.FirstName!.Trim(),
            request.LastName!.Trim(),
            request.DateOfBirth!.Value);

        await store.AddAsync(patient, cancellationToken);

        logger.LogInformation("Registered patient {PatientId}", patient.Id);

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, PatientResponse.From(patient));
    }

    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [EndpointSummary("Replace an existing patient. Idempotent. Never creates.")]
    [ProducesResponseType<PatientResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType, "application/problem+json")]
    public async Task<ActionResult<PatientResponse>> Replace(
        Guid id, UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        var patient = new Patient(                                                                      // 🔐 ⑦
            id,
            request.FirstName!.Trim(),
            request.LastName!.Trim(),
            request.DateOfBirth!.Value);

        if (!await store.UpdateAsync(patient, cancellationToken))
        {
            return PatientNotFound();                                                                   // ⑧
        }

        logger.LogInformation("Replaced patient {PatientId}", id);
        return Ok(PatientResponse.From(patient));
    }

    [HttpDelete("{id:guid}")]
    [EndpointSummary("Delete a patient. Idempotent: a second call returns 404.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!await store.RemoveAsync(id, cancellationToken))
        {
            return PatientNotFound();
        }

        logger.LogInformation("Deleted patient {PatientId}", id);
        return NoContent();                                                                             // ⑨
    }

    private ObjectResult PatientNotFound()                                                              // ⑩
    {
        var problem = ProblemDetailsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: StatusCodes.Status404NotFound,
            title: "Patient not found.",
            type: ProblemTypes.PatientNotFound,
            detail: "No patient exists with the requested id.");

        problem.Extensions["errorCode"] = ProblemTypes.Codes.PatientNotFound;
        return NotFound(problem);
    }
}