// tests/CareRoute.Domain.Tests/PatientFakerTests.cs
using CareRoute.Testing.Fakers;
using FluentAssertions;

namespace CareRoute.Domain.Tests;

public class PatientFakerTests
{
    [Fact]
    public void Same_seed_produces_the_same_first_patient()
    {
        var first = new PatientFaker(seed: 42).Generate();
        var second = new PatientFaker(seed: 42).Generate();

        first.Id.Should().Be(second.Id);
        first.NationalNumber.Should().Be(second.NationalNumber);
        first.Name.Should().Be(second.Name);
    }

    [Fact]
    public void Different_seeds_produce_different_patients()
    {
        var first = new PatientFaker(seed: 1).Generate();
        var second = new PatientFaker(seed: 2).Generate();

        first.Id.Should().NotBe(second.Id);
    }

    [Fact]
    public void Birth_dates_are_within_90_years_before_the_reference_date()
    {
        var reference = DateOnly.FromDateTime(CareRouteDataSet.DefaultReferenceDate);

        var patients = new PatientFaker().Generate(200);

        patients.Should().OnlyContain(p =>
            p.DateOfBirth >= reference.AddYears(-90) && p.DateOfBirth <= reference);
    }
}