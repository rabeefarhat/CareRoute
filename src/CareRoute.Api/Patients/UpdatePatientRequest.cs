// src/CareRoute.Api/Patients/UpdatePatientRequest.cs   (Api layer · Patient Registry)
using System.ComponentModel.DataAnnotations;

namespace CareRoute.Api.Patients;

public sealed record UpdatePatientRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string? FirstName { get; init; }

    [Required, StringLength(100, MinimumLength = 1)]
    public string? LastName { get; init; }

    [Required]
    public DateOnly? DateOfBirth { get; init; }
}