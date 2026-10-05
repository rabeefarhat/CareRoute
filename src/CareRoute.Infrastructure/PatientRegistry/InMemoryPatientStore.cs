// src/CareRoute.Infrastructure/PatientRegistry/InMemoryPatientStore.cs
using System.Collections.Concurrent;
using CareRoute.Application.PatientRegistry;
using CareRoute.Domain.PatientRegistry;

namespace CareRoute.Infrastructure.PatientRegistry;

public sealed class InMemoryPatientStore : IPatientStore
{
    private static readonly DoctorId SeedGp = new(Guid.Parse("d0c70000-0000-0000-0000-000000000001"));   // ①

    private readonly ConcurrentDictionary<PatientId, Patient> _patients = new();                          // ②

    public InMemoryPatientStore()
    {
        Seed("11111111-1111-1111-1111-111111111111", "62031411215", "Lotte", "Janssens");                   // ③
        Seed("22222222-2222-2222-2222-222222222222", "79092150802", "Wout", "Maes");
        Seed("33333333-3333-3333-3333-333333333333", "03110224558", "Noor", "Claes");
    }

    public Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Patient>>(_patients.Values.ToList());                                 // ④

    public Task<Patient?> GetByIdAsync(PatientId id, CancellationToken cancellationToken) =>
        Task.FromResult(_patients.TryGetValue(id, out var patient) ? patient : null);

    public Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        if (!_patients.TryAdd(patient.Id, patient))                                                        // ⑤
            throw new InvalidOperationException($"Patient {patient.Id} already exists.");
        return Task.CompletedTask;
    }

    private void Seed(string id, string nationalNumber, string firstName, string lastName) =>
        _patients[new PatientId(Guid.Parse(id))] = Patient.Register(
            new PatientId(Guid.Parse(id)),
            NationalNumber.Create(nationalNumber),
            PersonName.Create(firstName, lastName),
            SeedGp);
}