// src/CareRoute.Domain/PatientRegistry/PersonName.cs
using CareRoute.Domain.SharedKernel;

namespace CareRoute.Domain.PatientRegistry;

public sealed record PersonName
{
    public const int MaxLength = 100;

    private PersonName(string first, string last)
    {
        First = first;
        Last = last;
    }

    public string First { get; }
    public string Last { get; }

    public static PersonName Create(string? first, string? last)
    {
        var trimmedFirst = first?.Trim();
        var trimmedLast = last?.Trim();

        if (string.IsNullOrEmpty(trimmedFirst))
            throw new DomainException("person_name.first_required", "A first name is required.");
        if (string.IsNullOrEmpty(trimmedLast))
            throw new DomainException("person_name.last_required", "A last name is required.");
        if (trimmedFirst.Length > MaxLength || trimmedLast.Length > MaxLength)
            throw new DomainException("person_name.too_long", $"Names can be at most {MaxLength} characters.");

        return new PersonName(trimmedFirst, trimmedLast);
    }

    public override string ToString() => $"{First[0]}. {Last[0]}.";
}