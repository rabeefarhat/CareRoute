// src/CareRoute.Api/Controllers/DiagnosticsController.cs
using Microsoft.AspNetCore.Mvc;
using CareRoute.Application.PatientRegistry;
using CareRoute.Domain.PatientRegistry;
namespace CareRoute.Api.Controllers;

[ApiController]
[Route("api/diagnostics")]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class DiagnosticsController(IHostEnvironment environment) : ControllerBase
{
    [HttpGet("throw")]
    public IActionResult Throw()
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }
        throw new InvalidOperationException("Deliberate test exception from CareRoute diagnostics.");
    }
}