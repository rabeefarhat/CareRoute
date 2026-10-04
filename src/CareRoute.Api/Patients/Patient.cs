// src/CareRoute.Api/Patients/Patient.cs  (Api layer for now — moves to Domain in L04)
namespace CareRoute.Api.Patients;

public sealed record Patient(Guid Id,string FirstName,string LastName,DateOnly DateOfBirth);