// src/CareRoute.Api/Patients/InMemoryPatientStore.cs   (Api layer for now → Infrastructure in L05 · Patient Registry)
using System.Collections.Concurrent;

namespace CareRoute.Api.Patients;

public sealed class InMemoryPatientStore : IPatientStore
{
    private readonly ConcurrentDictionary<Guid, Patient> _patients = new();          // ①

    public InMemoryPatientStore()
    {
        Seed(new Patient(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), "Emma", "Peeters", new DateOnly(1985, 7, 30)));
        Seed(new Patient(Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), "Lucas", "Janssens", new DateOnly(1992, 2, 14)));
        Seed(new Patient(Guid.Parse("a3bb189e-8bf9-3888-9912-ace4e6543002"), "Noor", "El Amrani", new DateOnly(2003, 11, 5)));
    }

    public Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Patient> snapshot = _patients.Values                          // ②
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToList();

        return Task.FromResult(snapshot);                                           // ③
    }

    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _patients.TryGetValue(id, out var patient);
        return Task.FromResult(patient);
    }

    public Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        if (!_patients.TryAdd(patient.Id, patient))                                 // ④
        {
            throw new InvalidOperationException("A patient with this id already exists.");
        }

        return Task.CompletedTask;
    }

    private void Seed(Patient patient) => _patients.TryAdd(patient.Id, patient);
}