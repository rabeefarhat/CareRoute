// src/CareRoute.Api/Patients/RegisterPatientRequest.cs   (Api layer · Patient Registry context)
using System.ComponentModel.DataAnnotations;

namespace CareRoute.Api.Patients;

public sealed record RegisterPatientRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string? FirstName { get; init; }

    [Required, StringLength(100, MinimumLength = 1)]
    public string? LastName { get; init; }

    [Required]
    public DateOnly? DateOfBirth { get; init; }
}