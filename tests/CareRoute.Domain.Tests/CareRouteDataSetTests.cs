// tests/CareRoute.Domain.Tests/CareRouteDataSetTests.cs
using Bogus;
using CareRoute.Domain.PatientRegistry;
using CareRoute.Domain.SharedKernel;
using CareRoute.Testing.Fakers;
using FluentAssertions;

namespace CareRoute.Domain.Tests;

public class CareRouteDataSetTests
{
    [Fact]
    public void A_thousand_generated_national_numbers_are_all_valid()
    {
        var faker = new Faker { Random = new Randomizer(20260) };

        var numbers = Enumerable.Range(0, 1000).Select(_ => faker.CareRoute().NationalNumber()).ToList();

        var rejected = numbers.Where(n => !IsValid(n)).ToList();
        rejected.Should().BeEmpty("the faker must only generate numbers the domain accepts");
        numbers.Should().Contain(n => NationalNumber.Create(n).BirthDate.Year >= 2000,
            "the 2000+ branch must be exercised too");
    }

    private static bool IsValid(string raw)
    {
        try { NationalNumber.Create(raw); return true; }
        catch (DomainException) { return false; }
    }
}