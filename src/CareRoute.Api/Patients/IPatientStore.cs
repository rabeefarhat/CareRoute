// src/CareRoute.Api/Patients/IPatientStore.cs   (Api layer for now → Application in L05 · Patient Registry)
namespace CareRoute.Api.Patients;

public interface IPatientStore
{
    Task<(IReadOnlyList<Patient> Items, int TotalCount)> ListAsync(            // ①
        string? search, int page, int pageSize, CancellationToken cancellationToken);

    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Patient patient, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Patient patient, CancellationToken cancellationToken);   // ②

    Task<bool> RemoveAsync(Guid id, CancellationToken cancellationToken);           // ③
}