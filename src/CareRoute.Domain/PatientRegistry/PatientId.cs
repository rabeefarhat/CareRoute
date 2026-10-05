// src/CareRoute.Domain/PatientRegistry/PatientId.cs
namespace CareRoute.Domain.PatientRegistry;

public readonly record struct PatientId
{
    public PatientId(Guid value) => Value = value;

    public Guid Value { get; }

    public bool IsEmpty => Value == Guid.Empty;

    public static PatientId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}