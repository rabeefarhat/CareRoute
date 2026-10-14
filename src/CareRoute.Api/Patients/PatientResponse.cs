// src/CareRoute.Api/Patients/PatientResponse.cs   (Api layer · Patient Registry)
namespace CareRoute.Api.Patients;

public sealed record PatientResponse(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth)
{
    public static PatientResponse From(Patient patient) =>                                    // ①
        new(patient.Id, patient.FirstName, patient.LastName, patient.DateOfBirth);
}