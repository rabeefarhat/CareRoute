// src/CareRoute.Api/Patients/RegisterPatientRequest.cs
using System.ComponentModel.DataAnnotations;
using CareRoute.Domain.PatientRegistry;

namespace CareRoute.Api.Patients;

public sealed class RegisterPatientRequest
{
    [Required, StringLength(PersonName.MaxLength)]
    public string? FirstName { get; init; }

    [Required, StringLength(PersonName.MaxLength)]
    public string? LastName { get; init; }

    [Required, StringLength(20)]
    public string? NationalNumber { get; init; }

    [Required]
    public Guid? GpId { get; init; }
}