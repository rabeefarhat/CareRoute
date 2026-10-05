// src/CareRoute.Domain/PatientRegistry/NationalNumber.cs
using System.Globalization;
using CareRoute.Domain.SharedKernel;

namespace CareRoute.Domain.PatientRegistry;

public sealed record NationalNumber
{
    private const long Year2000Prefix = 2_000_000_000L;                          // ①

    private NationalNumber(string value, DateOnly birthDate)
    {
        Value = value;
        BirthDate = birthDate;
    }

    public string Value { get; }                                                 // ②
    public DateOnly BirthDate { get; }

    public string Formatted =>
        $"{Value[..2]}.{Value[2..4]}.{Value[4..6]}-{Value[6..9]}.{Value[9..]}";  // ③

    public static NationalNumber Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("national_number.required", "A national number is required.");

        var digits = raw.Replace(".", "").Replace("-", "").Replace(" ", "");     // ④
        if (digits.Length != 11 || !digits.All(char.IsAsciiDigit))               // ⑤
            throw new DomainException("national_number.format", "A national number must contain exactly 11 digits.");

        var firstNine = long.Parse(digits[..9], CultureInfo.InvariantCulture);   // ⑥
        var check = int.Parse(digits[9..], CultureInfo.InvariantCulture);

        int century;                                                             // ⑦
        if (CheckDigitsFor(firstNine) == check)
            century = 1900;
        else if (CheckDigitsFor(Year2000Prefix + firstNine) == check)
            century = 2000;
        else
            throw new DomainException("national_number.invalid_check", "The check digits of the national number are not correct.");

        var year = century + int.Parse(digits[..2], CultureInfo.InvariantCulture);
        var month = int.Parse(digits[2..4], CultureInfo.InvariantCulture);
        var day = int.Parse(digits[4..6], CultureInfo.InvariantCulture);

        if (month is < 1 or > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))   // ⑧
            throw new DomainException("national_number.invalid_birth_date", "The national number does not contain a valid birth date.");

        return new NationalNumber(digits, new DateOnly(year, month, day));       // ⑨
    }

    private static int CheckDigitsFor(long number) => (int)(97 - number % 97);   // ⑩

    public override string ToString() => $"{Value[..2]}*******{Value[9..]}";     // ⑪
}