// src/CareRoute.Api/Patients/PatientsController.cs   (Api layer · Patient Registry context)
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.Patients;

[ApiController]
[Route("api/patients")]
public sealed class PatientsController : ControllerBase
{ 
    private static readonly List<Patient> Patients =
    [
        new(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), "Emma", "Peeters", new DateOnly(1985, 7, 30)),
        new(Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), "Lucas", "Janssens", new DateOnly(1992, 2, 14)),
        new(Guid.Parse("a3bb189e-8bf9-3888-9912-ace4e6543002"), "Noor", "El Amrani", new DateOnly(2003, 11, 5)),
    ];

    [HttpGet]
    public ActionResult<IEnumerable<Patient>> GetAll() => Ok(Patients);

    [HttpGet("{id:guid}")]
    public ActionResult<Patient> GetById(Guid id)
    {
        var patient = Patients.Find(p => p.Id == id);
        return patient is null ? NotFound() : Ok(patient);
    }
}