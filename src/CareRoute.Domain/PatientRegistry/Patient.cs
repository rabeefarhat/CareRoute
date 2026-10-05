// src/CareRoute.Domain/PatientRegistry/Patient.cs
using CareRoute.Domain.SharedKernel;

namespace CareRoute.Domain.PatientRegistry;

public sealed class Patient : Entity<PatientId>                                       // ①
{
    private Patient(PatientId id, NationalNumber nationalNumber, PersonName name, DoctorId gpId)
        : base(id)                                                                    // ②
    {
        NationalNumber = nationalNumber;
        Name = name;
        GpId = gpId;
    }

    public NationalNumber NationalNumber { get; }                                     // ③

    public PersonName Name
    {
        get;
        private set => field = value ?? throw new ArgumentNullException(nameof(value));   // ④ [C# 14]
    }

    public DoctorId GpId { get; private set; }                                        // ⑤

    public DateOnly DateOfBirth => NationalNumber.BirthDate;                          // ⑥

    public static Patient Register(PatientId id, NationalNumber nationalNumber, PersonName name, DoctorId gpId)
    {
        if (id.IsEmpty)                                                               // ⑦
            throw new DomainException("patient.id_required", "A patient must have an id.");
        ArgumentNullException.ThrowIfNull(nationalNumber);                            // ⑧
        if (gpId.IsEmpty)
            throw new DomainException("patient.gp_required", "Every patient must have an assigned GP.");

        return new Patient(id, nationalNumber, name, gpId);
    }

    public void ReassignGp(DoctorId newGpId)                                          // ⑨
    {
        if (newGpId.IsEmpty)
            throw new DomainException("patient.gp_required", "Every patient must have an assigned GP.");

        GpId = newGpId;
    }

    public void Rename(PersonName newName) => Name = newName;                         // ⑩
}