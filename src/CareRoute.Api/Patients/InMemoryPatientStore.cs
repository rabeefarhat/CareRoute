// src/CareRoute.Api/Patients/InMemoryPatientStore.cs
using System.Collections.Concurrent;

namespace CareRoute.Api.Patients;

public sealed class InMemoryPatientStore : IPatientStore
{
    private static readonly Patient[] Seed =                                   // ①
    [
        new(Guid.Parse("3f2c1a9e-6b1d-4c7a-9a51-0f4e2b7d8c11"), "Lotte", "Peeters", new DateOnly(1985, 7, 30)),
        new(Guid.Parse("8d4e2b10-91a3-4f6e-b2c7-5a1d9e3f7b22"), "Jonas", "Maes", new DateOnly(1992, 3, 14)),
        new(Guid.Parse("c7a91f35-2e4d-4b8a-8f60-3d2c1b0a9e33"), "Amira", "El Idrissi", new DateOnly(2001, 11, 2)),
    ];

    private readonly ConcurrentDictionary<Guid, Patient> _patients = new();     // ②

    public InMemoryPatientStore()
    {
        foreach (var patient in Seed)
        {
            _patients.TryAdd(patient.Id, patient);
        }
    }

    public Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Patient> result = _patients.Values                        // ③
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToList();
        return Task.FromResult(result);                                         // ④
    }

    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_patients.TryGetValue(id, out var patient) ? patient : null);

    public Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        if (!_patients.TryAdd(patient.Id, patient))                             // ⑤
        {
            throw new InvalidOperationException($"Patient {patient.Id} already exists.");
        }
        return Task.CompletedTask;
    }
}