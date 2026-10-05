// src/CareRoute.Domain/PatientRegistry/Patient.cs
namespace CareRoute.Domain.PatientRegistry;

public sealed record Patient(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth);