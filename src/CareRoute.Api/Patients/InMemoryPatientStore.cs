// src/CareRoute.Api/Patients/InMemoryPatientStore.cs   (Api layer for now → Infrastructure in L05 · Patient Registry)
using System.Collections.Concurrent;

namespace CareRoute.Api.Patients;

public sealed class InMemoryPatientStore : IPatientStore
{
    private readonly ConcurrentDictionary<Guid, Patient> _patients = new();

    public InMemoryPatientStore()
    {
        Seed(new Patient(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), "Emma", "Peeters", new DateOnly(1985, 7, 30)));
        Seed(new Patient(Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), "Lucas", "Janssens", new DateOnly(1992, 2, 14)));
        Seed(new Patient(Guid.Parse("a3bb189e-8bf9-3888-9912-ace4e6543002"), "Noor", "El Amrani", new DateOnly(2003, 11, 5)));
    }

    public Task<(IReadOnlyList<Patient> Items, int TotalCount)> ListAsync(
        string? search, int page, int pageSize, CancellationToken cancellationToken)
    {
        IEnumerable<Patient> query = _patients.Values;                                      // ①

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||          // ②
                p.LastName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var ordered = query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ThenBy(p => p.Id)                                                              // ③
            .ToList();

        IReadOnlyList<Patient> items = ordered
            .Skip((page - 1) * pageSize)                                                    // ④
            .Take(pageSize)
            .ToList();

        return Task.FromResult((items, ordered.Count));                                     // ⑤
    }

    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _patients.TryGetValue(id, out var patient);
        return Task.FromResult(patient);
    }

    public Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        if (!_patients.TryAdd(patient.Id, patient))
        {
            throw new InvalidOperationException("A patient with this id already exists.");
        }

        return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(Patient patient, CancellationToken cancellationToken)
    {
        while (_patients.TryGetValue(patient.Id, out var current))                          // ⑥
        {
            if (_patients.TryUpdate(patient.Id, patient, current))                          // ⑦
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);                                                       // ⑧
    }

    public Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_patients.TryRemove(id, out _));                                     // ⑨

    private void Seed(Patient patient) => _patients.TryAdd(patient.Id, patient);
}