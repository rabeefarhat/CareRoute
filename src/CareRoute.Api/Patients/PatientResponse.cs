// src/CareRoute.Api/Patients/PatientResponse.cs
namespace CareRoute.Api.Patients;

public sealed record PatientResponse(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth)
{
    public static PatientResponse From(Patient patient) =>
        new(patient.Id, patient.FirstName, patient.LastName, patient.DateOfBirth);
}