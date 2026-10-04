// src/CareRoute.Api/Patients/IPatientStore.cs
namespace CareRoute.Api.Patients;

public interface IPatientStore
{
    Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken);
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Patient patient, CancellationToken cancellationToken);
}