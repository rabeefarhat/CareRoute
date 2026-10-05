// tests/CareRoute.Testing/Fakers/CareRouteDataSet.cs
using System.Globalization;
using Bogus;

namespace CareRoute.Testing.Fakers;

public static class CareRouteFakerExtensions
{
    public static CareRouteDataSet CareRoute(this Faker faker) => new(faker);          // ①
}

public sealed class CareRouteDataSet(Faker faker)
{
    public static readonly DateTime DefaultReferenceDate = new(2026, 1, 1);            // ②

    public string NationalNumber() => NationalNumber(DefaultReferenceDate);

    public string NationalNumber(DateTime referenceDate)
    {
        var birthDate = faker.Date.Past(90, referenceDate);                            // ③
        var serial = faker.Random.Int(1, 998);
        var firstNine = birthDate.ToString("yyMMdd", CultureInfo.InvariantCulture)
                        + serial.ToString("D3", CultureInfo.InvariantCulture);         // ④

        var number = long.Parse(firstNine, CultureInfo.InvariantCulture);
        if (birthDate.Year >= 2000)
            number += 2_000_000_000L;                                                  // ⑤

        var check = 97 - number % 97;
        return firstNine + check.ToString("D2", CultureInfo.InvariantCulture);         // ⑥
    }
}