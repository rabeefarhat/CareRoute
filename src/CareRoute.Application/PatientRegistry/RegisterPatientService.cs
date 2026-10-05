// src/CareRoute.Application/PatientRegistry/RegisterPatientService.cs
using CareRoute.Domain.PatientRegistry;

namespace CareRoute.Application.PatientRegistry;

public sealed class RegisterPatientService(IPatientStore store)        // ①
{
    public async Task<Guid> RegisterAsync(                             // ②
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        CancellationToken cancellationToken)
    {
        var patient = new Patient(Guid.NewGuid(), firstName, lastName, dateOfBirth); // ③
        await store.AddAsync(patient, cancellationToken);              // ④
        return patient.Id;                                             // ⑤
    }
}