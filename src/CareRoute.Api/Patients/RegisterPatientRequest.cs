// src/CareRoute.Api/Patients/RegisterPatientRequest.cs
using System.ComponentModel.DataAnnotations;

namespace CareRoute.Api.Patients;

public sealed record RegisterPatientRequest
{
    [Required]                                   // ①
    [StringLength(100, MinimumLength = 1)]
    public string? FirstName { get; init; }      // ②

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? LastName { get; init; }

    [Required]
    public DateOnly? DateOfBirth { get; init; }  // ③
}