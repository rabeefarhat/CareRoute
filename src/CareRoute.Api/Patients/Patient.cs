// src/CareRoute.Api/Patients/Patient.cs   (Api layer · Patient Registry context · temporary)
namespace CareRoute.Api.Patients;

public sealed record Patient(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth);