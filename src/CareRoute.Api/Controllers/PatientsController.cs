// src/CareRoute.Api/Controllers/PatientsController.cs  (Api layer — HTTP only)
using CareRoute.Api.Patients;
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.Controllers;

[ApiController]                                                         // ①
[Route("api/[controller]")]                                             // ②
public sealed class PatientsController : ControllerBase                 // ③
{
    // Fictional patients. Hard-coded for L01; an injected store replaces this in L02.
    private static readonly IReadOnlyList<Patient> Patients =           // ④
    [
        new(Guid.Parse("3f2c1a9e-6b1d-4c7a-9a51-0f4e2b7d8c11"), "Lotte", "Peeters",  new DateOnly(1985, 7, 30)),
        new(Guid.Parse("8d4e2b10-91a3-4f6e-b2c7-5a1d9e3f7b22"), "Jonas", "Maes",     new DateOnly(1992, 3, 14)),
        new(Guid.Parse("c7a91f35-2e4d-4b8a-8f60-3d2c1b0a9e33"), "Amira", "El Idrissi", new DateOnly(2001, 11, 2))
    ];

    [HttpGet]                                                           // ⑤
    public ActionResult<IReadOnlyList<Patient>> GetAll() => Ok(Patients);

    [HttpGet("{id:guid}")]                                              // ⑥
    public ActionResult<Patient> GetById(Guid id)
    {
        var patient = Patients.FirstOrDefault(p => p.Id == id);        // ⑦
        return patient is null ? NotFound() : Ok(patient);             // ⑧
    }
}