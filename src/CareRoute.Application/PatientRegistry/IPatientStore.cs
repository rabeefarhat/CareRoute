// src/CareRoute.Application/PatientRegistry/IPatientStore.cs
using CareRoute.Domain.PatientRegistry;

namespace CareRoute.Application.PatientRegistry;

public interface IPatientStore
{
    Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken);
    Task<Patient?> GetByIdAsync(PatientId id, CancellationToken cancellationToken);
    Task AddAsync(Patient patient, CancellationToken cancellationToken);
}