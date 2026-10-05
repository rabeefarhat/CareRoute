// tests/CareRoute.Testing/Fakers/PatientFaker.cs
using Bogus;
using CareRoute.Domain.PatientRegistry;

namespace CareRoute.Testing.Fakers;

public sealed class PatientFaker : Faker<Patient>
{
    public const int DefaultSeed = 20260;                                      // ①
    private static readonly DateTime ReferenceDate = new(2026, 1, 1);          // ②

    public PatientFaker(int seed = DefaultSeed) : base(locale: "nl_BE")        // ③
    {
        UseSeed(seed);                                                         // ④
        CustomInstantiator(f => new Patient(                                   // ⑤
            Id: f.Random.Guid(),                                               // ⑥
            FirstName: f.Name.FirstName(),
            LastName: f.Name.LastName(),
            DateOfBirth: DateOnly.FromDateTime(
                f.Date.Past(yearsToGoBack: 90, refDate: ReferenceDate))));     // ⑦
    }
}