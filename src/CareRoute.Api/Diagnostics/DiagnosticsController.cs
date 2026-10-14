// src/CareRoute.Api/Diagnostics/DiagnosticsController.cs   (Api layer · DEV ONLY · replaced by a [DevelopmentOnly] filter in L18)
using Microsoft.AspNetCore.Mvc;

namespace CareRoute.Api.Diagnostics;

[ApiController]
[Route("api/diagnostics")]
public sealed class DiagnosticsController(IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("throw")]
    public IActionResult Throw()
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();                                                                   // 🔐
        }

        throw new InvalidOperationException("Simulated failure: table dbo.Patients locked by session 57.");
    }
}