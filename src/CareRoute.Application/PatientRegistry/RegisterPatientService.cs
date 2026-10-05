// src/CareRoute.Application/PatientRegistry/RegisterPatientService.cs
using CareRoute.Domain.PatientRegistry;

namespace CareRoute.Application.PatientRegistry;

public sealed class RegisterPatientService(IPatientStore store)
{
    public async Task<PatientId> RegisterAsync(
        string firstName, string lastName, string nationalNumber, Guid gpId,
        CancellationToken cancellationToken)
    {
        var patient = Patient.Register(
            PatientId.New(),
            NationalNumber.Create(nationalNumber),
            PersonName.Create(firstName, lastName),
            new DoctorId(gpId));

        await store.AddAsync(patient, cancellationToken);
        return patient.Id;
    }
}