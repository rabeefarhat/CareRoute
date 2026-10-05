// src/CareRoute.Domain/PatientRegistry/DoctorId.cs
namespace CareRoute.Domain.PatientRegistry;

// The GP lives in the Hospital Directory context later. Patient Registry only stores the doctor's ID —
// contexts share IDs, not classes (L03 DDD note).
public readonly record struct DoctorId
{
    public DoctorId(Guid value) => Value = value;

    public Guid Value { get; }

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString();
}