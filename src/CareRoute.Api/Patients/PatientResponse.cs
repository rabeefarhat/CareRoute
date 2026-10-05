// src/CareRoute.Api/Patients/PatientResponse.cs
using CareRoute.Domain.PatientRegistry;

namespace CareRoute.Api.Patients;

public sealed record PatientResponse(
    Guid Id, string FirstName, string LastName, DateOnly DateOfBirth, string NationalNumber, Guid GpId)
{
    public static PatientResponse From(Patient patient) => new(
        patient.Id.Value,
        patient.Name.First,
        patient.Name.Last,
        patient.DateOfBirth,
        patient.NationalNumber.Formatted,
        patient.GpId.Value);
}